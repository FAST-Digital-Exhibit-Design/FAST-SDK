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
    /// A base class to help load resources or dependencies on startup.
    /// </summary>
    /// <remarks>
    /// Each type of <c>StartupLoader</c> executes different loading steps depending on 
    /// the resource or dependency, but every <c>StartupLoader</c> communicates updates and state 
    /// to the <see cref="FAST.StartupManager"/> and the <see cref="FAST.LoadingScreenManager"/>.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
    /// UnityEngine.Events.UnityEvent</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Coroutine.html">
    /// UnityEngine.Coroutine</a>
    public abstract class StartupLoader : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The amount of time in seconds to show a <see cref="FAST.LoadingProgress"/> message.
        /// </summary>
        [SerializeField, Range(0f, 5f)]
        protected float loadingMessageDuration = 0f;

        /// <summary>
        /// A variable to compose an error message title before invoking 
        /// the <see cref="FAST.StartupLoader.errorEvent"/>.
        /// </summary>
        /// <remarks>
        /// Using this to implement a derived class is a convention, but not strictly required.
        /// </remarks>
        protected string errorTitle;

        /// <summary>
        /// A variable to compose an error message and details before invoking 
        /// the <see cref="FAST.StartupLoader.errorEvent"/>.
        /// </summary>
        /// <remarks>
        /// Using this to implement a derived class is a convention, but not strictly required.
        /// </remarks>
        protected string errorMessage;

        /// <summary>
        /// A variable to compose a loading progress message title before invoking 
        /// the <see cref="FAST.StartupLoader.loadingEvent"/>.
        /// </summary>
        /// <remarks>
        /// Using this to implement a derived class is a convention, but not strictly required.
        /// </remarks>
        protected string loadingTitle;

        /// <summary>
        /// A variable to compose a loading progress message and details before invoking 
        /// the <see cref="FAST.StartupLoader.loadingEvent"/>.
        /// </summary>
        /// <remarks>
        /// Using this to implement a derived class is a convention, but not strictly required.
        /// </remarks>
        protected string loadingMessage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when there is a loading progress message.
        /// </summary>
        public StartupLoadingMessageEvent loadingEvent;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when there is an error message.
        /// </summary>
        public StartupErrorMessageEvent errorEvent;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when loading is done.
        /// </summary>
        public StartupSuccessEvent successEvent;

        /// <summary>
        /// The number of <see cref="FAST.StartupLoader"/>s to execute on startup.
        /// </summary>
        /// <remarks>
        /// This is used by the <see cref="FAST.StartupManager"/> to determine when resource 
        /// and dependency loading is done.
        /// </remarks>
        public static int needToLoadCount = 0;

        /// <summary>
        /// The default behavior is to increment the <see cref="FAST.StartupLoader.needToLoadCount"/>.
        /// </summary>
        protected virtual void Awake()
        {
            needToLoadCount++;
        }

        /// <summary>
        /// Runs the <see cref="FAST.StartupLoader.ExecuteLoad()"/> 
        /// <c style="color:DarkRed;"><see cref="Coroutine"/></c> 
        /// </summary>
        public void Load()
        {
            StopAllCoroutines();
            StartCoroutine(ExecuteLoad());
        }

        /// <summary>
        /// The <c style="color:DarkRed;"><see cref="Coroutine"/></c> function that executes 
        /// the loading of a resource or dependency, including progress and error messages.
        /// </summary>
        protected abstract IEnumerator ExecuteLoad();
    }
}
