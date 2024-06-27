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
    /// A predefined tool for a "magic" window.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a physical, rectangualr window that you can project onto and reveal a different 
    /// layer of content within it. A marker it place in each corner of the window as a reference 
    /// for the projection area. All 4 marker must always be visible.
    /// </para>
    /// <para>
    /// Use the <c>Marker Window Prefab</c> to instantiate this tool with default 
    /// attributes and assignments.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MeshFilter.html">
	/// UnityEngine.MeshFilter</a>
    public class MarkerWindow : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies this window.
        /// </summary>
        public string value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the markers used by this window.
        /// </summary>
        public MarkerData[] windowMarkers = new MarkerData[4];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/>s</c> to visualize the 
        /// position of markers used by this window.
        /// </summary>
        [SerializeField]
        private Image[] windowPointImages = new Image[4];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the IDs 
        /// of the markers used by this window.
        /// </summary>
        [SerializeField]
        private TMP_Text[] windowIdTexts = new TMP_Text[4];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the  
        /// <see cref="FAST.MarkerWindow.value"/>.
        /// </summary>
        [SerializeField]
        private TMP_Text windowValueText;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The quad <c style="color:DarkRed;"><see cref="MeshFilter"/></c> to visualize 
        /// the projection area of this window.
        /// </summary>
        [SerializeField]
        private MeshFilter windowMeshFilter;

        private Mesh mesh;
        private Vector3[] vertices;

        void Start()
        {
            mesh = windowMeshFilter.mesh;
            vertices = mesh.vertices;
        }
        void Update()
        {
            MarkerData markerData;
            bool isCornerUpdated;
            bool isWindowUpdated = true;
            for (int i = 0; i < markerIds.Length; i++) {

                markerData = trackingSystem.markerDataLUT[markerIds[i]];
                isCornerUpdated = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);
                if (isCornerUpdated) {
                    windowMarkers[i] = markerData;
                }
                isWindowUpdated = isWindowUpdated && isCornerUpdated;
            }

            isTracked = isWindowUpdated;

            if (isDrawTool) {
                canvasGroup.alpha = isTracked ? 1 : 0;
                windowMeshFilter.gameObject.SetActive(isTracked);
                DrawTool();
            }
            else {
                canvasGroup.alpha = 0;
                windowMeshFilter.gameObject.SetActive(false);
            }
        }

        protected override void DrawTool()
        {
            Vector3 centerPosition = new();
            float angle = 0f;
            for (int i = 0; i < markerIds.Length; i++) {
                windowPointImages[i].rectTransform.localPosition =
                    new(windowMarkers[i].x * trackingSystem.Width, -windowMarkers[i].y * trackingSystem.Height);
                windowPointImages[i].rectTransform.localEulerAngles = new(0f, 0f, windowMarkers[i].angle);
                windowIdTexts[i].text = markerIds[i].ToString();

                centerPosition += (windowPointImages[i].rectTransform.localPosition * 0.25f);
                angle += (windowMarkers[i].angle * 0.25f);
            }
            windowValueText.text = value;
            windowValueText.rectTransform.localPosition = centerPosition;
            windowValueText.rectTransform.localEulerAngles = new(0f, 0f, angle);


            vertices[0] = new(windowMarkers[3].x * trackingSystem.Width, -windowMarkers[3].y * trackingSystem.Height);
            vertices[1] = new(windowMarkers[2].x * trackingSystem.Width, -windowMarkers[2].y * trackingSystem.Height);
            vertices[2] = new(windowMarkers[0].x * trackingSystem.Width, -windowMarkers[0].y * trackingSystem.Height);
            vertices[3] = new(windowMarkers[1].x * trackingSystem.Width, -windowMarkers[1].y * trackingSystem.Height);

            mesh.vertices = vertices;
            mesh.RecalculateBounds();
        }
    }
}
