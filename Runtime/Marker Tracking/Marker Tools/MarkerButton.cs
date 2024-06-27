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
    /// A predefined tool for a button.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a physical button with 2 states, <c>up</c> and <c>pressed down</c>. One marker is placed 
    /// on the button housing as reference for the button position and must always be visible. A second 
    /// marker is placed on a mechanism that moves down button the button is pressed and up when released. 
    /// The second marker is only visible in the up state and occluded in the down state.
    /// </para>
    /// <para>
    /// Use the <c>Marker Button Prefab</c> to instantiate this tool with default 
    /// attributes and assignments.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    public class MarkerButton : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies the state of the button as either <c>up</c> and <c>pressed down</c>.
        /// </summary>
        public string value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the reference marker.
        /// </summary>
        public MarkerData buttonReferenceMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the button press marker.
        /// </summary>
        public MarkerData buttonPressMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the reference marker.
        /// </summary>
        [SerializeField]
        private Image buttonReferencePointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the reference marker.
        /// </summary>
        [SerializeField]
        private TMP_Text buttonReferenceIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the button press marker.
        /// </summary>
        [SerializeField]
        private Image buttonPressImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the button press marker.
        /// </summary>
        [SerializeField]
        private TMP_Text buttonPressIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// <see cref="FAST.MarkerButton.value"/>.
        /// </summary>
        [SerializeField]
        private TMP_Text buttonValueText;


        void Update()
        {
            MarkerData markerData;

            markerData = trackingSystem.markerDataLUT[markerIds[0]];
            bool isButtonReferenceUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isButtonReferenceUpdated) {
                buttonReferenceMarker = markerData;
            }

            markerData = trackingSystem.markerDataLUT[markerIds[1]];
            bool isButtonPressUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isButtonPressUpdated) {
                buttonPressMarker = markerData;
            }
            buttonPressImage.gameObject.SetActive(isButtonPressUpdated);

            value = (isButtonPressUpdated) ? "up" : "pressed down";

            isTracked = isButtonReferenceUpdated;

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
            buttonReferencePointImage.rectTransform.localPosition = 
                new(buttonReferenceMarker.x * trackingSystem.Width, -buttonReferenceMarker.y * trackingSystem.Height);
            buttonReferencePointImage.rectTransform.localEulerAngles = new(0f, 0f, buttonReferenceMarker.angle);
            buttonReferenceIdText.text = markerIds[0].ToString();

            buttonPressImage.rectTransform.localPosition = 
                new(buttonPressMarker.x * trackingSystem.Width, -buttonPressMarker.y * trackingSystem.Height);
            buttonPressImage.rectTransform.localEulerAngles = new(0f, 0f, buttonPressMarker.angle);
            buttonPressIdText.text = markerIds[1].ToString();

            buttonValueText.text = value;
        }
    }
}
