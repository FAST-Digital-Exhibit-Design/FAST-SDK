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
    /// A base class timer that either invokes a <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
    /// when time elapses or resets when an interrupt happens.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
    /// UnityEngine.Events.UnityEvent</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Coroutine.html">
    /// UnityEngine.Coroutine</a>
    public abstract class EventTimer : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when the timer has elapsed.
        /// </summary>
        [Tooltip("Specify the functions that should be called when timer has elapsed.")]
        public UnityEvent OnTimerElapsed;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the <see cref="FAST.EventTimer.OnTimerElapsed"/> 
        /// event should be invoked during <see cref="FAST.EventTimer.Start()"/>.
        /// </summary>
        [SerializeField]
        [Tooltip("Enable this to trigger the \"On Timer Elapsed ()\" event on startup.")]
        protected bool triggerOnStart = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The duration of the timer in seconds.
        /// </summary>
        [SerializeField]
        [Tooltip("Specify duration of the timer in seconds.")]
        protected float timerSeconds = 120f;

        /// <summary>
        /// Invokes the <see cref="FAST.EventTimer.OnTimerElapsed"/> event if 
        /// <see cref="FAST.EventTimer.triggerOnStart"/> is set to <see langword="true"/>.
        /// </summary>
        protected virtual void Start()
        {
            if (triggerOnStart) {
                OnTimerElapsed.Invoke();
            }
        }

        /// <summary>
        /// Calls <see cref="FAST.EventTimer.CheckForInterrupt()"/> to determine if 
        /// the timer will reset.
        /// </summary>
        protected virtual void Update()
        {
            if (CheckForInterrupt()) {
                StopAllCoroutines();
                StartCoroutine(Timer());
            }
        }

        /// <summary>
        /// Checks for an interrupt condition to determine if the timer will reset.
        /// </summary>
        /// <returns><see langword="true"/> if the timer will reset, 
        /// <see langword="false"/> if the timer will keep going or not reset.</returns>
        protected abstract bool CheckForInterrupt();

        /// <summary>
        /// The <c style="color:DarkRed;"><see cref="Coroutine"/></c> function 
        /// that waits for the timer to elapse and then invokes the 
        /// <see cref="FAST.EventTimer.OnTimerElapsed"/> event.
        /// </summary>
        protected virtual IEnumerator Timer()
        {
            yield return new WaitForSecondsRealtime(timerSeconds);
            OnTimerElapsed.Invoke();
        }
    }
}
