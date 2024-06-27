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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace FAST
{
    /// <summary>
    /// Verifies and opens a web server connection on startup.
    /// </summary>
    public class WebRequestLoader : StartupLoader
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
        /// A unique name to identify this <see cref="FAST.WebRequest"/>.
        /// </summary>
        [SerializeField]
        private string id;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The index of the <see cref="FAST.WebRequestSettings"/> to use in the 
        /// <see cref="FAST.BaseSettings.webRequestSettings"/>.
        /// </summary>
        /// <remarks>
        /// Set to <c>-1</c> if you aren't loading settings from 
        /// <see cref="FAST.BaseSettings.webRequestSettings"/> and are just using Inspector values.
        /// </remarks>
        [SerializeField]
        private int settingsIndex;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
        /// The target URI for the <see cref="FAST.WebRequest"/> to communicate with.
        /// </summary>
        [SerializeField]
        private string uri;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The number of times to try connecting to the web server before failing.
        /// </summary>
        [SerializeField]
        private int maxAttempts = 3;
        
        public WebRequest webRequest;

        protected override IEnumerator ExecuteLoad()
        {
            WebRequestSettings settings = new() {
                uri = uri,
                id = id 
            };
            
            if (settingsIndex >= 0 && Application.settings.webRequestSettings.Count > settingsIndex) {
                settings = Application.settings.webRequestSettings[settingsIndex];
                uri = settings.uri;
                id = settings.id;
            }

            webRequest = new WebRequest {
                uri = WebRequest.Get(uri).uri,
                id = id
            };

            loadingTitle = $"Loading internet or network resource . . .";
            Debug.Log($"\n{loadingTitle}");

            bool isConnected = false;
            for (int i = 0; i < maxAttempts; i++) {
                loadingMessage = $"{settings.id}: {settings.uri}" + 
                    $"\nAttempt {i + 1} of {maxAttempts}";
                Debug.Log($"{loadingMessage}");
                loadingEvent.Invoke(loadingTitle, loadingMessage);

                yield return webRequest.SendWebRequest();
                yield return new WaitForSecondsRealtime(loadingMessageDuration);

                if (webRequest.error == null) {
                    isConnected = true;
                    break;
                }
                else {
                    Debug.Log(webRequest.error);
                }
            }

            if (!isConnected) {
                errorTitle = "Connection failed!";
                if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                    errorMessage = $"Cannot connect to the internet or network resource {settings.id} at {settings.uri}.";
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
