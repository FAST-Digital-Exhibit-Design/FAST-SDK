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

namespace FAST
{
    /// <summary>
    /// Verifies the number of displays connected and activates them if more than one is required.
    /// </summary>
    public class DisplayLoader : StartupLoader
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The number of displays required to run the activity.
        /// </summary>
        [SerializeField]
        private int numDisplaysExpected = 1;

        protected override IEnumerator ExecuteLoad()
        {
            DisplaySettings settings = Application.settings.displaySettings;

#if UNITY_EDITOR
            // The Editor can only register 1 display no matter how many game windows are open or monitors are connected.
            numDisplaysExpected = 1;
#else
            numDisplaysExpected = settings.numberOfDisplays;
#endif
            // Check the number of displays connected
            int numDisplaysConnected = Display.displays.Length;
            loadingTitle = "Verifying displays . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = $"Displays connected: {numDisplaysConnected}";
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (numDisplaysConnected < numDisplaysExpected) {
                errorTitle = "Displays not detected!";
                if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                    errorMessage = $"Cannot detect all the displays required. " +
                        $"Only {numDisplaysConnected} of {numDisplaysExpected} displays are connected.";
                } else {
                    errorMessage = settings.replacementErrorMessage;
                }
                errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Activate additional displays if there is more than 1
            for (int i= 1; i < Display.displays.Length; i++) {
                Display.displays[i].Activate();
            }
            loadingMessage = $"Activated {numDisplaysConnected} displays";
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            successEvent.Invoke();
        }
    }
}
