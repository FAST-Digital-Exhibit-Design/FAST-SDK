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
    /// Provides alignment adjustments to a <c style="color:DarkRed;"><see cref="RectTransform"/></c> using offsets.
    /// </summary>
    /// <remarks>
    /// Use this on a <c style="color:DarkRed;"><see cref="GameObject"/></c> which may need to have its 
    /// position, rotation, scale, or size adjusted at runtime. It is meant to be used with the <c>AlignmentTool</c> 
    /// prefab and <see cref="FAST.AlignmentRectTransformSettings"/> to load, edit, and save transform offsets.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/RectTransform.html">
    /// UnityEngine.RectTransform</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
    /// UnityEngine.GameObject</a>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/Canvas.html">
    /// UnityEngine.Canvas</a>
    [RequireComponent(typeof(RectTransform))]
    public class AlignmentRectTransform : AlignmentTransform
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The offset added to the <see cref="FAST.AlignmentRectTransform.initialSize"/>.
        /// </summary>
        public Vector2 offsetSize;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The initial size at runtime, which is set in the Editor.
        /// </summary>
        [SerializeField]
        protected Vector2 initialSize;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The canvas this <c style="color:DarkRed;"><see cref="RectTransform"/></c> belongs to.
        /// </summary>
        public Canvas parentCanvas;
        private RectTransform rectTransform;
        
        protected override void Awake()
        {
            parentCanvas = GetComponentInParent<Canvas>();
            if (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) {
                drawingCamera = null;
            }
            else {
                drawingCamera = parentCanvas.worldCamera;
            }

            rectTransform = transform as RectTransform;
            initialPosition = rectTransform.position;
            initialRotation = rectTransform.rotation;
            initialScale = rectTransform.localScale;
            initialSize = rectTransform.sizeDelta;
        }
        protected override void Update()
        {
            rectTransform.position = initialPosition + (parentCanvas.transform.localScale.x * offsetPosition);
            Quaternion rotationQuaternion = Quaternion.AngleAxis(offsetRotation, Vector3.forward);
            rectTransform.rotation = rotationQuaternion * initialRotation;
            rectTransform.localScale = initialScale * (1f + offsetScale);
            rectTransform.sizeDelta = initialSize + offsetSize;
        }
    }
}
