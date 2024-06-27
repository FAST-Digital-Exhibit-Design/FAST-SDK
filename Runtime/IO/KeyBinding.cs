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
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using NaughtyAttributes;

namespace FAST
{
	/// <summary>
	/// Maps key presses to callbacks and actions.
	/// </summary>
	/// <remarks>
	/// Key binding <see cref="FAST.KeyBinding.ActiveState"/>s determine which contexts the key presses will apply.
	/// </remarks>
	public class KeyBinding : MonoBehaviour
	{
		/// <summary>
		/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
		/// Set to <see langword="true"/> if the key press should be logged to the 
		/// Editor Console or Player log.
		/// </summary>
		public bool logKeys = false;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// Set to <see langword="true"/> if this key binding should apply in all states.
		/// </summary>
		[Space(10)]
		[SerializeField]
		private bool isAlwaysActive;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// The list of states this key binding will apply to.
		/// </summary>
		/// <remarks>
		/// Add named states to this list as needed using <see cref="FAST.KeyBinding.StateType.Custom"/>.
		/// </remarks>
		[SerializeField, HideIf("isAlwaysActive")]
		private List<ActiveState> activeStates = new();
		private readonly List<string> activeStateNames = new();

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// The list of key presses that will trigger <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
		/// callbacks.
		/// </summary>
		[Space(10)]
		[SerializeField, Tooltip("Key presses to trigger Unity Event callbacks")]
		private UnityEventKeyCommand[] unityEventKeys;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// The list of key presses that will toggle <c style="color:DarkRed;"><see cref="GameObject"/>s</c> 
		/// active/inactive.
		/// </summary>
		[SerializeField, Tooltip("Key presses to toggle GameObject active/inactive")]
		private GameObjectKeyCommand[] gameObjectKeys;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// The list of key presses that will trigger UI 
		/// <c style="color:DarkRed;"><see cref="Button.onClick()"/></c> events.
		/// </summary>
		[SerializeField, Tooltip("Key presses to trigger UI Button OnClick()")]
		private ButtonKeyCommand[] uiButtonKeys;

		/// <summary>
		/// <b style="color: DarkCyan;">Inspector</b><br/>
		/// The list of key presses that will toggle UI 
		/// <c style="color:DarkRed;"><see cref="Toggle"/>s</c> on/off.
		/// </summary>
		[SerializeField, Tooltip("Key presses to toggle UI Toggle on/off")]
		private ToggleKeyCommand[] uiToggleKeys;

		private List<KeyCommand> _keyCommands;
		private List<UnityEventKeyCommand> _keyUps;

		/// <summary>
		/// The name of the default <see cref="FAST.KeyBinding.ActiveState"/>.
		/// </summary>
		public const string kDefaultState = "Default";
		private static string currentState = kDefaultState;
		private static string previousState = kDefaultState;
		private static readonly List<KeyBinding> keyBindings = new();

		private void OnValidate()
		{
			int defaultCount = 0;
			foreach (var state in activeStates) {
				if (state.type.Equals(StateType.Default)) {
					defaultCount++;
					if (defaultCount > 1) {
						state.type = StateType.Custom;
						state.name = "";
						continue;
					}
					state.name = kDefaultState;
				}
				else if (state.type.Equals(StateType.Custom) && state.name.Equals(kDefaultState)) {
					state.name = "";
				}
			}
		}

		private void Awake()
		{
			keyBindings.Add(this);
			foreach (var activeState in activeStates) {
				if (!activeStateNames.Contains(activeState.name) && !string.IsNullOrWhiteSpace(activeState.name)) {
					activeStateNames.Add(activeState.name);
				}
			}

			_keyCommands = new List<KeyCommand>();
			_keyUps = new List<UnityEventKeyCommand>();
			foreach (var item in uiButtonKeys) {
				_keyCommands.Add(item);
			}
			foreach (var item in uiToggleKeys) {
				_keyCommands.Add(item);
			}
			foreach (var item in unityEventKeys) {
				if (item.onKeyUp) {
					_keyUps.Add(item);
				}
				else {
					_keyCommands.Add(item);
				}
			}
			foreach (var item in gameObjectKeys) {
				_keyCommands.Add(item);
			}
			SortCommandsByModiferCount();
		}

		// Sort the key commands by modifier count, ensuring that the highest modifier count is checked first.
		private void SortCommandsByModiferCount()
		{
			_keyCommands.Sort();
			_keyUps.Sort();
		}

		private void Update()
		{
			KeyCommand.ClearEvaluatedKeysList();

			foreach (UnityEventKeyCommand keyUp in _keyUps) {
				if (Input.GetKeyUp(keyUp.key)) {
					if (!keyUp.ModifierQualified()) continue;
					if (logKeys) {
						Debug.Log(keyUp.ToString());
					}
					keyUp.Invoke();
				}
			}

			if (!Input.anyKeyDown) return;

			foreach (KeyCommand keyCommand in _keyCommands) {
				if (Input.GetKeyDown(keyCommand.key)) {
					if (!keyCommand.ModifierQualified()) continue;
					if (logKeys) {
						Debug.Log(keyCommand.ToString());
					}
					keyCommand.Invoke();
				}
			}
		}

		/// <summary>
		/// Change the current <see cref="FAST.KeyBinding.ActiveState"/>.
		/// </summary>
		/// <param name="newState">The name of the new <see cref="FAST.KeyBinding.ActiveState"/></param>
		public static void SetState(string newState)
		{
			previousState = currentState;
			currentState = newState;

			foreach (var keyBinding in keyBindings) {
				if (keyBinding.isAlwaysActive) {
					keyBinding.enabled = true;
					continue;
				}
				else if (keyBinding.activeStateNames.Contains(currentState)) {
					keyBinding.enabled = true;
					continue;
				}
				else {
					keyBinding.enabled = false;
				}
			}
		}

		/// <summary>
		/// Change back to the previous <see cref="FAST.KeyBinding.ActiveState"/>.
		/// </summary>
		public static void SetPreviousState()
		{
			SetState(previousState);
		}

		/// <summary>
		/// Change to the default <see cref="FAST.KeyBinding.ActiveState"/>.
		/// </summary>
		public static void SetDefaultState()
		{
			SetState(kDefaultState);
		}

		/// <summary>
		/// Allows you to add key press functionality at runtime.
		/// </summary>
		/// <param name="keyCommand">The key press funtionality to add.</param>
		public void AddKey(KeyCommand keyCommand)
		{
			if (keyCommand is UnityEventKeyCommand && ((UnityEventKeyCommand)keyCommand).onKeyUp) {
				_keyUps.Add((UnityEventKeyCommand)keyCommand);
			}
			else {
				_keyCommands.Add(keyCommand);
			}
			SortCommandsByModiferCount();
		}

		/// <summary>
		/// A base class for key presses and how they map to commands.
		/// </summary>
		[Serializable]
		public abstract class KeyCommand : IComparable<KeyCommand>
		{
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The name of the key press and command.
			/// </summary>
			public string name;

			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The primary key that triggers the command.
			/// </summary>
			public KeyCode key;

			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// Set to <see langword="true"/> if the Ctrl key needs to be pressed at the same time.
			/// </summary>
			[Header("Modifier Keys")]
			public bool ctrl;
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// Set to <see langword="true"/> if the Shift key needs to be pressed at the same time.
			/// </summary>
			public bool shift;
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// Set to <see langword="true"/> if the Alt key needs to be pressed at the same time.
			/// </summary>
			public bool alt;
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// Set to <see langword="true"/> if the Windows key needs to be pressed at the same time.
			/// </summary>
			public bool win;

			private static List<KeyCode> evaluatedKeys = new List<KeyCode>();
			public bool ModifierQualified()
			{
				if (evaluatedKeys.Contains(key)) return false;
				// If no modifiers are required, ignore all modifier states
				if (!ctrl && !shift && !alt && !win) return true;

				bool shiftState = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				bool ctrlState = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
				bool altState = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
				bool winState = Input.GetKey(KeyCode.LeftWindows) || Input.GetKey(KeyCode.RightWindows)
									|| Input.GetKey(KeyCode.LeftApple) || Input.GetKey(KeyCode.RightApple);

				// Return true if all states match requirements
				return shiftState == shift &&
						ctrlState == ctrl &&
						altState == alt &&
						winState == win;
			}

			// Clear the list of evaluated keys at the start of each frame
			public static void ClearEvaluatedKeysList()
			{
				if (evaluatedKeys == null) {
					evaluatedKeys = new List<KeyCode>();
				}
				evaluatedKeys.Clear();
			}

			public virtual void Invoke()
			{
				// Add this key to the already evaluated list, so it is not considered for the rest of the frame,
				// having been used by a higher priority command.
				evaluatedKeys.Add(key);
			}

			public int ModifierCount()
			{
				int count = 0;
				if (ctrl) count++;
				if (shift) count++;
				if (alt) count++;
				if (win) count++;
				return count;
			}

			// Sort by modifier count, descending. This way, the highest modifier count is checked first.
			public int CompareTo(KeyCommand otherCommand)
			{
				if (ModifierCount() == otherCommand.ModifierCount()) return 0;
				return (ModifierCount() > otherCommand.ModifierCount()) ? -1 : 1;
			}

			/// <summary>
			/// Format the key press and modifier keys as a combined <see langword="string"/>.
			/// </summary>
			/// <returns>The key combination as a <see langword="string"/>.</returns>
			/// <remarks>
			/// For example: "CTRL+SHIFT+A"
			/// </remarks>
			public override string ToString()
			{
				string id = string.Empty;
				if (ctrl) id += "CTRL+";
				if (shift) id += "SHIFT+";
				if (alt) id += "ALT+";
				if (win) id += "WIN+";
				id += Enum.GetName(typeof(KeyCode), key);
				return id;
			}
		}

		private enum StateType { Default, Custom }

		/// <summary>
		/// Defines an active key binding state.
		/// </summary>
		[Serializable]
		private class ActiveState
		{
			/// <summary>
			/// The name of the key binding state.
			/// </summary>
			[AllowNesting, ShowIf("type", StateType.Custom)]
			public string name;

			/// <summary>
			/// The type of key binding state.
			/// </summary>
			/// <remarks>
			/// There can only be one active Default state type per <see cref="FAST.KeyBinding"/>. 
			/// The name of a Default state type will always be "Default".
			/// </remarks>
			public StateType type;
		}

		/// <summary>
		/// Maps a key press to trigger a UI 
		/// <c style="color:DarkRed;"><see cref="Button.onClick()"/></c> event.
		/// </summary>
		[Serializable]
		public class ButtonKeyCommand : KeyCommand
		{
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The UI <c style="color:DarkRed;"><see cref="Button"/></c> to trigger.
			/// </summary>
			[Header("Invoke OnClick")]
			public UnityEngine.UI.Button button;

			/// <summary>
			/// Triggers the <c style="color:DarkRed;"><see cref="Button.onClick()"/></c> event.
			/// </summary>
			public override void Invoke()
			{
				if (button == null) return;
				base.Invoke();
				if (button != null && button.interactable) {
					button.onClick.Invoke();
				}
			}

			/// <summary>
			/// Format the key press, modifier keys, and <c style="color:DarkRed;"><see cref="Button"/></c> 
			/// name as a combined <see langword="string"/>.
			/// </summary>
			/// <returns>The key combination and <c style="color:DarkRed;"><see cref="Button"/></c> 
			/// name as a <see langword="string"/>.</returns>
			/// <remarks>
			/// For example: "CTRL+SHIFT+A: [Button] ButtonName"
			/// </remarks>
			public override string ToString()
			{
				return $"{base.ToString()}: [Button] {button.name}";
			}
		}

		/// <summary>
		/// Maps a key press to toggle a UI 
		/// <c style="color:DarkRed;"><see cref="Toggle"/></c> on/off.
		/// </summary>
		[Serializable]
		public class ToggleKeyCommand : KeyCommand
		{
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The UI <c style="color:DarkRed;"><see cref="Toggle"/></c> to toggle.
			/// </summary>
			[Header("Toggle On/Off")]
			public UnityEngine.UI.Toggle toggle;

			/// <summary>
			/// Toggles the <c style="color:DarkRed;"><see cref="Toggle"/></c> on/off.
			/// </summary>
			public override void Invoke()
			{
				if (toggle == null) return;
				base.Invoke();
				if (toggle != null && toggle.interactable) {
					toggle.isOn = !toggle.isOn;
				}
			}

			/// <summary>
			/// Format the key press, modifier keys, and <c style="color:DarkRed;"><see cref="Toggle"/></c> 
			/// name as a combined <see langword="string"/>.
			/// </summary>
			/// <returns>The key combination and <c style="color:DarkRed;"><see cref="Toggle"/></c> 
			/// name as a <see langword="string"/>.</returns>
			/// <remarks>
			/// For example: "CTRL+SHIFT+A: [Toggle] ToggleName"
			/// </remarks>
			public override string ToString()
			{
				return $"{base.ToString()}: [Toggle] {toggle.name}";
			}
		}

		/// <summary>
		/// Maps a key press to trigger a 
		/// <c style="color:DarkRed;"><see cref="UnityEvent"/></c>.
		/// </summary>
		[Serializable]
		public class UnityEventKeyCommand : KeyCommand
		{
			/// <summary>
			/// Set to <see langword="true"/> if the <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
			/// should be triggered on key up, instead of key down.
			/// </summary>
			public bool onKeyUp = false;

			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The <c style="color:DarkRed;"><see cref="UnityEvent"/></c> to trigger.
			/// </summary>
			[Header("Invoke Callbacks")]
			public UnityEvent unityEvent;

			/// <summary>
			/// Triggers the <c style="color:DarkRed;"><see cref="UnityEvent"/></c>.
			/// </summary>
			public override void Invoke()
			{
				if (unityEvent == null) return;
				base.Invoke();
				unityEvent.Invoke();
			}

			/// <summary>
			/// Format the key press, modifier keys, and <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
			/// name as a combined <see langword="string"/>.
			/// </summary>
			/// <returns>The key combination and <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
			/// name as a <see langword="string"/>.</returns>
			/// <remarks>
			/// For example: "CTRL+SHIFT+A: [Events]"<br/>
			///	"EventName1"<br/>
			///	"EventName2"
			/// </remarks>
			public override string ToString()
			{
				string result = $"{base.ToString()}: [Events";
				if (onKeyUp) result += " (On Key Up)";
				result += "]";
				int eventCount = unityEvent.GetPersistentEventCount();
				if (eventCount < 1) return result;
				result += $"\n";

				for (int i = 0; i < eventCount; i++) {
					result += $"{unityEvent.GetPersistentTarget(i)}.{unityEvent.GetPersistentMethodName(i)}";
					if (i < eventCount - 1) result += "\n";
				}
				return result;
			}
		}

		/// <summary>
		/// Maps a key press to toggle a  
		/// <c style="color:DarkRed;"><see cref="GameObject"/></c> active/inactive.
		/// </summary>
		[Serializable]
		public class GameObjectKeyCommand : KeyCommand
		{
			/// <summary>
			/// <b style="color: DarkCyan;">Inspector, Code</b><br/>
			/// The <c style="color:DarkRed;"><see cref="GameObject"/></c> to toggle.
			/// </summary>
			[Header("Toggle Active/Inactive")]
			public GameObject gameObject;

			/// <summary>
			/// Toggles the <c style="color:DarkRed;"><see cref="GameObject"/></c> active/inactive.
			/// </summary>
			public override void Invoke()
			{
				if (gameObject == null) return;
				base.Invoke();
				gameObject.SetActive(!gameObject.activeInHierarchy);
			}

			/// <summary>
			/// Format the key press, modifier keys, and <c style="color:DarkRed;"><see cref="GameObject"/></c> 
			/// name as a combined <see langword="string"/>.
			/// </summary>
			/// <returns>The key combination and <c style="color:DarkRed;"><see cref="GameObject"/></c> 
			/// name as a <see langword="string"/>.</returns>
			/// <remarks>
			/// For example: "CTRL+SHIFT+A: [GameObject] GameObjectName"
			/// </remarks>
			public override string ToString()
			{
				return $"{base.ToString()}: [GameObject] {gameObject.name}";
			}
		}
	}
}
