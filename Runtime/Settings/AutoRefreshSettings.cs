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
using UnityEngine.Events;

namespace FAST
{
    /// <summary>
    /// Automatically refresh the activity settings on an interval and trigger a 
    /// <c style="color:DarkRed;"><see cref="UnityEvent"/></c> on every refresh.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
    /// UnityEngine.Events.UnityEvent</a>
    public class AutoRefreshSettings : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Set to <see langword="true"/> to automatically refresh the activity settings.
        /// </summary>
        [Tooltip("Specify if the settings should be automatically refreshed.")]
        public bool isAutoRefresh;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when the activity settings are refreshed.
        /// </summary>
        [Tooltip("Specify the functions that should be called when the settings are refreshed.")]
        public UnityEvent OnRefreshSettings;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The refresh rate in seconds.
        /// </summary>
        [SerializeField]
        [Tooltip("Specify the refresh rate  in seconds.")]
        protected float refreshRateSeconds = 0.25f;

        IEnumerator Start()
        {
            while (true) {
                if (isAutoRefresh) {
                    Application.ReadSettings();
                    OnRefreshSettings.Invoke();
                }

                yield return new WaitForSecondsRealtime(refreshRateSeconds);
            }
        }
    }
}
