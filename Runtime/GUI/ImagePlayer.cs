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
using UnityEngine.UI;

namespace FAST
{
    /// <summary>
    /// A class to play an image sequence of <c style="color:DarkRed;"><see cref="Sprite"/>s</c> similar to a GIF.
    /// </summary>
    /// <remarks>
    /// The image  is rendered to the UI as an <c style="color:DarkRed;"><see cref="Image"/></c>.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Sprite.html">
    /// UnityEngine.Sprite</a>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    [RequireComponent(typeof(Image))]
    public class ImagePlayer : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The <c style="color:DarkRed;"><see cref="Image"/></c> component that will render the current frame 
        /// of the image sequence.
        /// </summary>
        /// <remarks>
        /// Retrieved at runtime from <see cref="FAST.ImagePlayer.Awake"/> and viewable in the Inspector. 
        /// You can modify the assignment in code, but it is not recommended.
        /// </remarks>
        public Image uiImage;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The framerate that the image sequence will be played at, measured in frames per second (fps).
        /// </summary>
        /// <remarks>
        /// The minimum framerate is 0.1fps.
        /// </remarks>
        [Space(10)]
        [SerializeField, Min(0.1f)]
        private float frameRate = 1;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the images sequence should start playing on 
        /// <c style="color:DarkRed;"><see cref="Awake()"/></c> or <c style="color:DarkRed;"><see cref="OnEnable()"/></c>.
        /// </summary>
        [SerializeField]
        private bool playOnAwake = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the last frame should be held when the image 
        /// sequence finishes playing. Otherwise the <c style="color:DarkRed;"><see cref="Image"/>.color</c>
        /// will be set to clear/transparent.
        /// </summary>
        [SerializeField]
        private bool holdLastFrame = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the image sequence should play in a loop, 
        /// otherwise it is played once.
        /// </summary>
        [SerializeField]
        private bool loopPlayback = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if frames should be skipped to catch up with the current time. 
        /// </summary>
        /// <remarks>
        /// Skipping frames will maintain the duration of the image sequence.
        /// </remarks>
        [SerializeField]
        private bool skipOnDrop = true;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The index of the current frame being played. 
        /// </summary>
        [Space(10)]
        [SerializeField]
        private int currentFrame;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The total number of frames in the image sequence. 
        /// </summary>
        [SerializeField]
        private int frameCount;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The duration of one frame of the image sequence in seconds. 
        /// </summary>
        /// <remarks>
        /// Depends on the <see cref="FAST.ImagePlayer.frameRate"/>.
        /// </remarks>
        [SerializeField]
        private float frameTime;

        //[SerializeField]
        //private float startTime;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The elapsed play time of the image sequence in seconds. 
        /// </summary>
        [SerializeField]
        private float currentTime;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The duration of the image sequence in seconds. 
        /// </summary>
        /// <remarks>
        /// Depends on the <see cref="FAST.ImagePlayer.frameCount"/> and <see cref="FAST.ImagePlayer.frameTime"/>.
        /// </remarks>
        [SerializeField]
        private float totalTime;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// Indicates the image squence is playing if <see langword="true"/>. 
        /// </summary>
        [SerializeField]
        private bool isPlaying;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// Indicates the image squence is paused if <see langword="true"/>. 
        /// </summary>
        [SerializeField]
        private bool isPaused;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The <c style="color:DarkRed;"><see cref="Sprite"/>s</c> that the image sequence is loaded into. 
        /// </summary>
        /// <remarks>
        /// Use <see cref="FAST.ImagePlayerFromFile"/> to load these <c style="color:DarkRed;"><see cref="Sprite"/>s</c> 
        /// at automatically runtime.
        /// </remarks>
        [Space(10)]
        public Sprite[] sprites;

        private Coroutine playbackCoroutine;

        private void Awake()
        {
            uiImage = GetComponent<Image>();

            isPlaying = false;
            isPaused = false;

            frameCount = sprites.Length;
            frameTime = 1f / frameRate;
            totalTime = frameCount * frameTime;

            currentFrame = 0;
            currentTime = 0f;
        }

        private void OnEnable()
        {
            if (playOnAwake) {
                Play();
            }
        }

        /// <summary>
        /// Starts playing the image sequence. 
        /// </summary>
        public void Play()
        {
            if (!isPaused) {
                Stop();

                frameCount = sprites.Length;
                frameTime = 1f / frameRate;
                totalTime = frameCount * frameTime;
                if (frameCount > 0) {
                    playbackCoroutine = StartCoroutine(PlayImages());
                }
            }
            else {
                isPaused = false;
            }
        }
        private IEnumerator PlayImages()
        {
            isPlaying = true;
            do {
                currentFrame = 0;
                currentTime = 0;
                uiImage.color = Color.white;
                while (currentTime < totalTime) {
                    if (!isPaused) {
                        currentTime += Time.deltaTime;
                        if (currentTime > frameTime * (currentFrame + 1)) {
                            if (skipOnDrop) {
                                currentFrame = Mathf.Min(Mathf.FloorToInt(currentTime / frameTime), frameCount - 1);
                            }
                            else {
                                currentFrame = Mathf.Min(currentFrame + 1, frameCount - 1);
                            }
                        }
                        uiImage.sprite = sprites[currentFrame];
                    }
                    yield return null;
                }
            } while (loopPlayback);

            if (!holdLastFrame) {
                uiImage.color = Color.clear;
            }

            isPlaying = false;
        }

        /// <summary>
        /// Pauses playback of the image sequence and keeps the current time. 
        /// </summary>
        public void Pause()
        {
            isPaused = true;
        }

        /// <summary>
        /// Stops playback of the image sequence and resets the current time to 0. 
        /// </summary>
        public void Stop()
        {
            if (playbackCoroutine != null) {
                StopCoroutine(playbackCoroutine);
            }
            
            isPlaying = false;
            isPaused = false;

            currentFrame = 0;
            currentTime = 0f;
            if (frameCount > 0) {
                uiImage.sprite = sprites[currentFrame];
            }

            if (!holdLastFrame) {
                uiImage.color = Color.clear;
            }
        }
    }
}
