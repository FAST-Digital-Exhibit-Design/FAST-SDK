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
    /// Provides the same functionality as <c style="color:DarkRed;"><see cref="UnityWebRequest"/></c>, 
    /// but adds a string attribute to identify this instance.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Networking.UnityWebRequest.html">
    /// UnityEngine.Networking.UnityWebRequest</a>
    public class WebRequest : UnityWebRequest
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// Identifies this instance.
        /// </summary>
        /// <remarks>
        /// Loaded from <see cref="FAST.WebRequestSettings"/> at runtime.
        /// </remarks>
        public string id;
    }
}
