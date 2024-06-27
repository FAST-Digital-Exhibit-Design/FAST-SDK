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
    /// A data structure to store <see cref="FAST.DisplayLoader"/> settings.
    /// </summary>
    [System.Serializable]
    public class DisplaySettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The number of displays required to run the activity.
        /// </summary>
        public int numberOfDisplays = 1;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A message that will be added before the <see cref="FAST.LoadingProgress"/> 
        /// error message at runtime.
        /// </summary>
        public string prefixErrorMessage = "";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A message that will replace the <see cref="FAST.LoadingProgress"/> 
        /// error message at runtime.
        /// </summary>
        public string replacementErrorMessage = "";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A message that will be added after the <see cref="FAST.LoadingProgress"/> 
        /// error message at runtime.
        /// </summary>
        public string suffixErrorMessage = "";
    }
}
