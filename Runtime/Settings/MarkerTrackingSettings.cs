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
using FAST;

namespace FAST
{
    /// <summary>
    /// A data structure to store <see cref="FAST.MarkerTrackingSystem"/> settings.
    /// </summary>
    [System.Serializable]
    public class MarkerTrackingSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The name of the <see cref="FAST.UdpConnection"/> with marker tracking data.
        /// </summary>
        [XmlElement(ElementName = "UdpConnectionId")]
        public string udpConnectionId;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The maximum number of markers that this marker tracking system can track.
        /// </summary>
        /// <remarks>
        /// This value can be found in the Computer Vision app.
        /// </remarks>
        [XmlElement(ElementName = "MaximumNumberOfMarkers")]
        public int maxNumMarkers;
    }

    [System.Serializable]
    public class MarkerToolData
    {
        [XmlAttribute]
        public string name = "Tool";
        [XmlElement(ElementName = "MarkerId")]
        public int[] markerIds = { 0 };
    }

    [System.Serializable]
    public class MarkerObjectData : MarkerToolData
    {
        [XmlAttribute]
        public int value = 0;
    }
}
