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
    /// A predefined tool for a tangible object.
    /// </summary>
    /// <remarks>
    /// <para>A tangible object is a physical object that uses 1 or more markers to 
    /// track the object's position and rotation.
    /// </para>
    /// <para>
    /// Use the <c>Marker Object Prefab</c> to instantiate this tool with default 
    /// attributes and assignments for 1 marker.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/RectTransform.html">
    /// UnityEngine.RectTransform</a>
    public class MarkerObject : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies this tangible object.
        /// </summary>
        public int value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the markers used by this tangible object.
        /// </summary>
        public MarkerData[] objectMarkers = new MarkerData[1];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/>s</c> to visualize the 
        /// position of markers used by this tangible object.
        /// </summary>
        /// <remarks>
        /// This will already be assigned if you use the <c>Marker Object Prefab</c>.
        /// </remarks>
        [SerializeField]
        private Image[] objectPointImages = new Image[1];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the IDs 
        /// of the markers used by this tangible object.
        /// </summary>
        /// <remarks>
        /// This will already be assigned if you use the <c>Marker Object Prefab</c>.
        /// </remarks>
        [SerializeField]
        private TMP_Text[] objectIdText = new TMP_Text[1];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The parent UI <c style="color:DarkRed;"><see cref="RectTransform"/></c> that 
        /// contains the label text for this tangible object.
        /// </summary>
        /// <remarks>
        /// This will already be assigned if you use the <c>Marker Object Prefab</c>.
        /// </remarks>
        [SerializeField]
        private RectTransform objectLabelsTransform;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the name of this 
        /// tangible object based on the name of the <c style="color:DarkRed;"><see cref="GameObject"/></c> 
        /// this script is attached to.
        /// </summary>
        /// <remarks>
        /// This will already be assigned if you use the <c>Marker Object Prefab</c>.
        /// </remarks>
        [SerializeField]
        private TMP_Text objectNameText;


        void Update()
        {
            isTracked = false;

            for (int i = 0; i < objectMarkers.Length; i++) {
                objectMarkers[i] = trackingSystem.markerDataLUT[markerIds[i]];
                bool isMarkerUpdated = !objectMarkers[i].trackingState.Equals(MarkerData.TrackingState.NotTracked);
                objectPointImages[i].gameObject.SetActive(isMarkerUpdated);

                isTracked |= isMarkerUpdated;
            }


            canvasGroup.alpha = isTracked && isDrawTool ? 1 : 0;

            DrawTool();
        }

        protected override void DrawTool()
        {
            Vector2 centerPosition = new();
            int numTrackedMarkers = 0;
            for (int i = 0; i < objectMarkers.Length; i++) {
                Vector2 markerPostion = new(objectMarkers[i].x * trackingSystem.Width, -objectMarkers[i].y * trackingSystem.Height);
                Vector3 markerAngles = new(0f, 0f, objectMarkers[i].angle);

                objectPointImages[i].rectTransform.localPosition = markerPostion;
                objectPointImages[i].rectTransform.localEulerAngles = markerAngles;
                objectIdText[i].text = markerIds[i].ToString();

                if (!objectMarkers[i].trackingState.Equals(MarkerData.TrackingState.NotTracked)) {
                    centerPosition += markerPostion;
                    numTrackedMarkers++;
                }
            }
            if (numTrackedMarkers > 0) {
                centerPosition /= (float)numTrackedMarkers;
            }
            objectLabelsTransform.localPosition = centerPosition + (Vector2.up * 40f);
            objectNameText.text = name;
        }
    }
}
