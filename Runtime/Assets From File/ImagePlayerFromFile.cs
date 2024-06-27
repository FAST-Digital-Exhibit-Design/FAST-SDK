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
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace FAST
{
    /// <summary>
    /// A class to help use an image sequence asset loaded from the file system as 
    /// <see cref="FAST.ImagePlayer.sprites"/> for the current skin in multiple languages.
    /// </summary>
    /// <inheritdoc cref="FAST.AssetFromFile" path="/remarks"/>
    /// @copydetails FAST.AssetFromFile
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Sprite.html">
    /// UnityEngine.Sprite</a>
    /// @see <a href="https://docs.unity3d.com/2018.4/Documentation/ScriptReference/UI.Image.html">
    /// UnityEngine.UI.Image</a>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings.html">
    /// UnityEditor.TextureImporterSettings</a>
    [RequireComponent(typeof(ImagePlayer))]
    public class ImagePlayerFromFile : AssetFromFile
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Runtime</b><br/>
        /// The <see cref="FAST.ImagePlayer"/> component that the image sequence asset is loaded into.
        /// </summary>
        /// <remarks>
        /// Retrieved at runtime from <see cref="FAST.ImagePlayerFromFile.Awake"/> if no 
        /// <see cref="FAST.ImagePlayer"/> is assigned in the Inspector.
        /// </remarks>
        [SerializeField]
        private ImagePlayer imagePlayer;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The image import settings to use when loading the image sequence asset as a 
        /// <c style="color:DarkRed;"><see cref="Sprite"/></c>[].
        /// </summary>
        [SerializeField,
         ContextMenuItem("Initialize Image and Sprite Settings", "SetImageSettings")]
        private ImageSettings imageSettings = new();

#if UNITY_EDITOR
        /// <summary>
        /// When you right-click on <see cref="FAST.AssetFromFile.baseFileName"/> in the Inspector, 
        /// this function will run in the Editor to get the filename of the 
        /// <c style="color:DarkRed;"><see cref="Image.sprite"/></c> assigned to the 
        /// <see cref="FAST.ImagePlayer.uiImage"/> of <see cref="FAST.ImagePlayerFromFile.imagePlayer"/>.
        /// </summary>
        override protected void SetNameWithFileExtension()
        {
            imagePlayer.uiImage = GetComponent<Image>();
            baseFileName = Path.GetFileName(AssetDatabase.GetAssetPath(imagePlayer.uiImage.sprite.GetInstanceID()));
            UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        }

        /// <summary>
        /// When you right-click on <see cref="FAST.ImagePlayerFromFile.imageSettings"/> in the Inspector, 
        /// this function will run in the Editor to get the 
        /// <c style="color:DarkRed;"><see cref="TextureImporterSettings"/></c> of 
        /// the <c style="color:DarkRed;"><see cref="Image.sprite"/></c> assigned to the 
        /// <see cref="FAST.ImagePlayer.uiImage"/> of <see cref="FAST.ImagePlayerFromFile.imagePlayer"/>.
        /// </summary>
        private void SetImageSettings()
        {
            imagePlayer.uiImage = GetComponent<Image>();

            if (imagePlayer.uiImage.sprite != null) {
                TextureImporter importer = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(imagePlayer.uiImage.sprite.GetInstanceID())) as TextureImporter;
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
        /// <remarks>
        /// Retrieves the <see cref="FAST.ImagePlayer"/> component at runtime if 
        /// <see cref="FAST.ImagePlayerFromFile.imagePlayer"/> isn't assigned in the Inspector at runtime.
        /// </remarks>
        override protected void Awake()
        {
            base.Awake();

            if (imagePlayer == null) {
                imagePlayer = GetComponent<ImagePlayer>();
            }
        }

        /// <inheritdoc/>
        /// @copydoc FAST.AssetFromFile.Load()
        /// <remarks>
        /// <see cref="FAST.ImageFromFile.imageSettings"/> are used to load the image sequence asset as a 
        /// <c style="color:DarkRed;"><see cref="Sprite"/></c>[] into <see cref="FAST.ImagePlayerFromFile.imagePlayer"/>. 
        /// The <see cref="FAST.ImagePlayer.uiImage"/> of <see cref="FAST.ImagePlayerFromFile.imagePlayer"/> is 
        /// also initialized with the first element of the <c style="color:DarkRed;"><see cref="Sprite"/></c>[].
        /// </remarks>
        override public void Load(string language)
        {
            if (isSharedByAllLanguages) {
                language = kSharedLanguage;
            }
            if (imagePlayer == null) {
                imagePlayer = GetComponent<ImagePlayer>();
            }

            var assets = Application.assets;
            bool isAssetAvailable = false;
            List<Sprite> sprites = new();
            int index = 0;
            int numDigits = baseFileName.Length - baseFileName.Replace("#", "").Length;
            string kIndexTag = new('#', numDigits);
            if (assets.ContainsKey(language)) {
                do {
                    UpdateFileName(language);
                    if (fileName.Contains("#")) {
                        string paddedIndex = index.ToString($"D{numDigits}");
                        fileName = fileName.Replace(kIndexTag, paddedIndex);
                        isAssetAvailable = assets[language].ContainsKey(fileName);
                        if (isAssetAvailable) {
                            Texture2D texture = assets[language][fileName] as Texture2D;
                            Sprite sprite = Sprite.Create(
                                    texture,
                                    new Rect(0, 0, texture.width, texture.height),
                                    imageSettings.spritePivot,
                                    imageSettings.spritePixelsPerUnit,
                                    imageSettings.spriteExtrude,
                                    imageSettings.spriteMeshType,
                                    imageSettings.spriteBorder);
                            sprite.name = fileName;
                            sprites.Add(sprite);

                            index++;
                        }
                    }
                } while (isAssetAvailable);
            }

            if (sprites.Count > 0) {
                imagePlayer.sprites = sprites.ToArray();
                imagePlayer.uiImage.sprite = sprites[0];

                if (imageSettings.loadImageAtNativeSize) {
                    imagePlayer.uiImage.SetNativeSize();
                }
                imagePlayer.uiImage.enabled = true;
            }
            else {
                imagePlayer.uiImage.sprite = null;
                imagePlayer.uiImage.enabled = false;
            }
        }
    }
}
