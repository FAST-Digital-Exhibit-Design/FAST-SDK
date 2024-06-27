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
using UnityEngine.Events;

namespace FAST
{
    /// <summary>
    /// A <c style="color:DarkRed;"><see cref="UnityEvent"/></c> used by <see cref="FAST.StartupLoader"/> 
    /// to update the loading screen title and message during startup. 
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
	/// UnityEngine.Events.UnityEvent</a>
    [System.Serializable]
    public class StartupLoadingMessageEvent : UnityEvent<string, string> { };

    /// <summary>
    /// A <c style="color:DarkRed;"><see cref="UnityEvent"/></c> used by <see cref="FAST.StartupManager"/> 
    /// to update the loading screen progress bar during startup. 
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
	/// UnityEngine.Events.UnityEvent</a>
    [System.Serializable]
    public class StartupLoadingPercentEvent : UnityEvent<int> { };

    /// <summary>
    /// A <c style="color:DarkRed;"><see cref="UnityEvent"/></c> used by <see cref="FAST.StartupLoader"/> 
    /// to update the on-screen title and message during a startup error. 
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
	/// UnityEngine.Events.UnityEvent</a>
    [System.Serializable]
    public class StartupErrorMessageEvent : UnityEvent<string, string> { };

    /// <summary>
    /// A <c style="color:DarkRed;"><see cref="UnityEvent"/></c> used by a <see cref="FAST.StartupLoader"/> 
    /// when it has finished loading and also used by <see cref="FAST.StartupManager"/> when all 
    /// <see cref="FAST.StartupLoader"/>s have been loaded on startup. 
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
	/// UnityEngine.Events.UnityEvent</a>
    [System.Serializable]
    public class StartupSuccessEvent : UnityEvent { };
}