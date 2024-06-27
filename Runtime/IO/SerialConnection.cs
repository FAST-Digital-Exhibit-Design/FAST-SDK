//=============================================================================
// FAST SDK
// A software development kit for creating FAST digital exhibit experiences
// in Unity.
//
// Copyright (C) 2024 Museum of Science, Boston
// <https://www.mos.org/>
//
// This software was developed through a grant to the Museum of Science, Boston
// from the Institute of Museum and Library Services under
// Award #MG-249646-OMS-21. For more information about this grant, see
// <https://www.imls.gov/grants/awarded/mg-249646-oms-21>.
//
// This software is open source: you can redistribute it and/or modify
// it under the terms of the MIT License.
//
// This software is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// MIT License for more details.
//
// You should have received a copy of the MIT License along with this software.
// If not, see <https://opensource.org/license/MIT>.
//=============================================================================

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;

namespace FAST
{
    /// <summary>
    /// Provides methods to communicate over a serial port.
    /// </summary>
    /// <remarks>
    /// Receiving data is threaded for high-performance.
    /// </remarks>
    /// @note This class is designed for <see langword="string"/> messages. 
    /// @see <a href="https://learn.microsoft.com/en-us/dotnet/api/system.io.ports.serialport">
    /// System.IO.Ports.SerialPort</a>
    /// @see <a href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread">
    /// System.Threading.Thread</a>
    public class SerialConnection : MonoBehaviour
    {
        /// <summary>
        /// The baud rates available for Arduino.
        /// </summary>
        public enum BaudRates
        {
            _300 = 300, _600 = 600, _750 = 750, _1200 = 1200, _2400 = 2400, _4800 = 4800, _9600 = 9600,
            _14400 = 14400, _19200 = 19200, _28800 = 28800, _31250 = 31250, _38400 = 38400, _57600 = 57600, _74880 = 74880,
            _115200 = 115200, _230400 = 230400, _250000 = 250000, _460800 = 460800, _500000 = 500000, _921600 = 921600,
            _1000000 = 1000000, _2000000 = 2000000
        }

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// Identifies this instance.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.SerialConnection"/> at runtime.
        /// </remarks>
        public string id;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The COM port to use for serial communication.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.SerialConnectionSettings"/> at runtime.
        /// </remarks>
        [Range(1, 256)]
        public int comPort = 1;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The baud rate to use for serial communication.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.SerialConnectionSettings"/> at runtime.
        /// </remarks>
        public BaudRates baudRate = BaudRates._9600;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The number of milliseconds before a time-out occurs when a read operation does not finish.
        /// </summary>
        public int readTimeout = 10;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The number of milliseconds before a time-out occurs when a write operation does not finish.
        /// </summary>
        public int writeTimeout = 10;

        /// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Set to <see langword="true"/> if incoming the data should overwrite the previous data. 
        /// Otherwise the data will added to <see cref="FAST.SerialConnection.Data"/> in the order it is recieved.
		/// </summary>
        [SerializeField]
        private bool isOverwrite = false;

        /// <summary>
		/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
		/// Set to <see langword="true"/> if all serial activity should be logged to the 
		/// Editor Console or Player log.
		/// </summary>
        /// <remarks>
        /// Recieved messages are not logged because the communication may be too frequent.
        /// </remarks>
        [Space(10)]
        public bool isLogging = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityAction"/></c> 
        /// when data is received.
        /// </summary>
        /// @warning <remarks>
        /// Use with caution if communication might be frequent.
        /// <see cref="FAST.SerialConnection"/> receives data on a separate thread, 
        /// but these callbacks execute on Unity's main thread.
        /// Use <see cref="FAST.SerialConnection.isOverwrite"/> to only invoke this event on 
        /// the most recent data received.
        /// </remarks>
        public UnityAction<string> onDataReceivedEvent;

        private SerialPort serialPort;

        private readonly object dataLock = new();
        private readonly List<string> data = new();

        private readonly object queueLock = new();
        private readonly Queue<string> dataQueue = new();

        private Thread thread;
        private bool isThreadRunning = false;

        /// <summary>
        /// Gets the list of data recieved since the last read.
        /// </summary>
        /// <remarks>
        /// The list of data is ordered from oldest to newest. If 
        /// <see cref="FAST.SerialConnection.isOverwrite"/> is <see langword="true"/>, 
        /// the list only has 1 element.<br/>
        /// After reading, the list is cleared from <see cref="FAST.SerialConnection"/>.
        /// </remarks>
        public List<string> Data
        {
            get
            {
                lock (dataLock) {
                    List<string> dataCopy = new(data);
                    data.Clear();
                    return dataCopy;
                }
            }
        }

        /// <summary>
        /// Opens a serial port connection.
        /// </summary>
        /// <remarks>
        /// A <c style="color:DarkRed;"><see cref="Thread"/></c> is started if one doesn't exist.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if successful, else <see langword="false"/>
        /// </returns>
        /// @note Instead of calling @c Open() and configuring a @ref FAST.SerialConnection yourself, 
        /// @ref FAST.SerialConnectionLoader and @ref FAST.SerialConnectionSettings should be used to open 
        /// a serial connection with settings from file during the application load screen.
        public bool Open()
        {
            bool isConnected = false;
            try {
                serialPort = new("COM" + comPort.ToString(), (int)baudRate);
                serialPort.ReadTimeout = readTimeout;
                serialPort.WriteTimeout = writeTimeout;

                serialPort.Open();
                serialPort.DtrEnable = true;

                if (thread == null) {
                    StartThread();
                }

                isConnected = true;
                Print($"Serial connection opened!");
            }
            catch (Exception exception) {
                Debug.Log("ERROR\t" + $"{id}: Failed to open serial connection\n{exception.Message}\n");
            }
            return isConnected;
        }

        /// <summary>
        /// Writes a <see langword="string"/> with a new line character to the serial port.
        /// </summary>
        /// <param name="data">The <see langword="string"/> data to send.</param>
        public void SendAsLine(string data)
        {
            if (serialPort != null && serialPort.IsOpen) {
                serialPort.WriteLine(data);
            }

            Print("Sent data: " + data);
        }

        /// <summary>
        /// Writes a <see langword="string"/> to the serial port.
        /// </summary>
        /// <param name="data">The <see langword="string"/> data to send.</param>
        public void Send(string data)
        {
            if (serialPort != null && serialPort.IsOpen) {
                serialPort.Write(data);
            }

            Print("Sent data: " + data);
        }

        private void StartThread()
        {
            if (thread != null) {
                return;
            }

            try {
                thread = new(new ThreadStart(RunThread));
                thread.IsBackground = true;

                isThreadRunning = true;
                thread.Start();

                Print("Serial thread started!");
            }
            catch (Exception exception) {
                Debug.Log("ERROR\t" + $"{id}: Failed to start serial thread\n{exception.Message}\n");
            }
        }

        private void RunThread()
        {
            while (isThreadRunning) {
                if (serialPort == null) {
                    break;
                }
                try {
                    if (serialPort.IsOpen) {
                        string message = serialPort.ReadLine();

                        if (message != null && message != "") {
                            lock (dataLock) {
                                if (isOverwrite) {
                                    data.Clear();
                                }
                                data.Add(message);
                            }

                            if (onDataReceivedEvent != null) {
                                lock (queueLock) {
                                    if (isOverwrite) {
                                        dataQueue.Clear();
                                    }
                                    dataQueue.Enqueue(message);
                                }
                            }
                        }
                    }
                }
                catch (TimeoutException) { }
                catch (Exception exception) {
                    if (serialPort.IsOpen) {
                        Debug.Log("ERROR\t" + $"{id}: Failed to receive serial data\n{exception.Message}\n");
                    }
                    else {
                        Debug.Log("ERROR\t" + $"{id}: Serial connnection was closed while reading data\n{exception.Message}\n");
                    }
                }
            }

            Print("Serial thread completed!");
        }

        private void Update()
        {
            if (onDataReceivedEvent != null) {
                lock (queueLock) {
                    while (dataQueue.Count > 0) {
                        onDataReceivedEvent.Invoke(dataQueue.Dequeue());
                    }
                }
            }
        }

        private void Close()
        {
            if (serialPort != null) {
                try {
                    serialPort.Close();
                    serialPort = null;
                }
                catch (Exception exception) {
                    if (serialPort == null || serialPort.IsOpen == false) {
                        Debug.Log("ERROR\t" + $"{id}: Serial connection has already been closed\n{exception.Message}\n");
                    }
                    else {
                        Debug.Log("ERROR\t" + $"{id}: Failed to close serial connection\n{exception.Message}\n");
                    }
                }
            }

            Print("Serial connection closed!");
        }

        private void StopThread()
        {
            if (thread != null) {
                try {
                    // Let the while loop complete to end the thread
                    isThreadRunning = false;
                    Thread.Sleep(100);

                    // Otherwise, force the thread to end
                    if (thread.IsAlive) {
                        thread.Abort();
                        Thread.Sleep(100);
                    }

                    thread = null;
                }
                catch (Exception exception) {
                    Debug.Log("ERROR\t" + $"{id}: Failed to stop serial thread\n{exception.Message}\n");
                }
            }

            Print("Serial thread stopped!");
        }

        private void OnDestroy()
        {
            StopThread();
            Thread.Sleep(500);

            Close();
            Thread.Sleep(500);
        }

        private void Print(string message)
        {
            if (isLogging) {
                Debug.Log($"{id}: {message}");
            }
        }
    }
}
