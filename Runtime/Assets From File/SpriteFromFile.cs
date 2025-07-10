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
using UnityEditor;

namespace FAST
{
    /// <summary>
    /// A class to help use an image asset loaded from the file system as 
    /// <c style="color:DarkRed;"><see cref="Sprite"/></c> 
    /// for the current skin in multiple languages.
    /// </summary>
    /// <inheritdoc cref="FAST.AssetFromFile" path="/remarks"/>
    /// @copydetails FAST.AssetFromFile
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Sprite.html">
    /// UnityEngine.Sprite</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings.html">
    /// UnityEditor.TextureImporterSettings</a>
    public class SpriteFromFile : AssetFromFile
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The <c style="color:DarkRed;"><see cref="Sprite"/></c> that the image asset is loaded into.
        /// </summary>
        /// <remarks>
        /// Assigned at runtime in <see cref="FAST.SpriteFromFile.Load(string)"/>.
        /// </remarks>
        public Sprite sprite;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The image import settings to use when loading the image asset as a 
        /// <c style="color:DarkRed;"><see cref="Sprite"/></c>.
        /// </summary>
        [SerializeField,
         ContextMenuItem("Initialize Image and Sprite Settings", "SetImageSettings")]
        private ImageSettings imageSettings = new();

#if UNITY_EDITOR
        /// <summary>
        /// When you right-click on <see cref="FAST.AssetFromFile.baseFileName"/> in the Inspector, 
        /// this function will run in the Editor to get the filename of the 
        /// <c style="color:DarkRed;"><see cref="Sprite"/></c> assigned to 
        /// <see cref="FAST.SpriteFromFile.sprite"/>.
        /// </summary>
        override protected void SetNameWithFileExtension()
        {
            if (sprite == null) {
                return;
            }

            baseFileName = Path.GetFileName(AssetDatabase.GetAssetPath(sprite.GetInstanceID()));
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        /// <summary>
        /// When you right-click on <see cref="FAST.SpriteFromFile.imageSettings"/> in the Inspector, 
        /// this function will run in the Editor to get the 
        /// <c style="color:DarkRed;"><see cref="TextureImporterSettings"/></c> of 
        /// the <c style="color:DarkRed;"><see cref="Sprite"/></c> assigned to <see cref="FAST.SpriteFromFile.sprite"/>.
        /// </summary>
        private void SetImageSettings()
        {
            if (sprite != null) {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(sprite.GetInstanceID())) as TextureImporter;
                TextureImporterSettings importSettings = new();
                importer.ReadTextureSettings(importSettings);
                imageSettings.spritePivot = importSettings.spritePivot;
                imageSettings.spritePixelsPerUnit = importSettings.spritePixelsPerUnit;
                imageSettings.spriteExtrude = importSettings.spriteExtrude;
                imageSettings.spriteMeshType = importSettings.spriteMeshType;
                imageSettings.spriteBorder = importSettings.spriteBorder;
            }
            else {
                imageSettings = new();
            }
        }
#endif

        /// <inheritdoc/>
        /// @copydoc FAST.AssetFromFile.Load()
        /// <remarks>
        /// <see cref="FAST.SpriteFromFile.imageSettings"/> are used to load the image asset as a 
        /// <c style="color:DarkRed;"><see cref="Sprite"/></c> into <see cref="FAST.SpriteFromFile.sprite"/>.
        /// </remarks>
        override public void Load(string language)
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
                Texture2D texture = assets[language][fileName] as Texture2D;
                sprite = Sprite.Create(
                        texture,
                        new Rect(0, 0, texture.width, texture.height),
                        imageSettings.spritePivot,
                        imageSettings.spritePixelsPerUnit,
                        imageSettings.spriteExtrude,
                        imageSettings.spriteMeshType,
                        imageSettings.spriteBorder);
                sprite.name = fileName;
            }
            else {
                sprite = null;
                LogAssetNotLoadedError();
            }
        }
    }
}
