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
    /// A timer that invokes a <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
    /// when time elapses or resets when there is interaction input.
    /// </summary>
    /// <remarks>
    /// This can be used to determine if an activity is idle or being used by a visitor.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
    /// UnityEngine.Events.UnityEvent</a>
    public class IdleTimer : EventTimer
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if mouse button or key presses 
        /// should reset the timer.
        /// </summary>
        [Space(10)]
        [SerializeField]
        private bool checkKeyInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if touches should reset the timer.
        /// </summary>
        [SerializeField]
        private bool checkTouchInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if serial communication should reset the timer.
        /// </summary>
        [SerializeField]
        private bool checkSerialInput = true;

        private bool isSerialInput = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if UDP communication should reset the timer.
        /// </summary>
        [SerializeField]
        private bool checkUdpInput = false;

        private bool isUdpInput = false;

        /// <summary>
        /// Registers with <see cref="FAST.UdpConnection"/> and <see cref="FAST.SerialConnection"/> for 
        /// callbacks when data is recieved, then invokes the <see cref="FAST.EventTimer.OnTimerElapsed"/> event 
        /// if <see cref="FAST.EventTimer.triggerOnStart"/> is set to <see langword="true"/>.
        /// </summary>
        protected override void Start()
        {
            if (checkSerialInput) {
                foreach (var connection in Application.serialConnections) {
                    connection.onDataReceivedEvent += OnSerialInput;
                }
            }

            if (checkUdpInput) {
                foreach (var connection in Application.udpConnections) {
                    connection.onDataReceivedEvent += OnUdpInput;
                }
            }

            base.Start();
        }

        /// <summary>
        /// Checks for any interaction input to determine if the timer will reset.
        /// </summary>
        /// <returns><see langword="true"/> if there is input and the timer will reset, 
        /// <see langword="false"/> if not.</returns>
        protected override bool CheckForInterrupt()
        {
            bool isAnyInput = false;

            if (checkKeyInput) {
                isAnyInput |= IsKeyInput();
            }
            if  (checkTouchInput) {
                isAnyInput |= IsTouchInput();
            }
            if (checkSerialInput) {
                isAnyInput |= IsSerialInput();
            }
            if (checkUdpInput) {
                isAnyInput |= IsUdpInput();
            }

            return isAnyInput;
        }

        private bool IsKeyInput()
        {
            return Input.anyKeyDown;
        }

        private bool IsTouchInput()
        {
            return Input.touchCount > 0;
        }

        private bool IsSerialInput()
        {
            if (isSerialInput) {
                isSerialInput = false;
                return true;
            }

            return false;
        }
        private bool IsUdpInput()
        {
            if (isUdpInput) {
                isUdpInput = false;
                return true;
            }

            return false;
        }

        private void OnSerialInput(string data)
        {
            isSerialInput = true;
        }

        private void OnUdpInput(byte[] data)
        {
            isUdpInput = true;
        }
    }
}
