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
    /// A predefined tool for a 6-sided dice.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The dice is a physical cube with a marker on each side. 
    /// You could make a dice with a different number of sides, but it must be designed 
    /// so that only one marker is visible to the camera and tracked at a time.
    /// </para>
    /// <para>
    /// Use the <c>Marker Dice Prefab</c> to instantiate this tool with default 
    /// attributes and assignments for 6 markers.
    /// </para>
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/Packages/com.unity.textmeshpro@3.0/api/TMPro.TMP_Text.html">
    /// TMPro.TMP_Text</a>
    public class MarkerDice : MarkerTool
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Identifies the side of the dice that is currently being tracked.
        /// </summary>
        public string value;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The identifier to use for each side of the dice when that side is being tracked.
        /// </summary>
        [SerializeField]
        private string[] diceValues = new string[6];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The tracking data for the markers used by this dice.
        /// </summary>
        public MarkerData[] diceMarkers = new MarkerData[6];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="Image"/>s</c> to visualize the 
        /// position of markers used by this dice.
        /// </summary>
        [SerializeField]
        private Image[] dicePointImages = new Image[6];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the IDs 
        /// of the markers used by this dice.
        /// </summary>
        [SerializeField]
        private TMP_Text[] diceIdTexts = new TMP_Text[6];

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// UI <c style="color:DarkRed;"><see cref="TMP_Text"/></c> to label the 
        /// <see cref="FAST.MarkerDice.value"/>.
        /// </summary>
        [SerializeField]
        private TMP_Text diceValueText;

        private int sideSelectedIndex = 0;

        void Update()
        {
            bool[] isSideUpdated = { false, false, false, false, false, false };
            int numSidesUpdated = 0;
            for (int i = 0; i < markerIds.Length; i++) {
                MarkerData markerData = trackingSystem.markerDataLUT[markerIds[i]];
                isSideUpdated[i] = !markerData.trackingState.Equals(MarkerData.TrackingState.NotTracked);

                if (isSideUpdated[i]) {
                    diceMarkers[i] = markerData;
                    value = diceValues[i];
                    sideSelectedIndex = i;
                    numSidesUpdated++;
                }
                dicePointImages[i].gameObject.SetActive(isSideUpdated[i]);
            }

            isTracked = numSidesUpdated == 1;

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
            for (int i = 0; i < markerIds.Length; i++) {
                dicePointImages[i].rectTransform.localPosition = 
                    new(diceMarkers[i].x * trackingSystem.Width, -diceMarkers[i].y * trackingSystem.Height);
                //dicePointImages[i].rectTransform.localEulerAngles = new Vector3(0f, 0f, diceMarkers[i].angle);
                diceIdTexts[i].text = markerIds[i].ToString();
            }
            diceValueText.text = value;
            diceValueText.rectTransform.localPosition = 
                new(diceMarkers[sideSelectedIndex].x * trackingSystem.Width, (-diceMarkers[sideSelectedIndex].y * trackingSystem.Height) + 60f);
            //diceValueText.rectTransform.localEulerAngles = new Vector3(0f, 0f, diceMarkers[sideSelectedIndex].angle);
        }
    }
}
