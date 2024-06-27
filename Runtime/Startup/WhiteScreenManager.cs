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

using UnityEngine;
using TMPro;
using System.Collections;

namespace FAST
{
	/// <summary>
	/// Manages GUI for a white screen saver to help reduce display burn-in. Not meant to be used directly.
	/// </summary>
	/// <remarks>
	/// Default functionality is included in the <c>Startup Prefab</c>.
	/// </remarks>
	/// @warning If you find yourself trying to use @ref FAST.WhiteScreenManager, you may 
	/// want to review the @ref Startup documentation to make sure you are following FAST best practices.
	/// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
	/// UnityEngine.GameObject</a>
	public class WhiteScreenManager : MonoBehaviour
	{
		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// The amount of time in minutes that the white screen is shown for.
		/// </summary>
		[SerializeField]
		private int durationMinutes = 30;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if touch input should dismiss the white screen.
		/// </summary>
		[Space(10)]
		[SerializeField]
		private bool isTouchInput = true;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if mouse input should dismiss the white screen.
		/// </summary>
		[SerializeField]
		private bool isMouseInput = true;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if joystick input should dismiss the white screen.
		/// </summary>
		[SerializeField]
		private bool isAxisInput = true;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if key and mouse button press input should dismiss the white screen.
		/// </summary>
		[SerializeField]
		private bool isKeypressInput = true; // include mouse buttons

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if serial input should dismiss the white screen.
		/// </summary>
		[Space(10)]
		[SerializeField]
		private bool isSerialInput = true;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// The number of serial messages that must be recieved to dismiss the white screen.
		/// </summary>
		/// <remarks>
		/// This can be helpful if the input is noisy or very sensitive.
		/// </remarks>
		[SerializeField]
		private int serialInputCount = 10;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// Set to <see langword="true"/> if UDP input should dismiss the white screen.
		/// </summary>
		[Space(10)]
		[SerializeField]
		private bool isUdpInput = false;

		/// <summary>
		/// <b style="color: DarkCyan;">Settings, Inspector</b><br/>
		/// The number of UDP datagrams that must be recieved to dismiss the white screen.
		/// </summary>
		/// <remarks>
		/// This can be helpful if the input is noisy or very sensitive.
		/// </remarks>
		[SerializeField]
		private int udpInputCount = 1;


		private Vector3 startMousePosition;

		private void Start()
		{
			WhiteScreenSettings settings = Application.settings.whiteScreenSettings;

            TMP_Text[] whiteScreenMessages = GetComponentsInChildren<TMP_Text>();
			string whiteScreenMessage = settings.onScreenMessage;
			whiteScreenMessage = whiteScreenMessage.Replace("\\n", "\n").Replace("\\t", "\t");
			foreach (var message in whiteScreenMessages) {
				message.text = whiteScreenMessage;
			}

            durationMinutes = settings.durationMinutes;

			isTouchInput = settings.isTouchInput;
			isMouseInput = settings.isMouseInput;
			startMousePosition = Input.mousePosition;

			isAxisInput = settings.isAxisInput;
			isKeypressInput = settings.isKeypressInput;

			isSerialInput = settings.isSerialInput;
			serialInputCount = settings.serialInputCount;
            if (isSerialInput) {
				foreach (var connection in Application.serialConnections) {
					connection.onDataReceivedEvent += OnSerialInput;
				}
			}

			isUdpInput = settings.isUdpInput;
			udpInputCount = settings.udpInputCount;
			if (isUdpInput) {
				foreach (var connection in Application.udpConnections) {
					connection.onDataReceivedEvent += OnUdpInput;
				}
			}

			if (durationMinutes > 0) {
				StartCoroutine(DisplayWhiteScreen());
			}
			else {
				RemoveWhiteScreen();
            }

		}

		private void Update()
		{
			if (isTouchInput && Input.touchCount > 0) {
				RemoveWhiteScreen();
            }
			else if (isMouseInput && startMousePosition != Input.mousePosition) {
				RemoveWhiteScreen();
			}
			else if(isSerialInput && serialInputCount < 0) {
        		RemoveWhiteScreen();
			}
			else if (isUdpInput && udpInputCount < 0) {
				RemoveWhiteScreen();
			}
			else if (isKeypressInput && Input.anyKeyDown) {
				RemoveWhiteScreen();
			}
			else if (isAxisInput && Input.GetAxis("Horizontal") != 0f) {
				RemoveWhiteScreen();
			}
			else if (isAxisInput && Input.GetAxis("Vertical") != 0f) {
				RemoveWhiteScreen();
			}
		}

		private void OnSerialInput(string data) {
			serialInputCount--;
		}
		private void OnUdpInput(byte[] data)
		{
			udpInputCount--;
		}

		private IEnumerator DisplayWhiteScreen()
		{
			yield return new WaitForSeconds(60 * durationMinutes);
			RemoveWhiteScreen();
		}

		private void RemoveWhiteScreen()
		{
			StopAllCoroutines();
			foreach (var connection in Application.serialConnections) {
				connection.onDataReceivedEvent -= OnSerialInput;
			}
			foreach (var connection in Application.udpConnections) {
				connection.onDataReceivedEvent -= OnUdpInput;
			}
			gameObject.SetActive(false);
		}
	}
}