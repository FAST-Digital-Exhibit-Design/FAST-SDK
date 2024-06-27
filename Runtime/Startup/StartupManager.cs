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

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace FAST
{
	/// <summary>
	/// Implements startup and loader management. Not meant to be used directly in code, but 
	/// you may want to customize the event callbacks in the Inspector.
	/// </summary>
	/// <remarks>
	/// This class runs through a list of <see cref="FAST.StartupLoader"/>s to load and initialize resources 
	/// on startup. <c style="color:DarkRed;"><see cref="UnityEvent"/></c>s are triggered after each load for a 
	/// progress update and a final event is triggered when all <see cref="FAST.StartupLoader"/>s have loaded. 
	/// Default functionality is included in the <c>Startup Prefab</c>.
	/// </remarks>
	/// @warning If you find yourself trying to use @ref FAST.StartupManager in code, you may 
	/// want to review the @ref Startup documentation to make sure you are following FAST best practices.
	/// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
	/// UnityEngine.Events.UnityEvent</a>
	public class StartupManager : MonoBehaviour {

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
		/// when a <see cref="FAST.StartupLoader"/> is done loading.
		/// </summary>
		/// <remarks>
		/// The default functionality included in the <c>Startup Prefab</c> updates the loading progress bar as 
		/// a percentage of # of <see cref="FAST.StartupLoader"/>s loaded / total # of <see cref="FAST.StartupLoader"/>s * 100.
		/// </remarks>
		public StartupLoadingPercentEvent loadingEvent;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
		/// when all <see cref="FAST.StartupLoader"/>s are done loading.
		/// </summary>
		/// <remarks>
		/// The default functionality included in the <c>Startup Prefab</c> turns off the loading screen and turns 
		/// on a a white screen saver (to help reduce display burn-in).
		/// </remarks>
		public StartupSuccessEvent successEvent;

		/// <summary>
		/// <b style="color: DarkCyan;">Runtime</b><br/>
		/// The list of <see cref="FAST.StartupLoader"/>s to load.
		/// </summary>
		[SerializeField]
		private StartupLoader[] startupLoaders;

		private int indexToLoad;
		private int doneLoadingCount;

        private void Start()
		{
			doneLoadingCount = 0;
			indexToLoad = 0;
			startupLoaders = GetComponentsInChildren<StartupLoader>(false);

			StartLoading();
		}

		private void StartLoading()
		{
			// All loading is done
			if (doneLoadingCount == StartupLoader.needToLoadCount) {
				Application.udpConnections = GetComponentsInChildren<UdpConnection>(false);
				Application.serialConnections = GetComponentsInChildren<SerialConnection>(false);
				Application.webRequests = GetComponentsInChildren<WebRequestLoader>(false).Select(x => x.webRequest).ToArray();
				Application.WriteSettings();

				Application.CopyPreviousLog();
				AsyncOperation asyncLoadScene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
				asyncLoadScene.completed += (asyncLoadScene) =>
				{
					Application.InitializeLanguage();
					successEvent.Invoke();
					successEvent.RemoveAllListeners();
				};
			}
			// Load the next resource
			else if (indexToLoad < startupLoaders.Length) {
				startupLoaders[indexToLoad++].Load();
			}
		}
		public void DoneLoading()
		{
			doneLoadingCount++;
			float loadingPercentage = 100f * (float)doneLoadingCount / (float)StartupLoader.needToLoadCount;
			loadingEvent.Invoke((int)loadingPercentage);

			StartLoading();
		}
	}
}
