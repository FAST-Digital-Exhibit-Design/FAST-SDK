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
    /// A simple singleton pattern for classes that inherit from <c style="color:DarkRed;"><see cref="MonoBehaviour"/></c> 
    /// with public access to the instance.
    /// </summary>
    /// <remarks>This implementation is not thread-safe.</remarks>
    /// <typeparam name="T">The type of the derived class.</typeparam>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/MonoBehaviour.html">
    /// UnityEngine.MonoBehaviour</a>
    public abstract class SingletonWithInstance<T> : Singleton<T> where T : SingletonWithInstance<T>
    {
        /// <summary>
        /// Provides public access to <see cref="FAST.Singleton{T}._instance"/>.
        /// </summary>
        public static T Instance => _instance;
    }
}
