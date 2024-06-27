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
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// Verifies and opens a UDP connection on a startup.
    /// </summary>
    [RequireComponent(typeof(UdpConnection))]
    public class UdpConnectionLoader : StartupLoader
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The index of the <see cref="FAST.UdpConnectionSettings"/> to use in the 
        /// <see cref="FAST.BaseSettings.udpConnectionSettings"/>.
        /// </summary>
        /// <remarks>
        /// Set to <c>-1</c> if you aren't loading settings from 
        /// <see cref="FAST.BaseSettings.udpConnectionSettings"/> and are just using Inspector values.
        /// </remarks>
        [SerializeField]
        private int settingsIndex;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The number of times to try opening the UDP connection before failing.
        /// </summary>
        [SerializeField]
        private int maxAttempts = 3;

        private UdpConnection udpConnection;

        protected override IEnumerator ExecuteLoad()
        {
            udpConnection = GetComponent<UdpConnection>();

            UdpConnectionSettings settings = new() {
                id = udpConnection.id,
                localReceivePort = udpConnection.localReceivePort,
                localSendPort = udpConnection.localSendPort,
                remoteIpAddress = udpConnection.remoteIpAddress,
                remotePort = udpConnection.remotePort
            };

            if (settingsIndex >= 0 && Application.settings.udpConnectionSettings.Count > settingsIndex) {
                settings = Application.settings.udpConnectionSettings[settingsIndex];
                udpConnection.id = settings.id;
                udpConnection.localReceivePort = settings.localReceivePort;
                udpConnection.localSendPort = settings.localSendPort;
                udpConnection.remoteIpAddress = settings.remoteIpAddress;
                udpConnection.remotePort = settings.remotePort;
            }

            loadingTitle = $"Loading UDP network connection . . .";
            Debug.Log($"\n{loadingTitle}");

            // exceptions for invalid address or port
            bool isConnected = false;
            for (int i = 0; i < maxAttempts; i++) {
                loadingMessage = $"{udpConnection.id} from localhost:{udpConnection.localReceivePort} " +
                    $"to {udpConnection.remoteIpAddress}:{udpConnection.remotePort}" +
                    $"\nAttempt {i + 1} of {maxAttempts}";
                Debug.Log($"{loadingMessage}");
                loadingEvent.Invoke(loadingTitle, loadingMessage);

                yield return new WaitForSecondsRealtime(loadingMessageDuration);

                if (udpConnection.Open()) {
                    isConnected = true;
                    break;
                }
            }

            if (!isConnected) {
                errorTitle = "Connection failed!";
                if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                    errorMessage = $"Cannot configure the UDP network connection {udpConnection.id} " +
                        $"from localhost:{udpConnection.localReceivePort} to {udpConnection.remoteIpAddress}:{udpConnection.remotePort}.";
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
    }
}
