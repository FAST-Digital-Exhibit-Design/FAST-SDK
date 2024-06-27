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
    /// A base class used to manage screen states, respond to interaction, 
    /// change the screen, or change the language.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to define a screen manager class with modified or additional 
    /// functionality specific to your activity.
    /// </remarks>
    /// <typeparam name="T">The screen base class that you define and inherits from 
    /// <see cref="FAST.ScreenTemplate"/>.</typeparam>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
	/// UnityEngine.GameObject</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Coroutine.html">
	/// UnityEngine.Coroutine</a>
    public abstract class ScreenManagerTemplate<T> : MonoBehaviour where T : ScreenTemplate
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// A reference to a <see cref="FAST.AudioPlayer"/> that will be used for 
        /// playback of sequential audio that isn't specific to any screen.
        /// </summary>
        [SerializeField]
        protected AudioPlayer audioPlayer;

        /// <summary>
        /// <b style="color: DarkCyan;">Code</b><br/>
        /// All the <see cref="FAST.AudioClipFromFile"/>s that are children of this 
        /// <c style="color:DarkRed;"><see cref="GameObject"/></c> and should be considered 
        /// audio available to use that isn't specific to any screen.
        /// </summary>
        protected Dictionary<string, AudioClipFromFile> audioLUT = new();

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The name of the current, active screen state.
        /// </summary>
        /// <remarks>
        /// This is set to the first screen in the <see cref="FAST.ScreenManagerTemplate{T}.screensList"/> 
        /// on <see cref="FAST.ScreenManagerTemplate{T}.Awake()"/>.
        /// </remarks>
        [SerializeField,
         Tooltip("This is set to the first screen in the Screens List on Awake().")]
        protected string currentScreenName;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// A list of all the screen states as <see cref="FAST.ScreenTemplate"/>s.
        /// </summary>
        /// <remarks>
        /// This list exists to serialize/deserialize <see cref="FAST.ScreenManagerTemplate{T}.screens"/> 
        /// so it can be edited in the Inspector. Also, the first screen in the list is set as the 
        /// <see cref="FAST.ScreenManagerTemplate{T}.currentScreenName"/> on <see cref="FAST.ScreenManagerTemplate{T}.Awake()"/>.
        /// </remarks>
        [SerializeField,
         Tooltip("The first screen in the list is set as the Current Screen Name on Awake().")]
        protected NamedObject<T>[] screensList;

        /// <summary>
        /// The <see cref="FAST.ScreenManagerTemplate{T}.screensList"/> converted to a 
        /// <c style="color:DarkRed;"><see cref="Dictionary{TKey, TValue}"/></c> for easier lookup and use.
        /// </summary>
        protected Dictionary<string, T> screens = new();

        /// <summary>
        /// Default behavior is to copy the <see cref="FAST.ScreenManagerTemplate{T}.screensList"/> 
        /// to <see cref="FAST.ScreenManagerTemplate{T}.screens"/>, set the 
        /// <see cref="FAST.ScreenManagerTemplate{T}.currentScreenName"/>, and get all the 
        /// <see cref="FAST.AudioClipFromFile"/>s that are children of this 
        /// <c style="color:DarkRed;"><see cref="GameObject"/></c> and add them to 
        /// <see cref="FAST.ScreenTemplate.audioLUT"/>.
        /// </summary>
        protected virtual void Awake()
        {
            foreach (var item in screensList) {
                if (item.namedObject != null) {
                    screens.Add(item.name, item.namedObject);
                }
            }
            if (screensList.Length > 0) {
                currentScreenName = screensList[0].name;
            }

            AudioClipFromFile[] audioList = GetComponentsInChildren<AudioClipFromFile>();
            foreach (var item in audioList) {
                audioLUT.Add(item.name, item);
            }
        }

        /// <summary>
        /// Default behavior sets the current screen to <see cref="FAST.ScreenManagerTemplate{T}.currentScreenName"/>, 
        /// which is the first screen in the <see cref="FAST.ScreenManagerTemplate{T}.screensList"/>.
        /// </summary>
        protected virtual void Start()
        {
            ChangeScreen(currentScreenName);
        }

        /// <summary>
        /// Default behavior calls <see cref="FAST.ScreenManagerTemplate{T}.Start()"/>.
        /// </summary>
        public virtual void OnRestart()
        {
            Start();
        }

        /// <summary>
        /// Default behavior stops all <c style="color:DarkRed;"><see cref="Coroutine"/>s</c> and 
        /// starts the <see cref="FAST.ScreenManagerTemplate{T}.ChangeLanguage()"/> 
        /// <c style="color:DarkRed;"><see cref="Coroutine"/></c>.
        /// </summary>
        public virtual void OnLanguageChange()
        {
            StopAllCoroutines();
            StartCoroutine(ChangeLanguage());
        }

        /// <summary>
        /// Default behavior changes screens by setting the old screen's 
        /// <c style="color:DarkRed;"><see cref="GameObject"/></c> inactive and the new screen's 
        /// <c style="color:DarkRed;"><see cref="GameObject"/></c> active.
        /// </summary>
        /// <param name="newScreenName">The name of the new screen.</param>
        public virtual void ChangeScreen(string newScreenName)
        {
            if (currentScreenName != null && screens.ContainsKey(currentScreenName)) {
                screens[currentScreenName].gameObject.SetActive(false);
            }

            currentScreenName = newScreenName;
            if (currentScreenName != null && screens.ContainsKey(currentScreenName)) {
                screens[currentScreenName].gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// Default behavior clears the current screen, changes to the next language, 
        /// loads the new language assets, and then plays the current screen.
        /// </summary>
        protected virtual IEnumerator ChangeLanguage()
        {
            if (currentScreenName != null && screens.ContainsKey(currentScreenName)) {
                screens[currentScreenName].gameObject.SetActive(false);
            }
            Application.ChangeLanguage(Application.ChangeLanguageMode.Next);

            yield return null;

            if (currentScreenName != null && screens.ContainsKey(currentScreenName)) {
                screens[currentScreenName].gameObject.SetActive(true);
            }
        }
    }
}
