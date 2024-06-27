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
    /// A "template" class that represents a screen state that can be animated to show or hide itself.
    /// </summary>
    /// <remarks>
    /// Inherit from this class to define a base screen class with modified or additional functionality 
    /// specific to your activity. Then, inherit from that base screen class to define each screen in the 
    /// activity and the specifics needed to animate it.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/GameObject.html">
	/// UnityEngine.GameObject</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Coroutine.html">
	/// UnityEngine.Coroutine</a>
    public abstract class ScreenTemplate : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Code</b><br/>
        /// Returns <see langword="true"/> when the screen is playing its animation.
        /// </summary>
        public bool IsPlaying { get => _isPlaying; }

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// This attribute backs the <see cref="FAST.ScreenTemplate.IsPlaying"/> property.
        /// </summary>
        [SerializeField]
        protected bool _isPlaying = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// A reference to a <see cref="FAST.AudioPlayer"/> that will be used for 
        /// playback of sequential audio.
        /// </summary>
        [SerializeField]
        protected AudioPlayer audioPlayer;

        /// <summary>
        /// <b style="color: DarkCyan;">Code</b><br/>
        /// All the <see cref="FAST.AudioClipFromFile"/>s that are children of this 
        /// <c style="color:DarkRed;"><see cref="GameObject"/></c> and should be considered 
        /// audio available for this screen to use.
        /// </summary>
        protected Dictionary<string, AudioClipFromFile> audioLUT = new();

        /// <summary>
        /// Default behavior is to get all the <see cref="FAST.AudioClipFromFile"/>s 
        /// that are children of this <c style="color:DarkRed;"><see cref="GameObject"/></c> 
        /// and add them to <see cref="FAST.ScreenTemplate.audioLUT"/>.
        /// </summary>
        protected virtual void Awake()
        {
            AudioClipFromFile[] audioList = GetComponentsInChildren<AudioClipFromFile>();
            foreach (var item in audioList) {
                audioLUT.Add(item.name, item);
            }
        }
        /// <summary>
        /// Default behavior plays the screen.
        /// </summary>
        protected virtual void OnEnable()
        {
            PlayScreen();
        }

        /// <summary>
        /// Default behavior stops all <c style="color:DarkRed;"><see cref="Coroutine"/>s</c>, 
        /// which could be playing or clearing the screen.
        /// </summary>
        protected virtual void OnDisable()
        {
            if (audioPlayer != null) {
                if (audioPlayer.gameObject.activeInHierarchy) {
                    audioPlayer.Stop();
                }
                audioPlayer.ClearPlaylist();
            }
            StopAllCoroutines();
        }

        /// <summary>
        /// Default behavior stops all <c style="color:DarkRed;"><see cref="Coroutine"/>s</c>, 
        /// then starts a <c style="color:DarkRed;"><see cref="Coroutine"/></c> to play the screen.
        /// </summary>
        public virtual void PlayScreen()
        {
            StopAllCoroutines();
            StartCoroutine(PlayScreenCoroutine());
        }

        // <summary>
        /// Default behavior runs a <c style="color:DarkRed;"><see cref="Coroutine"/></c> 
        /// to animate playing the screen.
        /// </summary>
        protected virtual IEnumerator PlayScreenCoroutine()
        {
            _isPlaying = true;
            yield return PlayScreenAnimation();
            _isPlaying = false;
        }

        /// <summary>
        /// The <c style="color:DarkRed;"><see cref="Coroutine"/></c> you must define 
        /// to animate playing the screen.
        /// </summary>
        protected abstract IEnumerator PlayScreenAnimation();
    }
}
