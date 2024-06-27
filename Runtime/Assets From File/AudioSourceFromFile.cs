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
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace FAST
{
    /// <summary>
    /// A class to help use an audio asset loaded from the file system as 
    /// <c style="color:DarkRed;"><see cref="AudioSource.clip"/></c> 
    /// for the current skin in multiple languages.
    /// </summary>
    /// <inheritdoc cref="FAST.AssetFromFile" path="/remarks"/>
    /// @copydetails FAST.AssetFromFile
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AudioSource.html">
    /// UnityEngine.AudioSource</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/AudioClip.html">
    /// UnityEngine.AudioClip</a>
    [RequireComponent(typeof(AudioSource))]
    public class AudioSourceFromFile : AssetFromFile
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The <c style="color:DarkRed;"><see cref="AudioSource"/></c> 
        /// component that the audio asset is loaded into.
        /// </summary>
        /// <remarks>
        /// Retrieved at runtime from <see cref="FAST.AudioSourceFromFile.Awake"/> if no 
        /// <c style="color:DarkRed;"><see cref="AudioSource"/></c> is assigned in the Inspector.
        /// </remarks>
        [SerializeField]
        private AudioSource audioSource;

#if UNITY_EDITOR
        /// <summary>
        /// When you right-click on <see cref="FAST.AssetFromFile.baseFileName"/> in the Inspector, 
        /// this function will run in the Editor to get the filename of the 
        /// <c style="color:DarkRed;"><see cref="AudioSource.clip"/></c> 
        /// assigned to <see cref="FAST.AudioSourceFromFile.audioSource"/>.
        /// </summary>
        override protected void SetNameWithFileExtension()
        {
            audioSource = GetComponent<AudioSource>();
            baseFileName = Path.GetFileName(AssetDatabase.GetAssetPath(audioSource.clip.GetInstanceID()));
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif

        /// <inheritdoc/>
        /// <remarks>
        /// Retrieves the <c style="color:DarkRed;"><see cref="AudioSource"/></c> component at runtime if 
        /// <see cref="FAST.AudioSourceFromFile.audioSource"/> isn't assigned in the Inspector at runtime.
        /// </remarks>
        override protected void Awake()
        {
            base.Awake();
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }
        }

        /// <inheritdoc/>
        /// @copydoc FAST.AssetFromFile.Load()
        /// <remarks>
        /// The audio asset is loaded as a <c style="color:DarkRed;"><see cref="AudioClip"/></c> into 
        /// <see cref="FAST.AudioSourceFromFile.audioSource"/>.
        /// </remarks>
        override public void Load(string language)
        {
            if (isSharedByAllLanguages) {
                language = kSharedLanguage;
            }
            if (audioSource == null) {
                audioSource = GetComponent<AudioSource>();
            }

            var assets = Application.assets;
            bool isAssetAvailable = false;
            if (assets.ContainsKey(language)) {
                UpdateFileName(language);
                if (assets[language].ContainsKey(fileName)) {
                    isAssetAvailable = true;
                }
            }
            if (isAssetAvailable) {
                AudioClip audioClip = assets[language][fileName] as AudioClip;
                audioClip.name = fileName;

                audioSource.clip = audioClip;
                audioSource.enabled = true;
            }
            else {
                audioSource.clip = null;
                audioSource.enabled = false;
            }
        }
    }
}
