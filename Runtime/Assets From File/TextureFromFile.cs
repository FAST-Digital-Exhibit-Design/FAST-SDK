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
    /// A class to help use an image asset loaded from the file system as 
    /// <c style="color:DarkRed;"><see cref="Texture2D"/></c> for the current skin in multiple languages.
    /// </summary>
    /// <inheritdoc cref="FAST.AssetFromFile" path="/remarks"/>
    /// @copydetails FAST.AssetFromFile
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Texture2D.html">
    /// UnityEngine.Texture2D</a>
    public class TextureFromFile : AssetFromFile
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The <c style="color:DarkRed;"><see cref="Texture2D"/></c> that the image asset is loaded into.
        /// </summary>
        /// <remarks>
        /// Assigned at runtime in <see cref="FAST.TextureFromFile.Load(string)"/>.
        /// </remarks>
        public Texture2D texture;

#if UNITY_EDITOR
        /// <summary>
        /// When you right-click on <see cref="FAST.AssetFromFile.baseFileName"/> in the Inspector, 
        /// this function will run in the Editor to get the filename of the 
        /// <c style="color:DarkRed;"><see cref="Texture2D"/></c> if it is assigned to 
        /// <see cref="FAST.TextureFromFile.texture"/>.
        /// </summary>
        protected override void SetNameWithFileExtension()
        {
            if (texture == null) {
                return;
            }

            baseFileName = Path.GetFileName(AssetDatabase.GetAssetPath(texture.GetInstanceID()));
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }
#endif
        /// <inheritdoc/>
        /// @copydoc FAST.AssetFromFile.Load()
        /// <remarks>
        /// The image asset is loaded as a <c style="color:DarkRed;"><see cref="Texture2D"/></c> 
        /// into <see cref="FAST.TextureFromFile.texture"/>.
        /// </remarks>
        public override void Load(string language)
        {
            if (isSharedByAllLanguages) {
                language = kSharedLanguage;
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
                texture = assets[language][fileName] as Texture2D;
                texture.name = fileName;
            }
            else {
                texture = null;
            }
        }
    }
}
