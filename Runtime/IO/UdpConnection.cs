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
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.Events;


namespace FAST
{
    /// <summary>
    /// Provides methods to communicate over User Datagram Protocol (UDP).
    /// </summary>
    /// <remarks>
    /// Receiving data is threaded for high-performance.
    /// </remarks>
    /// @see <a href="https://learn.microsoft.com/en-us/dotnet/api/system.net.sockets.udpclient">
    /// System.Net.Sockets.UdpClient</a>
    /// @see <a href="https://learn.microsoft.com/en-us/dotnet/api/system.threading.thread">
    /// System.Threading.Thread</a>
    public class UdpConnection : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// Identifies this instance.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.UdpConnectionSettings"/> at runtime.
        /// </remarks>
        public string id;


        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The port of the local computer to recieve data on.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.UdpConnectionSettings"/> at runtime.
        /// </remarks>
        [Header("Local computer")]
        [Range(10000, 65000)]
        public int localReceivePort = 60000;

        /// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Set to <see langword="true"/> if incoming the datagram should overwrite the previous datagram. 
        /// Otherwise the datagram will added to <see cref="FAST.UdpConnection.Data"/> in the order it is recieved.
		/// </summary>
        [SerializeField]
        private bool isOverwrite = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The port of the local computer to send data from.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.UdpConnectionSettings"/> at runtime.
        /// </remarks>
        [Range(10000, 65000)]
        public int localSendPort = 60001;

        /// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Set to <see langword="true"/> if UDP messages should be broadcast to a subnet.
		/// </summary>
        /// <remarks>
        /// This functions the same as <c style="color:DarkRed;"><see cref="UdpClient.EnableBroadcast"/></c>.
        /// </remarks>
        [SerializeField]
        private bool isEnableBroadcast = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The IP address of the remote computer to communicate with.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.UdpConnectionSettings"/> at runtime.
        /// </remarks>
        [Header("Remote computer")]
        public string remoteIpAddress = "0.0.0.0";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The port of the remote computer to send data to.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.UdpConnectionSettings"/> at runtime.
        /// </remarks>
        [Range(10000, 65000)]
        public int remotePort = 50000;

        /// <summary>
		/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
		/// Set to <see langword="true"/> if all UDP activity should be logged to the 
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
        /// when a datagram is received.
        /// </summary>
        /// @warning <remarks>
        /// Use with caution if communication might be frequent.
        /// <see cref="FAST.UdpConnection"/> receives data on a separate thread, 
        /// but these callbacks execute on Unity's main thread.
        /// Use <see cref="FAST.UdpConnection.isOverwrite"/> to only invoke this event on 
        /// the most recent datagram received.
        /// </remarks>
        public UnityAction<byte[]> onDataReceivedEvent;

        private UdpClient udpSender;
        private volatile UdpClient udpReceiver;
        private IPEndPoint remoteIPEndPoint;

        private readonly object dataLock = new();
        private readonly List<byte[]> data = new();

        private readonly object queueLock = new();
        private readonly Queue<byte[]> dataQueue = new();

        private Thread thread;
        private bool isThreadRunning = false;

        /// <summary>
        /// Gets the list of datagrams recieved since the last read.
        /// </summary>
        /// <remarks>
        /// The list of datagrams is ordered from oldest to newest. If 
        /// <see cref="FAST.UdpConnection.isOverwrite"/> is <see langword="true"/>, 
        /// the list only has 1 element.<br/>
        /// After reading, the list is cleared from <see cref="FAST.UdpConnection"/>.
        /// </remarks>
        public List<byte[]> Data
        {
            get
            {
                lock (dataLock) {
                    List<byte[]> dataCopy = new(data);
                    data.Clear();
                    return dataCopy;
                }
            }
        }

        /// <summary>
        /// Opens a UDP connection.
        /// </summary>
        /// <remarks>
        /// The connection is initialized by binding the local send and receive ports. Also, a 
        /// <c style="color:DarkRed;"><see cref="Thread"/></c> is started if one doesn't exist.
        /// </remarks>
        /// <returns>
        /// <see langword="true"/> if successful, else <see langword="false"/>
        /// </returns>
        /// @note Instead of calling @c Open() and configuring a @ref FAST.UdpConnection yourself, 
        /// @ref FAST.UdpConnectionLoader and @ref FAST.UdpConnectionSettings should be used to open 
        /// a UDP connection with settings from file during the application load screen.
        public bool Open()
        {
            bool isConnected = false;
            remoteIPEndPoint = new IPEndPoint(IPAddress.Parse(remoteIpAddress), remotePort);

            try {
                udpReceiver = new(localReceivePort);

                if (thread == null) {
                    StartThread();
                }

                udpSender = new(localSendPort);
                udpSender.EnableBroadcast = isEnableBroadcast;

                byte[] data = Encoding.UTF8.GetBytes("Startup");
                Send(data);

                isConnected = true;
                Print($"UDP connection opened!");
            }
            catch (Exception exception) {
                Debug.Log("ERROR\t" + $"{id}: Failed to open UDP connection\n{exception.Message}\n");
            }
            return isConnected;
        }

        /// <summary>
        /// Sends a datagram to <see cref="FAST.UdpConnection.remoteIpAddress"/> on 
        /// <see cref="FAST.UdpConnection.remotePort"/>.
        /// </summary>
        /// <param name="data">The datagram to send.</param>
        public void Send(byte[] data)
        {
            if (udpSender == null) {
                return;
            }
            try {
                udpSender.Send(data, data.Length, remoteIPEndPoint);
                Print($"Sent {data.Length} bytes of data");
            }
            catch (ObjectDisposedException) { }
            catch (Exception exception) {
                Debug.Log("ERROR\t" + $"{id}: Failed to send UDP data\n{exception.Message}\n");
            }
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

                Print("UDP thread started!");
            }
            catch (Exception exception) {
                Debug.Log("ERROR\t" + $"{id}: Failed to start UDP thread\n{exception.Message}\n");
            }
        }

        private void RunThread()
        {
            IPEndPoint receiveEndPoint = new(IPAddress.Any, 0);
            while (isThreadRunning) {
                if (udpReceiver == null) {
                    break;
                }
                try {
                    var datagram = udpReceiver.Receive(ref receiveEndPoint);
                    lock (dataLock) {
                        if (isOverwrite) {
                            data.Clear();
                        }
                        data.Add(datagram);
                    }

                    if (onDataReceivedEvent != null) {
                        lock (queueLock) {
                            if (isOverwrite) {
                                dataQueue.Clear();
                            }
                            dataQueue.Enqueue(datagram);
                        }
                    }
                }
                catch (ObjectDisposedException) { }
                catch (Exception exception) {
                    Debug.Log("ERROR\t" + $"{id}: Failed to receive UDP data\n{exception.Message}\n");
                }
            }

            Print("UDP thread completed!");
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
            if (udpReceiver != null) {
                try {
                    udpReceiver.Close();
                    udpReceiver = null;
                }
                catch (Exception exception) {
                    Debug.Log("ERROR\t" + $"{id}: Failed to close UDP receiver\n{exception.Message}\n");
                }
            }

            if (udpSender != null) {
                try {
                    udpSender.Close();
                    udpSender = null;
                }
                catch (Exception exception) {
                    Debug.Log("ERROR\t" + $"{id}: Failed to close UDP sender\n{exception.Message}\n");
                }
            }

            Print("UDP connection closed!");
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
                    Debug.Log("ERROR\t" + $"{id}: Failed to stop UDP thread {id}\n{exception.Message}\n");
                }
            }

            Print("UDP thread stopped!");
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
