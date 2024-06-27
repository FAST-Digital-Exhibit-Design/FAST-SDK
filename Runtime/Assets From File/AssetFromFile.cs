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
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

namespace FAST
{
    /// <summary>
    /// A base class to help use an asset loaded from the file system for the current skin in multiple languages.
    /// </summary>
    /// <remarks>
    /// Assets are stored in <see cref="FAST.Application.assets"/> dictionary. This class will load the asset 
    /// by looking it up in the dictionary using <c>language</c> in <see cref="FAST.AssetFromFile.Load(string)"/> 
    /// and <see cref="FAST.AssetFromFile.fileName"/>.
    /// </remarks>
    /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application.html">UnityEngine.Application</a>
    abstract public class AssetFromFile : MonoBehaviour
    {
        /// <summary>
        /// A class for storing the image file import settings used by the Unity Editor. It ensures that the image settings 
        /// used when loading from file at runtime will match the import settings used by the Editor.
        /// </summary>
        /// <remarks>Use an instance of this class when deriving <see cref="FAST.AssetFromFile"/> classes that use an image asset.</remarks>
        /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings.html">
        /// UnityEditor.TextureImporterSettings</a>
        [System.Serializable]
        protected class ImageSettings
        {
            /// <summary>
            /// Set to true if the image should be loaded at the native size.
            /// </summary>
            [Header("Requires manual assignment")]
            public bool loadImageAtNativeSize = true;

            /// <summary>
            /// Pivot point of the Sprite relative to its graphic's rectangle. 
            /// Same as <c style="color:DarkRed;"><see cref="TextureImporterSettings.spritePivot"/></c>.
            /// </summary>
            /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings-spritePivot.html">
            /// UnityEditor.TextureImporterSettings.spritePivot</a>
            [Header("Assigned from context menu")]
            public Vector2 spritePivot = new(0.5f, 0.5f);

            /// <summary>
            /// The number of pixels in the sprite that correspond to one unit in world space. 
            /// Same as <c style="color:DarkRed;"><see cref="TextureImporterSettings.spritePixelsPerUnit"/></c>.
            /// </summary>
            /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings-spritePixelsPerUnit.html">
            /// UnityEditor.TextureImporterSettings.spritePixelsPerUnit</a>
            public float spritePixelsPerUnit = 100;

            /// <summary>
            /// The number of blank pixels to leave between the edge of the graphic and the mesh.  
            /// Same as <c style="color:DarkRed;"><see cref="TextureImporterSettings.spriteExtrude"/></c>.
            /// </summary>
            /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings-spriteExtrude.html">
            /// UnityEditor.TextureImporterSettings.spriteExtrude</a>
            public uint spriteExtrude = 1;

            /// <summary>
            /// The type of <c>Mesh</c> that <c>TextureImporter</c> generates for a <c>Sprite</c>.  
            /// Same as <c style="color:DarkRed;"><see cref="TextureImporterSettings.spriteMeshType"/></c>.
            /// </summary>
            /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings-spriteMeshType.html">
            /// UnityEditor.TextureImporterSettings.spriteMeshType</a>
            public SpriteMeshType spriteMeshType = SpriteMeshType.Tight;

            /// <summary>
            /// Border sizes of the generated sprites. 
            /// Same as <c style="color:DarkRed;"><see cref="TextureImporterSettings.spriteBorder"/></c>.
            /// </summary>
            /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/TextureImporterSettings-spriteBorder.html">
            /// UnityEditor.TextureImporterSettings.spriteBorder</a>
            public Vector4 spriteBorder = Vector4.zero;
        }
        
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector, Code</b><br/>
        /// The name of the asset with the file extension, but without the prefix or suffix.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example: if the full filename is <c>"FAST-QuizShow-Mars-EN-Player1-Join.png"</c> then the
        /// <c>baseFilename</c> would be <c>"Player1-Join.png"</c>.
        /// </para>
        /// </remarks>
        [Tooltip("The name of the file with extension, but without prefix or suffix"),
            ContextMenuItem("Set Name With File Extension", "SetNameWithFileExtension")]
        public string baseFileName;

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to true if this asset is the same across all languages and can be shared.
        /// </summary>
        [SerializeField]
        protected bool isSharedByAllLanguages;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings</b><br/>
        /// A string loaded from <see cref="FAST.AssetSettings"/> that will be added to the front of the 
        /// <see cref="FAST.AssetFromFile.baseFileName"/> at runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example: if the full filename is <c>"FAST-QuizShow-Mars-EN-Player1-Join.png"</c> then the
        /// <c>prefixFileName</c> would be <c>"FAST-QuizShow-Mars-EN-"</c>. When using hastags, the <c>prefixFileName</c>
        /// might be specified as <c>"#ProductName-#Skin-#Language-"</c>.
        /// </para>
        /// <para>
        /// See <see cref="FAST.AssetFromFile.kProductNameTag"/>,
        /// <see cref="FAST.AssetFromFile.kSkinTag"/>,
        /// <see cref="FAST.AssetFromFile.kLanguageTag"/>
        /// for more info about hashtags.
        /// </para>
        /// </remarks>
        [Header("Loaded from AssetSettings")]
        [SerializeField, Tooltip("Added to the front of the base file name")]
        protected string prefixFileName;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings</b><br/>
        /// A string loaded from <see cref="FAST.AssetSettings"/> that will be inserted to the end of the 
        /// <see cref="FAST.AssetFromFile.baseFileName"/>, but before the extension, at runtime.
        /// </summary>
        /// <remarks>
        /// <para>
        /// For example: if the full filename is <c>"Player1-Join-FAST-QuizShow-Mars-EN.png"</c> then the
        /// <c>suffixFileName</c> would be <c>"-FAST-QuizShow-Mars-EN"</c>. When using hastags, the <c>suffixFileName</c>
        /// might be specified as <c>"-#ProductName-#Skin-#Language"</c>.
        /// </para>
        /// <para>
        /// See <see cref="FAST.AssetFromFile.kProductNameTag"/>,
        /// <see cref="FAST.AssetFromFile.kSkinTag"/>,
        /// <see cref="FAST.AssetFromFile.kLanguageTag"/>
        /// for more info about hashtags.
        /// </para>
        /// </remarks>
        [SerializeField, Tooltip("Inserted to the end of the base file name, before the extension")]
        protected string suffixFileName;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The full file name combined from the <see cref="FAST.AssetFromFile.prefixFileName"/>, 
        /// <see cref="FAST.AssetFromFile.baseFileName"/>, and <see cref="FAST.AssetFromFile.suffixFileName"/> at runtime.
        /// </summary>
        [Header("Assigned at runtime")]
        [SerializeField, Tooltip("The full file name combined from the prefix, base, and suffix file names")]
        protected string fileName;

        /// <summary>
        /// Use this string as the language name for assets that are the same across all languages and can be shared.
        /// </summary>
        public const string kSharedLanguage = "Shared";

        /// <summary>
        /// Use this string in the <see cref="FAST.AssetSettings.prefixFileName"/> or 
        /// <see cref="FAST.AssetSettings.suffixFileName"/> to be replaced with the 
        /// <c style="color:DarkRed;"><see cref="UnityEngine.Application.productName"/></c> at runtime.
        /// </summary>
        /// @see <a href="https://docs.unity3d.com/2022.3/Documentation/ScriptReference/Application-productName.html">
        /// UnityEngine.Application.productName</a>
        protected const string kProductNameTag = "#ProductName";

        /// <summary>
        /// Use this string in the <see cref="FAST.AssetSettings.prefixFileName"/> or 
        /// <see cref="FAST.AssetSettings.suffixFileName"/> to be replaced 
        /// with the <see cref="FAST.AssetSettings.skin"/> at runtime.
        /// </summary>
        protected const string kSkinTag = "#Skin";

        /// <summary>
        /// Use this string in the <see cref="FAST.AssetSettings.prefixFileName"/> or 
        /// <see cref="FAST.AssetSettings.suffixFileName"/> to be replaced 
        /// with the <see cref="FAST.Application.language"/> at runtime.
        /// </summary>
        protected const string kLanguageTag = "#Language";

#if UNITY_EDITOR
        /// <summary>
        /// When you right-click on <see cref="FAST.AssetFromFile.baseFileName"/> in the Inspector, this function will run in the Editor 
        /// to get the filename of an asset assigned to a corresponding <c style="color:DarkRed;"><see cref="Component"/></c>.
        /// </summary>
        /// @details When creating a new derived class, override this function by surrounding the implementation 
        /// with a <c>UNITY_EDITOR</c> conditional compilation directive.
        /// @code
        /// #if UNITY_EDITOR
        /// override protected void SetNameWithFileExtension()
        /// {
        ///     component = GetComponent&lt;Component&gt;();
        ///     baseFileName = Path.GetFileName(AssetDatabase.GetAssetPath(component.asset.GetInstanceID()));
        ///     UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
        /// }
        /// #endif
        /// @endcode
        abstract protected void SetNameWithFileExtension();
#endif

        /// <summary>
        /// When the script is loaded, <see cref="FAST.AssetFromFile.UpdateFileName(string)"/> is called 
        /// to initialize <see cref="FAST.AssetFromFile.fileName"/>.
        /// </summary>
        virtual protected void Awake()
        {
            UpdateFileName(isSharedByAllLanguages ? kSharedLanguage : Application.language);
        }

        /// <summary>
        /// Loads the asset from <see cref="FAST.Application.assets"/> dictionary using 
        /// <c>language</c> and <see cref="FAST.AssetFromFile.fileName"/>.
        /// </summary>
        /// <param name="language">The language of the asset to load.</param>
        abstract public void Load(string language);

        /// <summary>
        /// Updates <see cref="FAST.AssetFromFile.fileName"/> using <c>language</c>,  <see cref="FAST.AssetSettings"/>, 
        /// <see cref="FAST.AssetFromFile.prefixFileName"/>, <see cref="FAST.AssetFromFile.baseFileName"/>, 
        /// and <see cref="FAST.AssetFromFile.suffixFileName"/>.
        /// </summary>
        /// <param name="language">The language to use for the <c>fileName</c>.</param>
        protected void UpdateFileName(string language)
        {
            AssetSettings settings = (Application.settings as BaseSettings).assetSettings;
            prefixFileName = settings.prefixFileName;
            suffixFileName = settings.suffixFileName;

            prefixFileName = ReplaceTagInName(prefixFileName, kProductNameTag, UnityEngine.Application.productName);
            prefixFileName = ReplaceTagInName(prefixFileName, kSkinTag, Application.skin);

            suffixFileName = ReplaceTagInName(suffixFileName, kProductNameTag, UnityEngine.Application.productName);
            suffixFileName = ReplaceTagInName(suffixFileName, kSkinTag, Application.skin);

            prefixFileName = ReplaceTagInName(prefixFileName, kLanguageTag, language);
            suffixFileName = ReplaceTagInName(suffixFileName, kLanguageTag, language);

            fileName = prefixFileName + baseFileName;

            int extensionIndex = fileName.LastIndexOf('.');
            if (extensionIndex != -1) {
                fileName = fileName.Insert(extensionIndex, suffixFileName);
            }
            else {
                fileName += suffixFileName;
                string errorTitle = "File extension missing!";
                string errorMessage = $"All asset filenames must be specified with an extension. " +
                    $"The game object {gameObject.name} with component {this.GetType().Name} does not have an extension.";
                Debug.LogError($"ERROR\n{errorTitle}\n{errorMessage}\n");
            }
        }

       // <summary>
       // Replaces the specified hashtag in a name with a different string. 
       // Used by <see cref="FAST.AssetFromFile.UpdateFileName(string)"/>
       // </summary>
       // <param name="name">A file name or part of a file name.</param>
       // <param name="tag">A hashtag to find and replace.</param>
       // <param name="replacement">A string to replace the hashtag.</param>
       // <returns>A new string with the hashtag replaced.</returns>
       // <remarks>
       // <para>
       // See <see cref="FAST.AssetFromFile.kProductNameTag"/>,
       // <see cref="FAST.AssetFromFile.kSkinTag"/>,
       // <see cref="FAST.AssetFromFile.kLanguageTag"/>
       // for more info about hashtags.
       // </para>
       // </remarks>
        private string ReplaceTagInName(string name, string tag, string replacement)
        {
            if (name.Contains(tag)) {
                name = name.Replace(tag, replacement);
            }
            if (name.Contains(tag.ToLower())) {
                name = name.Replace(tag.ToLower(), replacement);
            }
            return name;
        }
    }
}
