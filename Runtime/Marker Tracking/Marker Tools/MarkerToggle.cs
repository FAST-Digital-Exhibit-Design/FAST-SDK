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
    /// A predefined tool for a 2-state toggle.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The toggle is a physical switch with 2 states, <c>A</c> and <c>B</c>. A marker is placed 
    /// in each state, such that only 1 mark is visible and tracked at a time.
    /// </para>
    /// <para>
    /// Use the <c>Marker Toggle Prefab</c> to instantiate this tool with default 
    /// attributes and assignments.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    public class MarkerToggle : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies the state of the toggle that is currently being tracked.
        /// </summary>
        public string value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The identifier to use for when state <c>A</c> is being tracked.
        /// </summary>
        [SerializeField]
        private string optionA;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The identifier to use for when state <c>B</c> is being tracked.
        /// </summary>
        [SerializeField]
        private string optionB;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the marker used by state <c>A</c>.
        /// </summary>
        public MarkerData optionAMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the marker used by state <c>B</c>.
        /// </summary>
        public MarkerData optionBMarker;


        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the marker used by state <c>A</c>.
        /// </summary>
        [SerializeField]
        private Image optionAPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the marker used by state <c>A</c>.
        /// </summary>
        [SerializeField]
        private TMP_Text optionAIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// value of state <c>A</c>.
        /// </summary>
        [SerializeField]
        private TMP_Text optionAValueText;


        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the marker used by state <c>B</c>.
        /// </summary>
        [SerializeField]
        private Image optionBPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the marker used by state <c>B</c>.
        /// </summary>
        [SerializeField]
        private TMP_Text optionBIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// value of state <c>B</c>.
        /// </summary>
        [SerializeField]
        private TMP_Text optionBValueText;

        void Update()
        {
            MarkerData markerData;

            markerData = trackingSystem.markerDataLUT[markerIds[0]];
            bool isOptionAUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isOptionAUpdated) {
                optionAMarker = markerData;
                value = optionA;
            }
            optionAPointImage.gameObject.SetActive(isOptionAUpdated);

            markerData = trackingSystem.markerDataLUT[markerIds[1]];
            bool isOptionBUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isOptionBUpdated) {
                optionBMarker = markerData;
                value = optionB;
            }
            optionBPointImage.gameObject.SetActive(isOptionBUpdated);

            value = (isOptionAUpdated && isOptionBUpdated) ? "A and B" : value;

            isTracked = isOptionAUpdated || isOptionBUpdated;

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
            optionAPointImage.rectTransform.localPosition =
                new(optionAMarker.x * trackingSystem.Width, -optionAMarker.y * trackingSystem.Height);
            optionAPointImage.rectTransform.localEulerAngles = new(0f, 0f, optionAMarker.angle);
            optionAIdText.text = markerIds[0].ToString();
            optionAValueText.text = optionA;

            optionBPointImage.rectTransform.localPosition =
                new(optionBMarker.x * trackingSystem.Width, -optionBMarker.y * trackingSystem.Height);
            optionBPointImage.rectTransform.localEulerAngles = new(0f, 0f, optionBMarker.angle);
            optionBIdText.text = markerIds[1].ToString();
            optionBValueText.text = optionB;
        }
    }
}
