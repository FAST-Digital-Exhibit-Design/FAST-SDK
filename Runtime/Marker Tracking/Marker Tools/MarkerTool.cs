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

namespace FAST
{
    /// <summary>
    /// A base class to define marker-based tools and interactions.
    /// </summary>
    /// <remarks>
    /// Derive from this class to make your own custom marker-based tools 
    /// and interactions, or use one of the 7 pre-defined tools.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/CanvasGroup.html">
    /// UnityEngine.CanvasGroup</a>
    public abstract class MarkerTool : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the tool should be visualized.
        /// </summary>
        [SerializeField]
        protected bool isDrawTool;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// Returns <see langword="true"/> when enough of the markers that 
        /// compose the tool are tracked for the tool to function.
        /// </summary>
        public bool isTracked;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The IDs of the markers to use for this tool.
        /// </summary>
        public int[] markerIds;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The <see cref="FAST.MarkerTrackingSystem"/> providing the marker data for this tool.
        /// </summary>
        [SerializeField]
        protected MarkerTrackingSystem trackingSystem;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The <c style="color:DarkRed;"><see cref="CanvasGroup"/></c> to control the 
        /// visualization of this tool.
        /// </summary>
        /// <remarks>
        /// This is primarily used to turn the visualization on/off, but you could 
        /// also control the transparency using <c style="color:DarkRed;"><see cref="CanvasGroup.alpha"/></c>.
        /// </remarks>
        [SerializeField]
        protected CanvasGroup canvasGroup;

        /// <summary>
        /// Override this function to implement the visualization of this tool.
        /// </summary>
        protected abstract void DrawTool();

        /// <summary>
        /// Toggles the visualization of this tool on/off.
        /// </summary>
        public void OnToggleUI()
        {
            isDrawTool = !isDrawTool;
        }
    }
}