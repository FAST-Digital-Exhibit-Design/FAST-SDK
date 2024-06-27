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
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace FAST
{
    /// <summary>
    /// A data structure for storing the marker tracking data.
    /// </summary>
    [System.Serializable]
    public struct MarkerData
    {
        /// <summary>
        /// Indicates the tracking state of a marker. 
        /// </summary>
        public enum TrackingState
        {
            /// <summary>
            /// The marker is not tracked and hasn't been tracked for more than the 
            /// <see cref="FAST.MarkerTrackingSystem.trackingTimeout"/> period.
            /// </summary>
            NotTracked,

            /// <summary>
            /// The marker is not tracked, but it is probably temporary occlusion 
            /// and the last known tracking data will be provided.
            /// </summary>
            /// <remarks>
            /// The state is considered temporary occlusion because the marker was 
            /// last tracked less than the <see cref="FAST.MarkerTrackingSystem.trackingTimeout"/> 
            /// period.
            /// </remarks>
            Inferred,

            /// <summary>
            /// The marker is tracked.
            /// </summary>
            Tracked
        }

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The ID number encoded by the pattern on the tracked marker.
        /// </summary>
        public int id;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The normalized X-coordinate of the marker position after it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is normalized [0, 1] where 1 is the width of the tracking area.
        /// </remarks>
        public float x;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The normalized Y-coordinate of the marker position after it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is normalized [0, 1] where 1 is the height of the tracking area.
        /// </remarks>
        public float y;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The orientation angle of the marker after it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is measured in degrees.
        /// </remarks>
        public float angle;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The normalized diameter of the marker after it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is normalized [0, 1] where 1 is the area (width * height) of the tracking area.
        /// </remarks>
        public float size;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The raw normalized X-coordinate of the marker position before it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is normalized [0, 1] where 1 is the width of the tracking area.
        /// </remarks>
        public float rawX;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The raw normalized Y-coordinate of the marker position before it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is normalized [0, 1] where 1 is the height of the tracking area.
        /// </remarks>
        public float rawY;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The raw orientation of the marker before it has been filtered/smoothed.
        /// </summary>
        public float rawAngle;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The raw diameter of the marker position before it has been filtered/smoothed.
        /// </summary>
        /// <remarks>
        /// This value is measured in pixels.
        /// </remarks>
        public float rawSize;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The timestamp of when the marker data was recieved, measured is seconds since the beginning 
        /// of the application.
        /// </summary>
        public double timeReceived;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The tracking state of the marker, such as Tracked, Inferred, or Not Tracked.
        /// </summary>
        public TrackingState trackingState;
    }

    /// <summary>
    /// Manages marker tracking in terms of receiving, filtering, storing, 
    /// accessing, and visualizing the marker data.
    /// </summary>
    /// <remarks>
    /// Use the <c>Marker Tracking System Prefab</c> to instantiate a <c>MarkerTrackingSystem</c> 
    /// with default attributes and assignments.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
	/// UnityEngine.GameObject</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/RectTransform.html">
    /// UnityEngine.RectTransform</a>
    public class MarkerTrackingSystem : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings</b><br/>
        /// The <see cref="FAST.UdpConnection"/> to receive marker tracking data from.
        /// </summary>
        [SerializeField]
        private UdpConnection udpConnection;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The <c style="color:DarkRed;"><see cref="RectTransform"/></c> that represents 
        /// the marker tracking area.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will already be assigned if you use the <c>Marker Tracking System Prefab</c>.
        /// </para>
        /// <para>
        /// Specifying the tracking area as a <c style="color:DarkRed;"><see cref="RectTransform"/></c> 
        /// allows a <see cref="FAST.AlignmentRectTransform"/> to be used where the size is specified 
        /// in the activity settings XML file.
        /// </para>
        /// </remarks>
        [Header("Tracking Properties")]
        [SerializeField]
        private RectTransform trackingArea;

        /// <summary>
        /// Returns the width of the <see cref="FAST.MarkerTrackingSystem.trackingArea"/>.
        /// </summary>
        public float Width
        {
            get => trackingArea.sizeDelta.x;
        }

        /// <summary>
        /// Returns the height of the <see cref="FAST.MarkerTrackingSystem.trackingArea"/>.
        /// </summary>
        public float Height
        {
            get => trackingArea.sizeDelta.y;
        }

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The amount of time in seconds before the marker tracking changes from 
        /// <see cref="FAST.MarkerData.TrackingState.Inferred"/> to <see cref="FAST.MarkerData.TrackingState.NotTracked"/> 
        /// when a marker isn't detected.
        /// </summary>
        [SerializeField]
        private double trackingTimeout = 0.25;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The amount of filtering to apply to the marker position and size.
        /// </summary>
        /// <remarks>
        /// 0 is the new raw position/size and 1 is the previous filtered position/size. Any value in between 
        /// is linearly interpolated. 
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        private float positionFilterAmount = 0.5f;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The amount of filtering to apply to the marker orientation.
        /// </summary>
        /// <remarks>
        /// 0 is the new raw orientation and 1 is the previous filtered orientation. Any value in between 
        /// is linearly interpolated. 
        /// </remarks>
        [SerializeField, Range(0f, 1f)]
        private float rotationFilterAmount = 0.25f;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the tracked markers should be visualized.
        /// </summary>
        [Header("Marker Properties")]
        [SerializeField]
        private bool isDrawMarkers;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings</b><br/>
        /// The maximum number of markers that this marker tracking system can track.
        /// </summary>
        /// <remarks>
        /// This value can be found in the Computer Vision app, but the default is 24.
        /// </remarks>
        [SerializeField]
        private int maxNumMarkers = 24;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The parent <c style="color:DarkRed;"><see cref="GameObject"/></c> where 
        /// <c>Marker Prefabs</c> will be added at runtime to visualize the markers.
        /// </summary>
        /// <remarks>
        /// This will already be assigned if you use the <c>Marker Tracking System Prefab</c>.
        /// </remarks>
        [SerializeField]
        private GameObject markerParent;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The prefab to use as a visualization of a marker at runtime.
        /// </summary>
        /// <remarks>
        /// This will already be assigned as the <c>Marker Prefab</c> if you use the <c>Marker Tracking System Prefab</c>.
        /// </remarks>
        [SerializeField]
        private GameObject markerPrefab;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The array of marker visualization <c style="color:DarkRed;"><see cref="GameObject"/>s</c> 
        /// instantiated from <see cref="FAST.MarkerTrackingSystem.markerPrefab"/> at runtime.
        /// </summary>
        [SerializeField]
        private GameObject[] markers;

        /// <summary>
        /// The most recently recieved marker tracking data ordered by <see cref="FAST.MarkerData.id"/>.
        /// </summary>
        public MarkerData[] markerDataLUT;
        private MarkerData[] lastMarkerDataLUT;

        private void Awake()
        {
            MarkerTrackingSettings settings = Application.settings.markerTrackingSettings;
            maxNumMarkers = settings.maxNumMarkers;

            try {
                udpConnection = Application.udpConnections
                    .Where((udpConnection) => udpConnection.id.ToLower() == settings.udpConnectionId.ToLower())
                    .Single();
            }
            catch (Exception exception) {
                Debug.LogError(exception.Message);
            }
            markerDataLUT = new MarkerData[maxNumMarkers];
            lastMarkerDataLUT = new MarkerData[maxNumMarkers];

            // Add markers to the scene
            markers = new GameObject[maxNumMarkers];
            for (int i = 0; i < markers.Length; i++) {
                markers[i] = Instantiate(markerPrefab, markerParent.transform);
                markers[i].name = $"Marker ({i})";
                var idText = markers[i].GetComponentInChildren<TMP_Text>();
                idText.text = $"{i}";
            }
        }

        private void Update()
        {
            // Retrieve all the UDP messages
            List<byte[]> udpData = udpConnection.Data;

            if (udpData.Count < 1) {
                return;
            }

            // Only the last UDP message is needed
            byte[] bytes = udpData.Last<byte[]>();

            if (bytes.Length == 0) {
                return;
            }

            // Determine the number of tracked markers
            int index = 0;
            int stopIndex = bytes.Length - (2 * sizeof(int));
            int frameNumber = BitConverter.ToInt32(bytes, index);
            int numMarkersReceived = BitConverter.ToInt32(bytes, index += 4);

            // timeout any markers that haven't been tracked or default to inferred
            double timeReceived = Time.timeAsDouble;
            for (int i = 0; i < maxNumMarkers; i++) {
                MarkerData.TrackingState trackingState = MarkerData.TrackingState.Inferred;

                if (timeReceived - lastMarkerDataLUT[i].timeReceived > trackingTimeout) {
                    trackingState = MarkerData.TrackingState.NotTracked;
                }

                markerDataLUT[i].trackingState = trackingState;
            }

            // Update the marker data LUT
            while (index < stopIndex) {
                MarkerData markerData = new();
                markerData.id = BitConverter.ToInt32(bytes, index += 4);
                markerData.rawX = BitConverter.ToSingle(bytes, index += 4);
                markerData.rawY = BitConverter.ToSingle(bytes, index += 4);
                markerData.rawAngle = BitConverter.ToSingle(bytes, index += 4);
                markerData.rawSize = BitConverter.ToSingle(bytes, index += 4);
                markerData.timeReceived = timeReceived;
                markerData.trackingState = MarkerData.TrackingState.Tracked;

                // filter the data if the marker was tracked in the last update
                MarkerData lastMarkerData = lastMarkerDataLUT[markerData.id];
                markerData.x = Mathf.Lerp(markerData.rawX, lastMarkerData.x, positionFilterAmount);
                markerData.y = Mathf.Lerp(markerData.rawY, lastMarkerData.y, positionFilterAmount);
                // LerpAngle() takes care of wraping when changing 360 -> 0 or 0 -> 360.
                markerData.angle = Mathf.LerpAngle(markerData.rawAngle, lastMarkerData.angle, rotationFilterAmount);
                markerData.size = Mathf.Lerp(markerData.rawSize, lastMarkerData.size, positionFilterAmount);

                markerDataLUT[markerData.id] = markerData;
            }
            lastMarkerDataLUT = markerDataLUT.Clone() as MarkerData[];

            if (isDrawMarkers) {
                DrawMarkers();
            }
            else {
                HideMarkers();
            }

        }
        private void DrawMarkers()
        {
            // Intialize by turning off all marker UI elements
            HideMarkers();

            // Update each tracked marker UI element
            foreach (MarkerData markerData in markerDataLUT) {
                if (markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked)) {
                    continue;
                }
                markers[markerData.id].SetActive(true);
                markers[markerData.id].transform.localPosition = new(Width * markerData.x, -(Height * markerData.y), 0f);
                markers[markerData.id].transform.localEulerAngles = new(0f, 0f, markerData.angle);
            }
        }

        private void HideMarkers()
        {
            foreach (GameObject marker in markers) {
                marker.SetActive(false);
            }
        }

        /// <summary>
        /// Toggles the visualization of markers on/off.
        /// </summary>
        public void OnToggleUI()
        {
            isDrawMarkers = !isDrawMarkers;
        }

    }
}
