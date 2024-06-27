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
    /// A data structure to store <see cref="FAST.WhiteScreenManager"/> settings.
    /// </summary>
    [System.Serializable]
    public class WhiteScreenSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A message that will be displayed in the center of the screen.
        /// </summary>
        /// <remarks>
        /// For example, <c>"Touch the screen to begin."</c>
        /// </remarks>
        public string onScreenMessage = "";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The amount of time in minutes that the white screen is shown for.
        /// </summary>
        public int durationMinutes = 30;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if touch input should dismiss the white screen.
        /// </summary>
        public bool isTouchInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if mouse input should dismiss the white screen.
        /// </summary>
        public bool isMouseInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if joystick input should dismiss the white screen.
        /// </summary>
        public bool isAxisInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if key and mouse button press input should dismiss the white screen.
        /// </summary>
        public bool isKeypressInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if serial input should dismiss the white screen.
        /// </summary>
        /// <remarks>
		/// This can be helpful if the input is noisy or very sensitive.
		/// </remarks>
        public bool isSerialInput = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// This can be helpful if the input is noisy or very sensitive.
        /// </summary>
        public int serialInputCount = 10;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Set to <see langword="true"/> if UDP input should dismiss the white screen.
        /// </summary>
        public bool isUdpInput = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The number of UDP datagrams that must be recieved to dismiss the white screen.
        /// </summary>
        /// <remarks>
        /// This can be helpful if the input is noisy or very sensitive.
        /// </remarks>
        public int udpInputCount = 1;
    }
}
