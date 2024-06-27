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
using System.Collections;
using System.Collections.Generic;
using System.IO.Ports;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// Verifies and opens a serial port connection on startup.
    /// </summary>
    [RequireComponent(typeof(SerialConnection))]
    public class SerialConnectionLoader : StartupLoader
    {
        /// <summary>
        /// Indicates what search method should be used if connecting to the specified COM port fails.
        /// </summary>
        enum SearchMethod {
            /// <summary>
            /// Other ports won't be searched.
            /// </summary>
            None,

            /// <summary>
            /// The search will start on COM3 and increment by 1 until a connection is made 
            /// or COM256 is reached.
            /// </summary>
            FirstAvailable,

            /// <summary>
            /// A lookup table of serial device name and port number will be made. Then the table 
            /// will be searched for the first device name that matches the specified search name.
            /// </summary>
            FirstNamed
        }

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The index of the <see cref="FAST.SerialConnectionSettings"/> to use in the 
        /// <see cref="FAST.BaseSettings.serialConnectionSettings"/>.
        /// </summary>
        /// <remarks>
        /// Set to <c>-1</c> if you aren't loading settings from 
        /// <see cref="FAST.BaseSettings.serialConnectionSettings"/> and are just using Inspector values.
        /// </remarks>
        [SerializeField]
        private int settingsIndex;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The number of times the specified COM port will be tried before searching for another connected COM port. 
        /// Also, the number of times that the COM port found in the search will be tried before failing.
        /// </summary>
        [SerializeField]
        private int maxAttempts = 3;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The search method that will be used if connecting to the specified COM port fails.
        /// </summary>
        [SerializeField]
        private SearchMethod searchMethod;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The name of the serial device to search for if connecting to the specified COM port fails.
        /// </summary>
        /// <remarks>
        /// This only applies to <see cref="FAST.SerialConnectionLoader.SearchMethod.FirstNamed"/>.
        /// </remarks>
        [SerializeField, NaughtyAttributes.ShowIf("searchMethod", SearchMethod.FirstNamed)]
        private string searchName;

        private const int kMinPortNumber = 3;
        private const int kMaxPortNumber = 256;

        private const float kRetryDelay = 5f;
        private const float kSearchDelay = 0.05f;

        private SerialConnection serialConnection;
        private bool isConnected;
        private SerialConnectionSettings settings;

        protected override IEnumerator ExecuteLoad()
        {
            serialConnection = GetComponent<SerialConnection>();

            settings = new() {
                comPort = serialConnection.comPort,
                baudRate = (int)serialConnection.baudRate,
                id = serialConnection.id
            };

            if (settingsIndex >= 0 && Application.settings.serialConnectionSettings.Count > settingsIndex) {
                settings = Application.settings.serialConnectionSettings[settingsIndex];
                serialConnection.id = settings.id;
                serialConnection.comPort = settings.comPort;
                bool isBaudRateDefined = Enum.IsDefined(typeof(SerialConnection.BaudRates), settings.baudRate);
                serialConnection.baudRate = (SerialConnection.BaudRates) (isBaudRateDefined ? settings.baudRate : 9600);
            }

            loadingTitle = $"Loading serial device . . .";
            Debug.Log($"\n{loadingTitle}");

            // Try the COM port specified in the settings
            isConnected = false;
            for (int i = 0; i < maxAttempts; i++) {
                loadingMessage = $"{serialConnection.id} on COM{serialConnection.comPort} " +
                    $"with baud rate {(int)serialConnection.baudRate}" +
                    $"\nAttempt {i + 1} of {maxAttempts}";
                loadingEvent.Invoke(loadingTitle, loadingMessage);

                yield return new WaitForSecondsRealtime(Mathf.Max(i > 0 ? kRetryDelay : 0f, loadingMessageDuration));

                if (serialConnection.Open()) {
                    isConnected = true;
                    break;
                }
            }

            // If the COM port specified in the settings didn't work, try searching for the correct one
            if (!isConnected) {
                if (searchMethod.Equals(SearchMethod.FirstAvailable)) {
                    yield return FindFirstAvailablePort();
                }
                else if (searchMethod.Equals(SearchMethod.FirstNamed)) {
                    yield return FindFirstNamedPort();
                }
            }

            // If searching for the COM port didn't work, just give up and show an error
            if (!isConnected) {
                errorTitle = "Connection failed!";
                if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                    errorMessage = $"Cannot connect to the serial device {settings.id} on COM{settings.comPort} " +
                        $"with baud rate {settings.baudRate}.";
                    if (!searchMethod.Equals(SearchMethod.None)) {
                        errorMessage += "\nAll available ports were searched, and no connection was made.";
                    }
                }
                else {
                    errorMessage = settings.replacementErrorMessage;
                }
                errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }

            successEvent.Invoke();
        }

        private IEnumerator FindFirstAvailablePort()
        {
            for (int i = 0; i < maxAttempts; i++) {
                yield return new WaitForSecondsRealtime(kRetryDelay);

                if (isConnected) {
                    break;
                }

                Debug.Log($"Searching for first available port" +
                    $"\nAttempt {i + 1} of {maxAttempts}");

                for (int n = kMinPortNumber; n <= kMaxPortNumber; n++) {
                    serialConnection.comPort = n;
                    loadingMessage = $"Searching for first available port on COM{serialConnection.comPort}" +
                        $"\nAttempt {i + 1} of {maxAttempts}";
                    loadingEvent.Invoke(loadingTitle, loadingMessage);
                    yield return new WaitForSecondsRealtime(kSearchDelay);

                    if (serialConnection.Open()) {
                        settings.comPort = serialConnection.comPort;
                        Application.WriteSettings();
                        isConnected = true;
                        break;
                    }
                }
            }

            Debug.Log($"{serialConnection.id} cannot be found");
            Debug.Log("Available serial ports:\n" + string.Join(Environment.NewLine, SerialPort.GetPortNames()));
        }
        private IEnumerator FindFirstNamedPort()
        {
            Dictionary<int, string> portAssignments = new Dictionary<int, string>();
            // Return the first port found with a name that matches a
            // specified substring.
            int GetFirstPortMatchingName(string contains)
            {
                foreach (KeyValuePair<int, string> entry in portAssignments) {
                    if (entry.Value.Contains(contains)) {
                        return entry.Key;
                    }
                }
                return -1;
            }

            for (int i = 0; i < maxAttempts; i++) {
                yield return new WaitForSecondsRealtime(kRetryDelay);

                if (isConnected) {
                    break;
                }

                // Remove existing port entries
                portAssignments.Clear();

                // Run PowerShell process with Get-WMIObject Win32_SerialPort
                // Request DeviceID (port) and Description (device name)
                using (System.Diagnostics.Process process = new()) {
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.FileName = "powershell.exe";
                    process.StartInfo.Arguments = "Get-WMIObject Win32_SerialPort | Select-Object DeviceID,Description";
                    process.Start();

                    // Read each line of the output
                    string lineOut;
                    while ((lineOut = process.StandardOutput.ReadLine()) != null) {
                        // If it contains "COM" parse out the int for the port number
                        // and save the remainder of the line as the description
                        if (lineOut.Contains("COM")) {
                            int firstSpacePos = lineOut.IndexOf(' ');
                            string port = lineOut.Substring(0, firstSpacePos).Trim();
                            string desc = lineOut.Substring(firstSpacePos).Trim();
                            // Add to the port list
                            if (int.TryParse(port.Substring(3), out int portNum)) {
                                portAssignments.Add(portNum, desc);
                            }
                        }
                    }
                    process.CloseMainWindow();
                    process.Close();
                }

                loadingMessage = $"Searching for {serialConnection.id} by name" +
                    $"\nAttempt {i + 1} of {maxAttempts}";
                Debug.Log($"{loadingMessage}");
                loadingEvent.Invoke(loadingTitle, loadingMessage);
                yield return new WaitForSecondsRealtime(kSearchDelay);

                int comPort = GetFirstPortMatchingName(searchName);
                if (comPort > 0) {
                    serialConnection.comPort = comPort;

                    if (serialConnection.Open()) {
                        settings.comPort = serialConnection.comPort;
                        Application.WriteSettings();
                        isConnected = true;
                        break;
                    }
                }
            }

            Debug.Log($"{serialConnection.id} cannot be found");
            Debug.Log("Available serial ports:\n" + string.Join(Environment.NewLine, portAssignments));
        }
    }
}
