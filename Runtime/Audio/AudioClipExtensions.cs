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
    /// A class to extend the functionality of an <c style="color:DarkRed;"><see cref="AudioClip"/></c>.
    /// </summary>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AudioClip.html">
    /// UnityEngine.AudioClip</a>
    public static class AudioClipExtensions
    {
        /// <summary>
        /// Create a new multi-channel <c style="color:DarkRed;"><see cref="AudioClip"/></c> from the current 
        /// <c style="color:DarkRed;"><see cref="AudioClip"/></c>. 
        /// The new clip can have audio on specifc channels so that it plays through specific speaker(s).
        /// </summary>
        /// <param name="originalClip">The current <c style="color:DarkRed;"><see cref="AudioClip"/></c> 
        /// to create a new multi-channel clip from.</param>
        /// <param name="targetChannels">The array to select which channels to use. Set an element to 
        /// <see langword="true"/> if that index represents a channel to use.</param>
        /// <returns>A new multi-channel <c style="color:DarkRed;"><see cref="AudioClip"/></c>.</returns>
        /// @see This is a modified version of the code shared by 
        /// <a href="https://forum.unity.com/threads/sending-sounds-directly-to-speakers.77978/#post-2673422">
        /// Thomas-Mountainborn on Unity Forums on June 10, 2016</a>
        /// 
        public static AudioClip CreateChannelClip(this AudioClip originalClip, bool[] targetChannels)
        {
            int numberOfChannels = targetChannels.Length;
            if (numberOfChannels == 0) {
                return null;
            }

            // Create a new clip with the target amount of channels.
            AudioClip clip = AudioClip.Create(originalClip.name, originalClip.samples, numberOfChannels, originalClip.frequency, false);
            // Init audio arrays.
            float[] audioData = new float[originalClip.samples * numberOfChannels];
            float[] originalAudioData = new float[originalClip.samples * originalClip.channels];
            if (!originalClip.GetData(originalAudioData, 0)) {
                return null;
            }

            // Fill in the audio from the original clip into the target channel. Samples are interleaved by channel (L0, R0, L1, R1, etc).
            int originalClipIndex = 0;
            for (int i = 0; i < audioData.Length;) {
                for (int channel = 0; channel < targetChannels.Length; channel++) {
                    if (targetChannels[channel] == true) {
                        audioData[i] = originalAudioData[originalClipIndex];
                    }
                    i++;
                }
                originalClipIndex += originalClip.channels;
            }

            if (!clip.SetData(audioData, 0)) {
                return null;
            }

            return clip;
        }

        /// <summary>
        /// Create an empty <c style="color:DarkRed;"><see cref="AudioClip"/></c>, which could be use for 
        /// pausing before or after playing another <c style="color:DarkRed;"><see cref="AudioClip"/></c>.
        /// </summary>
        /// <param name="duration">The amount of time in seconds that the clip should play for.</param>
        /// <returns>A new <c style="color:DarkRed;"><see cref="AudioClip"/></c>.</returns>
        public static AudioClip CreatePauseClip(float duration)
        {
           return AudioClip.Create($"pause {duration:0.##} seconds", Mathf.CeilToInt(duration * 44100), 1, 44100, false);
        }
    }
}
