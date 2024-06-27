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
	/// Provides alignment adjustments to a <c style="color:DarkRed;"><see cref="Transform"/></c> using offsets.
	/// </summary>
	/// <remarks>
	/// Use this on a <c style="color:DarkRed;"><see cref="GameObject"/></c> which may need to have its 
    /// position, rotation, or scale adjusted at runtime. It is meant to be used with the <c>AlignmentTool</c> 
    /// prefab and <see cref="FAST.AlignmentTransformSettings"/> to load, edit, and save transform offsets.
	/// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Transform.html">
    /// UnityEngine.Transform</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
    /// UnityEngine.GameObject</a>
    [RequireComponent(typeof(Transform))]
    public class AlignmentTransform : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The camera to render <c>AlignmentTool</c> visual highlight, label, and bounding box to.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For <see cref="FAST.AlignmentTransform"/>, the camera must be specified.
        /// </para>
        /// <para>
        /// For <see cref="FAST.AlignmentRectTransform"/>, if the camera is <see langword="null"/> the visuals 
        /// are rendered as a screen space overlay.
        /// </para>
        /// </remarks>
        public Camera drawingCamera;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The offset added to the <see cref="FAST.AlignmentTransform.initialPosition"/>.
        /// </summary>
        [Header("Runtime")]
        public Vector3 offsetPosition;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The Z-axis offset added to the <see cref="FAST.AlignmentTransform.initialRotation"/>.
        /// </summary>
        public float offsetRotation;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Inspector, Code</b><br/>
        /// The uniform offset added to the <see cref="FAST.AlignmentTransform.initialScale"/>.
        /// </summary>
        public float offsetScale;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The initial position at runtime, which is set in the Editor.
        /// </summary>
        [SerializeField]
        protected Vector3 initialPosition;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The initial rotation at runtime, which is set in the Editor.
        /// </summary>
        [SerializeField]
        protected Quaternion initialRotation;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The initial scale at runtime, which is set in the Editor.
        /// </summary>
        [SerializeField]
        protected Vector3 initialScale;

        protected virtual void Awake()
        {
            initialPosition = transform.position;
            initialRotation = transform.rotation;
            initialScale = transform.localScale;
        }

        protected virtual void Update()
        {
            transform.position = initialPosition + offsetPosition;
            Quaternion rotationQuaternion = Quaternion.AngleAxis(offsetRotation, Vector3.forward);
            transform.rotation = rotationQuaternion * initialRotation;
            transform.localScale = initialScale * (1f + offsetScale);
        }
    }
}
