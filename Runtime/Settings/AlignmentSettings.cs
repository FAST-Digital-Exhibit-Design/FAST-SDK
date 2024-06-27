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
using System.Xml.Serialization;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// A data structure to format the XML for <c style="color:DarkRed;"><see cref="Vector3"/></c> 
    /// data, like the position of an <see cref="FAST.AlignmentTransform"/>.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Vector3.html">
    /// UnityEngine.Vector2</a>
    [System.Serializable]
    public struct XmlVector3
    {
        /// <summary>
        /// X-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float x;
        /// <summary>
        /// Y-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float y;
        /// <summary>
        /// Z-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float z;
    }

    /// <summary>
    /// A data structure to format the XML for <c style="color:DarkRed;"><see cref="Vector2"/></c> 
    /// data, like the size of an <see cref="FAST.AlignmentRectTransform"/>.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Vector2.html">
    /// UnityEngine.Vector2</a>
    [System.Serializable]
    public struct XmlVector2
    {
        /// <summary>
        /// X-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float x;
        /// <summary>
        /// Y-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float y;
    }

    /// <summary>
    /// A data structure to format the XML for the Z-axis data, 
    /// like the rotation of an <see cref="FAST.AlignmentTransform"/>.
    /// </summary>
    [System.Serializable]
    public struct XmlFloatZ
    {
        /// <summary>
        /// Z-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float z;
    }

    /// <summary>
    /// A data structure to format the XML for the uniform XYZ-axis data, 
    /// like the scale of an <see cref="FAST.AlignmentTransform"/>.
    /// </summary>
    [System.Serializable]
    public struct XmlFloatXYZ
    {
        /// <summary>
        /// Uniform XYZ-axis value, formatted as an XML attribute.
        /// </summary>
        [XmlAttribute]
        public float xyz;
    }

    /// <summary>
    /// A data structure to store <see cref="FAST.AlignmentTransform"/> settings.
    /// </summary>
    [System.Serializable]
    public class AlignmentTransformSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The name of the <see cref="FAST.AlignmentTransform"/>.
        /// </summary>
        [XmlAttribute]
        public string name;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The position of the <see cref="FAST.AlignmentTransform"/>.
        /// </summary>
        public XmlVector3 position;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The Z-axis rotation of the <see cref="FAST.AlignmentTransform"/>.
        /// </summary>
        public XmlFloatZ rotation;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The scale of the <see cref="FAST.AlignmentTransform"/>.
        /// </summary>
        public XmlFloatXYZ scale;
    }

    /// <summary>
    /// A data structure to store <see cref="FAST.AlignmentRectTransform"/> settings.
    /// </summary>
    [System.Serializable]
    public class AlignmentRectTransformSettings : AlignmentTransformSettings
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Code</b><br/>
        /// The size of the <see cref="FAST.AlignmentRectTransform"/>.
        /// </summary>
        public XmlVector2 size;
    }
}
