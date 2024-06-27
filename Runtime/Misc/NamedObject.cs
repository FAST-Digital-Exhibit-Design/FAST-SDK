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
    /// A data structure that wraps a single object so it can be shown as an 
    /// array element with a name in the Inspector.
    /// </summary>
    /// <typeparam name="T">The type of object being wrapped.</typeparam>
    [System.Serializable]
    public struct NamedObject<T>
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// </summary>
        /// The name of the element.
        public string name;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// </summary>
        /// The object to wrap.
        public T namedObject;
    }

    /// <summary>
    /// A data structure that wraps an array of objects so they can be shown as an 
    /// array element with a name in the Inspector.
    /// </summary>
    /// <typeparam name="T">The type of objects being wrapped.</typeparam>
    [System.Serializable]
    public struct NamedObjects<T>
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// </summary>
        /// The name of the element.
        public string name;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// </summary>
        /// The objects to wrap.
        public T[] namedObjects;
    }
}