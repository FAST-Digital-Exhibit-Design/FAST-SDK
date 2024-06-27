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
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// A data structure to store <see cref="FAST.WebRequest"/> and 
    /// <see cref="FAST.WebRequestLoader"/> settings.
    /// </summary>
    [System.Serializable]
    public class WebRequestSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// A unique name to identify this <see cref="FAST.WebRequest"/>.
        /// </summary>
        [XmlAttribute]
        public string id = "";

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The target URI for the <see cref="FAST.WebRequest"/> to communicate with.
        /// </summary>
        public string uri = "";

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
