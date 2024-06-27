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
using UnityEngine.UI;
using TMPro;

namespace FAST
{
    /// <summary>
    /// A predefined tool for a dial control.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a physical dial with a knob that rotates 360 degrees. One marker is placed 
    /// on the dial housing as reference for the knob rotation. A second marker is placed on 
    /// a gear connected to the knob and rotates as the knob rotates. The angle of rotation 
    /// is measured in degrees as the difference between the orientation of the two markers. 
    /// Both markers must always be visible.
    /// </para>
    /// <para>
    /// Use the <c>Marker Dial Prefab</c> to instantiate this tool with default 
    /// attributes and assignments.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    public class MarkerDial : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The dial rotation amount as a <see langword="string"/>.
        /// </summary>
        // Why isn't this just the angle as a float?
        public string value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the reference marker.
        /// </summary>
        public MarkerData dialReferenceMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the rotating knob marker.
        /// </summary>
        public MarkerData dialRotationMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the reference marker.
        /// </summary>
        [SerializeField]
        private Image dialReferencePointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the reference marker.
        /// </summary>
        [SerializeField]
        private TMP_Text dialReferenceIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the rotating marker.
        /// </summary>
        [SerializeField]
        private Image dialRotationPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the rotating knob marker.
        /// </summary>
        [SerializeField]
        private TMP_Text dialRotationIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// <see cref="FAST.MarkerSlider.value"/>.
        /// </summary>
        [SerializeField]
        private TMP_Text dialValueText;

        /// <summary>
        /// The dial rotation amount in the range of [0, 360] degrees.
        /// </summary>
        public float angle = 0;

        void Update()
        {
            MarkerData markerData;

            markerData = trackingSystem.markerDataLUT[markerIds[0]];
            bool isDialReferenceUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isDialReferenceUpdated) {
                dialReferenceMarker = markerData;
            }

            markerData = trackingSystem.markerDataLUT[markerIds[1]];
            bool isDialRotationUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isDialRotationUpdated) {
                dialRotationMarker = markerData;
            }

            isTracked = isDialReferenceUpdated && isDialRotationUpdated;

            float newAngle = ((dialReferenceMarker.angle - dialRotationMarker.angle) % 360f);
            if (newAngle < 0) {
                newAngle = 360f + newAngle;
            }
            angle = Mathf.LerpAngle(angle, newAngle, 0.1f) % 360f;
            if (angle < 0) {
                angle = 360f + angle;
            }

            value = angle.ToString("0.000");

            if (isDrawTool) {
                canvasGroup.alpha = isTracked ? 1 : 0;
                DrawTool();
            }
            else {
                canvasGroup.alpha = 0;
            }
        }

        protected override void DrawTool()
        {
            dialReferencePointImage.rectTransform.localPosition = 
                new(dialReferenceMarker.x * trackingSystem.Width, -dialReferenceMarker.y * trackingSystem.Height);
            dialReferencePointImage.rectTransform.localEulerAngles = new(0f, 0f, dialReferenceMarker.angle);
            dialReferenceIdText.text = markerIds[0].ToString();

            dialRotationPointImage.rectTransform.localPosition = 
                new(dialRotationMarker.x * trackingSystem.Width, -dialRotationMarker.y * trackingSystem.Height);
            dialRotationPointImage.rectTransform.localEulerAngles = new(0f, 0f, dialRotationMarker.angle);
            dialRotationIdText.text = markerIds[1].ToString();

            dialValueText.text = value;
        }
    }
}
