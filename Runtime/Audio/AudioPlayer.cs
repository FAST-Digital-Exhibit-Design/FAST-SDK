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
    /// A class to play <c style="color:DarkRed;"><see cref="AudioClip"/>s</c> from a playlist.
    /// </summary>
    /// <remarks>
    /// This class was designed primiarily for narration, but it could be used for any type of audio.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AudioClip.html">
    /// UnityEngine.AudioClip</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AudioSource.html">
    /// UnityEngine.AudioSource</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Events.UnityEvent.html">
    /// UnityEngine.Events.UnityEvent</a>
    [RequireComponent(typeof(AudioSource))]
    public class AudioPlayer : MonoBehaviour
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The maximum volume of the <c style="color:DarkRed;"><see cref="AudioSource"/>s</c> associated 
        /// with this <c>AudioPlayer</c>.
        /// </summary>
        /// <remarks>The volume range is clamped to [0.0, 1.0]</remarks>
        public float MaxVolume
        { 
            get => _maxVolume; 
            set => _maxVolume = Mathf.Clamp01(value);
        }
        [SerializeField, Range(0f, 1f)]
        private float _maxVolume = 1f;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The array to select which channels to use. Set an element to 
        /// <see langword="true"/> if that index represents a channel to use.
        /// </summary>
        /// <remarks>
        /// Most commonly, set the array size to the number of audio channels used in the application.
        /// </remarks>
        [Space(10)]
        [Tooltip("Set the array size to the number of audio channels used in the activity, " +
            "then enable the channels you want this \"AudioPlayer\" to use.")]
        public bool[] targetChannels = new bool[2];

        /// <summary>
        /// Indicates the <c>AudioPlayer</c> is running a play or stop command if <see langword="true"/>.
        /// </summary>
        public bool IsRunning { get => _isRunning; }
        [SerializeField, Space(10)]
        private bool _isRunning = false;

        /// <summary>
        /// Indicates the amount of the playlist that has been played so far.
        /// </summary>
        /// <remarks>
        /// The progress range is normalized to [0.0, 1.0]
        /// </remarks>
        public float Progress { get => _progress; }
        [SerializeField, Range(0f, 1f)]
        private float _progress = 0f;

        /// <summary>
        /// Indicates the <c>AudioPlayer</c> playback is paused if <see langword="true"/>.
        /// </summary>
        public bool IsPaused { get => _isPaused; }
        [SerializeField]
        private bool _isPaused = false;

        /// <summary>
        /// Pauses the <c>AudioPlayer</c> playback by pausing the underlying <c style="color:DarkRed;"><see cref="AudioSource"/></c>.
        /// </summary>
        public void Pause()
        {
            audioSource.Pause();
            _isPaused = true;
        }

        /// <summary>
        /// Unpauses the <c>AudioPlayer</c> playback by unpausing the underlying <c style="color:DarkRed;"><see cref="AudioSource"/></c>.
        /// </summary>
        public void UnPause()
        {
            audioSource.UnPause();
            _isPaused = false;
        }

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Set to <see langword="true"/> to fade the volume as the <c>AudioPlayer</c> stops.
        /// </summary>
        [Space(10)]
        [Tooltip("Enable this to fade the volume as the player stops.")]
        public bool isFadeVolumeOnStop = false;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The volume fade duration in seconds when the <c>AudioPlayer</c> stops if <see cref="FAST.AudioPlayer.isFadeVolumeOnStop"/> 
        /// is set to <see langword="true"/>.
        /// </summary>
        [Tooltip("Specify the volume fade duration in seconds when the player stops, if \"isFadeVolumeOnStop\" is enabled.")]
        public float volumeFadeDuration = 0.5f;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when an <c style="color:DarkRed;"><see cref="AudioClip"/></c> starts playing.
        /// </summary>
        /// <remarks>
        /// The <see langword="string"/> parameter passed into the callback function(s) is the name of the 
        /// <c style="color:DarkRed;"><see cref="AudioClip"/></c> that started playing.
        /// </remarks>
        [Space(10)]
        [Tooltip("Specify the function(s) that should be called when an audio clip starts playing.")]
        public UnityEvent<string> OnClipStart;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when an <c style="color:DarkRed;"><see cref="AudioClip"/></c> stops playing before it reaches the end.
        /// </summary>
        /// <remarks>
        /// The <see langword="string"/> parameter passed into the callback function(s) is the name of the 
        /// <c style="color:DarkRed;"><see cref="AudioClip"/></c> that stopped playing.
        /// </remarks>
        [Tooltip("Specify the function(s) that should be called when an audio clip stops playing before the clip ends.")]
        public UnityEvent<string> OnClipStop;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// Calls the function(s) subscribed to this <c style="color:DarkRed;"><see cref="UnityEvent"/></c> 
        /// when an <c style="color:DarkRed;"><see cref="AudioClip"/></c> reaches the end.
        /// </summary>
        /// <remarks>
        /// The <see langword="string"/> parameter passed into the callback function(s) is the name of the 
        /// <c style="color:DarkRed;"><see cref="AudioClip"/></c> that reached the end.
        /// </remarks>
        [Tooltip("Specify the function(s) that should be called when an audio clip reaches the end.")]
        public UnityEvent<string> OnClipEnd;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The list of <c style="color:DarkRed;"><see cref="AudioClip"/>s</c> that are playing or queued to play 
        /// when the <c>AudioPlayer</c> begins playing.
        /// </summary>
        /// <remarks>
        /// This attribute is primarily to make the playlist visible in the Inspector at runtime. Use the 
        /// <c>AudioPlayer</c> functions to modify the playlist.
        /// </remarks>
        [SerializeField, Space(10)]
        private List<AudioClip> playlist = new();
        private float playlistDuration = 0f;

        private AudioSource audioSource;

        private enum AudioPlayerCommand { Stop, Play }

        private void Awake()
        {
            audioSource = gameObject.GetComponent<AudioSource>();
            playlist.Clear();
            playlistDuration = 0;
        }

        /// <summary>
        /// Start playing the playlist from the beginning.
        /// </summary>
        /// <remarks>
        /// If the <c>AudioPlayer</c> is already playing, it will restart the playlist.
        /// </remarks>
        public void Play()
        {
            StopAllCoroutines();
            StartCoroutine(PlayerCoroutine(AudioPlayerCommand.Play));
        }

        /// <summary>
        /// Change the playlist and start playing it from the beginning.
        /// </summary>
        /// <remarks>
        /// If the <c>AudioPlayer</c> is already playing, it will restart the playlist.
        /// </remarks>
        /// <param name="audioClips">The new playlist of <c style="color:DarkRed;"><see cref="AudioClip"/>s</c>.</param>
        public void Play(AudioClip[] audioClips)
        {
            playlist.Clear();
            playlistDuration = 0;
            playlist = new List<AudioClip>(audioClips);
            Play();
        }

        /// <summary>
        /// Stop playing audio. This does not pause the playlist.
        /// </summary>
        /// <remarks>
        /// If you play after calling <c>Stop()</c>, the playlist will restart from the beginning.
        /// </remarks>
        public void Stop()
        {
            StopAllCoroutines();
            StartCoroutine(PlayerCoroutine(AudioPlayerCommand.Stop));
        }

        /// <summary>
        /// Remove all the clips from the playlist. 
        /// </summary>
        /// <remarks>
        /// If an <c style="color:DarkRed;"><see cref="AudioClip"/></c> is currently playing, 
        /// it will finish and then the <c>AudioPlayer</c> will stop.
        /// </remarks>
        public void ClearPlaylist()
        {
            playlist.Clear();
            playlistDuration = 0;
        }

        /// <summary>
        /// Add an <c style="color:DarkRed;"><see cref="AudioClip"/></c> to the end of the playlist. 
        /// </summary>
        /// <remarks>
        /// This does not interrupt the <c>AudioPlayer</c>.
        /// </remarks>
        /// <param name="audioClip">The <c style="color:DarkRed;"><see cref="AudioClip"/></c> to add to the playlist.</param>
        public void AddToPlaylist(AudioClip audioClip)
        {
            playlist.Add(audioClip);
            playlistDuration += audioClip.length;
        }

        /// <summary>
        /// Change the playlist and stop the player.
        /// </summary>
        /// <param name="audioClips">The new playlist of <c style="color:DarkRed;"><see cref="AudioClip"/>s</c>.</param>
        public void NewPlaylist(AudioClip[] audioClips)
        {
            playlist.Clear();
            playlistDuration = 0;
            playlist = new List<AudioClip>(audioClips);
            Stop();
        }

        private IEnumerator PlayerCoroutine(AudioPlayerCommand command)
        {
            _isRunning = true;
            yield return StopPlayer();

            if (command.Equals(AudioPlayerCommand.Play)) {
                yield return StartPlayer();
            }
            _isRunning = false;
        }

        private IEnumerator StartPlayer()
        {
            _progress = 0f;

            audioSource.volume = _maxVolume;

            playlistDuration = 0;
            for (int i = 0; i < playlist.Count; i++) {
                AudioClip audioClip = playlist[i];
                playlistDuration += audioClip.length;
            }

            StartCoroutine(TrackPlayerProgress());

            for (int i = 0;  i < playlist.Count; i++) {
                AudioClip audioClip = playlist[i];
                audioSource.clip = audioClip.CreateChannelClip(targetChannels);
                Debug.Log(audioSource.clip.name);

                OnClipStart.Invoke(audioSource.clip.name);
                audioSource.Play();
                yield return new WaitWhile(() => audioSource.isPlaying || _isPaused);

                OnClipEnd.Invoke(audioSource.clip.name);
                Destroy(audioSource.clip);
            }
            audioSource.clip = null;
        }

        private IEnumerator StopPlayer()
        {
            if (!audioSource.isPlaying) {
                _progress = 0f;
                yield return null;
            }

            if (isFadeVolumeOnStop) {
                yield return FadeVolume(audioSource.volume, 0f, volumeFadeDuration);
            }

            audioSource.Stop();

            if (audioSource.clip != null) {
                OnClipStop.Invoke(audioSource.clip.name);
                Destroy(audioSource.clip);
                audioSource.clip = null;
            }

            _progress = 0f;
        }

        private IEnumerator FadeVolume(float startVolume, float endVolume, float durationSeconds)
        {
            float elapsedTime = 0f;
            while (elapsedTime < durationSeconds) {
                elapsedTime += Time.deltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, endVolume, elapsedTime / durationSeconds);
                yield return null;
            }
            audioSource.volume = endVolume;
        }

        private IEnumerator TrackPlayerProgress()
        {
            float time = 0f;
            while (time < playlistDuration) {
                if (!_isPaused) {
                    time += Time.deltaTime;
                }
                _progress = time / playlistDuration;
                yield return null;
            }
            _progress = 1f;
        }
    }
}
