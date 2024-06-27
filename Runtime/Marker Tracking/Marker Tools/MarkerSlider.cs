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
    /// A predefined tool for a slider.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a physical slider with a knob that moves along a track. One marker is placed 
    /// at the beginning of the slider track and another marker is placed at the end of the slider track. 
    /// Both markers must be visible at some point to determine the slider track length, but after that only 
    /// one of these reference markers needs to be visible while the slider is in use. A third marker 
    /// is placed on the knob that slides on the track and must also always be visible.
    /// </para>
    /// <para>
    /// Use the <c>Marker Slider Prefab</c> to instantiate this tool with default 
    /// attributes and assignments.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    public class MarkerSlider : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies the slider position as normalized value in the range [0, 1].
        /// </summary>
        public float value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The X-coordinate distance from the reference marker to the knob marker 
        /// when at the beginning of the slider.
        /// </summary>
        public float sliderLength;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the slider start reference marker.
        /// </summary>
        public MarkerData startMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the slider end reference marker.
        /// </summary>
        public MarkerData endMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the knob marker.
        /// </summary>
        public MarkerData knobMarker;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the slider start reference marker.
        /// </summary>
        [SerializeField]
        private Image startPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the slider start reference marker.
        /// </summary>
        [SerializeField]
        private TMP_Text startIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the slider end reference marker.
        /// </summary>
        [SerializeField]
        private Image endPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the slider end reference marker.
        /// </summary>
        [SerializeField]
        private TMP_Text endIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// position of the knob marker.
        /// </summary>
        [SerializeField]
        private Image knobPointImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the ID 
        /// of the knob marker.
        /// </summary>
        [SerializeField]
        private TMP_Text knobIdText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// <see cref="FAST.MarkerSlider.value"/>.
        /// </summary>
        [SerializeField]
        private TMP_Text valueText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/></c> to visualize the 
        /// track the knob slides along.
        /// </summary>
        [SerializeField]
        private Image slideAreaImage;

        void Update()
        {
            MarkerData markerData;

            markerData = trackingSystem.markerDataLUT[markerIds[0]];
            bool isStartUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isStartUpdated) {
                startMarker = markerData;
            }

            markerData = trackingSystem.markerDataLUT[markerIds[1]];
            bool isEndUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isEndUpdated) {
                endMarker = markerData;
            }

            markerData = trackingSystem.markerDataLUT[markerIds[2]];
            bool isKnobUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
            if (isKnobUpdated) {
                knobMarker = markerData;
            }

            Vector2 startPoint = new(startMarker.x, startMarker.y);
            Vector2 endPoint = new(endMarker.x, endMarker.y);
            Vector2 knobPoint = new(knobMarker.x, knobMarker.y);

            if (isStartUpdated && isEndUpdated) {
                sliderLength = Vector2.Distance(startPoint, endPoint);
            }
            else if (isEndUpdated && !isStartUpdated) {
                Vector2 basisX = new(Mathf.Cos(Mathf.Deg2Rad * endMarker.angle), Mathf.Sin(Mathf.Deg2Rad * endMarker.angle));
                startPoint = (sliderLength * -basisX) + endPoint;
            }

            Vector2 startToEndDirection = endPoint - startPoint;
            Vector2 startToKnobDirection = knobPoint - startPoint;
            // Projecting the start to knob vector on the start to end vector, will give the slide distance 
            // measured along the slider direction (the X-axis basis vector of the start point).
            float knobDistance = Vector2.Dot(startToEndDirection.normalized, startToKnobDirection.normalized) * 
                startToKnobDirection.magnitude;

            value = Mathf.Clamp(knobDistance / sliderLength, 0f, 1f);

            isTracked = (isStartUpdated && isKnobUpdated) || (isEndUpdated && isKnobUpdated);

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
            startPointImage.rectTransform.localPosition =
                new(startMarker.x * trackingSystem.Width, -startMarker.y * trackingSystem.Height);
            startPointImage.rectTransform.localEulerAngles = new(0f, 0f, startMarker.angle);
            startIdText.text = markerIds[0].ToString();

            endPointImage.rectTransform.localPosition =
                new(endMarker.x * trackingSystem.Width, -endMarker.y * trackingSystem.Height);
            endPointImage.rectTransform.localEulerAngles = new(0f, 0f, endMarker.angle);
            endIdText.text = markerIds[1].ToString();

            knobPointImage.rectTransform.localPosition = new(knobMarker.x * trackingSystem.Width, -knobMarker.y * trackingSystem.Height);
            knobPointImage.rectTransform.localEulerAngles = new(0f, 0f, knobMarker.angle);
            knobIdText.text = markerIds[2].ToString();

            valueText.text = value.ToString("0.000");

            slideAreaImage.rectTransform.sizeDelta = new(sliderLength * trackingSystem.Width, 10f);
        }
    }
}
