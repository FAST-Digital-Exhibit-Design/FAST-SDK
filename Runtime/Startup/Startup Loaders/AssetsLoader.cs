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

using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// Loads image, video, and audio assets from the Assets folder where this activity is installed 
    /// on startup.
    /// </summary>
    /// <remarks>
    /// The assets for the current skin and available languages specified 
    /// in the <see cref="FAST.AssetSettings"/> are loaded into <see cref="FAST.Application.assets"/>.
    /// </remarks>
    public class AssetsLoader : StartupLoader
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The path to the Assets folder where this activity is installed.
        /// </summary>
        [SerializeField]
        private string assetsPath;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The path to the current skin folder where this activity is installed.
        /// </summary>
        [SerializeField]
        private string skinPath;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The paths to the available language folders where this activity is installed.
        /// </summary>
        [SerializeField]
        private List<string> languagePaths;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The current skin for the activity.
        /// </summary>
        [SerializeField]
        private string skin;

        /// <summary>
        /// <b style="color: DarkCyan;">Runtime</b><br/>
        /// The available languages for the activity.
        /// </summary>
        [SerializeField]
        private List<string> languages;

        protected override IEnumerator ExecuteLoad()
        {
            skin = Application.skin;
            assetsPath = Application.assetsDirectory;
            AssetSettings settings = Application.settings.assetSettings;
            languages = new(settings.languages);
            
            // Remove the Shared language if it was listed in the settings so it isn't added as 
            // an available language that can be set. The Shared folder will be checked for before
            // loading assets.
            if (languages.Contains(AssetFromFile.kSharedLanguage)) {
                languages.Remove(AssetFromFile.kSharedLanguage);
            }
            Application.languages = languages.ToArray();

            // Verify skin path
            skinPath = Path.Combine(assetsPath, skin);
            loadingTitle = $"Loading assets . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = $"Skin directory: {skinPath}";
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!Directory.Exists(skinPath)) {
                errorTitle = "Folder not found!";
                if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                    errorMessage = $"The {skin} folder cannot be found! " +
                        $"It is expected within the Assets folder where this application is installed: " +
                        $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{skin}";
                }
                else {
                    errorMessage = settings.replacementErrorMessage;
                }
                errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Verify language paths
            languagePaths = new(new string[languages.Count]);
            for (int i = 0; i < languages.Count; i++) {
                languagePaths[i] = Path.Combine(skinPath, languages[i]);
                loadingMessage = $"Language directory: {languagePaths[i]}";
                Debug.Log($"{loadingMessage}");
                loadingEvent.Invoke(loadingTitle, loadingMessage);

                if (!Directory.Exists(languagePaths[i])) {
                    errorTitle = "Folder not found!";
                    if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                        errorMessage = $"The {languages[i]} folder cannot be found. " +
                            $"It is expected within the {skin} folder where this application is installed: " +
                            $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{skin}\n\t\t\t{languages[i]}";
                    }
                    else {
                        errorMessage = settings.replacementErrorMessage;
                    }
                    errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                    Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                    errorEvent.Invoke(errorTitle, errorMessage);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(loadingMessageDuration);
            }

            // Check for Shared folder and add it as a language to load assets from, if it exists.
            string sharedLanguagePath = Path.Combine(skinPath, AssetFromFile.kSharedLanguage);
            if (Directory.Exists(sharedLanguagePath)) {
                languages.Add(AssetFromFile.kSharedLanguage);
                languagePaths.Add(sharedLanguagePath);
            }

            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            stopwatch.Start();

            // Load assets for each language
            for (int i = 0; i < languages.Count; i++) {
                List<string> filePaths = new();
                Application.assets.Add(languages[i], new Dictionary<string, System.Object>());

                // Load images
                filePaths.Clear();
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.png", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.jpg", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.jpeg", SearchOption.AllDirectories));
                Debug.LogFormat($"Number of image files: {filePaths.Count}");

                foreach (string filePath in filePaths) {
                    string fileName = Path.GetFileName(filePath);
                    loadingMessage = $"Image file: {filePath}";
                    Debug.Log($"{loadingMessage}");
                    loadingEvent.Invoke(loadingTitle, loadingMessage);

                    WWW fileRequest = new WWW("file:///" + filePath);
                    yield return fileRequest;

                    if (fileRequest.error != null) {
                        errorTitle = "File not accessible!";
                        if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                            errorMessage = $"The {fileName} file cannot be read." +
                                $"It is expected within the {languages[i]} folder where this application is installed: " +
                                $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{skin}\n\t\t\t{languages[i]}";
                        }
                        else {
                            errorMessage = settings.replacementErrorMessage;
                        }
                        errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                        Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n{fileRequest.error}\n");
                        errorEvent.Invoke(errorTitle, errorMessage);
                        yield break;
                    }
                    Application.assets[languages[i]].Add(fileName, fileRequest.texture);
                }

                // Load audio
                filePaths.Clear();
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.ogg", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mp3", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.wav", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.xm", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.it", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mod", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.s3m", SearchOption.AllDirectories));
                Debug.LogFormat($"Number of audio files: {filePaths.Count}");

                foreach (string filePath in filePaths) {
                    string fileName = Path.GetFileName(filePath);
                    loadingMessage = $"Audio file: {filePath}";
                    Debug.Log($"{loadingMessage}");
                    loadingEvent.Invoke(loadingTitle, loadingMessage);

                    WWW fileRequest = new WWW("file:///" + filePath);
                    yield return fileRequest;

                    if (fileRequest.error != null) {
                        errorTitle = "File not accessible!";
                        if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                            errorMessage = $"The {fileName} file cannot be read." +
                                $"It is expected within the {languages[i]} folder where this application is installed: " +
                                $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{skin}\n\t\t\t{languages[i]}";
                        }
                        else {
                            errorMessage = settings.replacementErrorMessage;
                        }
                        errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                        Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n{fileRequest.error}\n");
                        errorEvent.Invoke(errorTitle, errorMessage);
                        yield break;
                    }
                    Application.assets[languages[i]].Add(fileName, fileRequest.GetAudioClip());
                }

                // Load video paths (can't actually load as Video Clips)
                filePaths.Clear();
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.asf", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.avi", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.dv", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.m4v", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mov", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mp4", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mpg", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.mpeg", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.ogv", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.vp8", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.webm", SearchOption.AllDirectories));
                filePaths.AddRange(Directory.GetFiles(languagePaths[i], "*.wmv", SearchOption.AllDirectories));
                Debug.LogFormat($"Number of video files: {filePaths.Count}");

                foreach (string filePath in filePaths) {
                    string fileName = Path.GetFileName(filePath);
                    loadingMessage = $"Video file: {filePath}";
                    Debug.Log($"{loadingMessage}");
                    loadingEvent.Invoke(loadingTitle, loadingMessage);

                    yield return null;

                    if (!File.Exists(filePath)) {
                        errorTitle = "File not found!";
                        if (string.IsNullOrEmpty(settings.replacementErrorMessage)) {
                            errorMessage = $"The {fileName} file cannot be found." +
                                $"It is expected within the {languages[i]} folder where this application is installed: " +
                                $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{skin}\n\t\t\t{languages[i]}";
                        }
                        else {
                            errorMessage = settings.replacementErrorMessage;
                        }
                        errorMessage = settings.prefixErrorMessage + errorMessage + settings.suffixErrorMessage;
                        Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                        errorEvent.Invoke(errorTitle, errorMessage);
                        yield break;
                    }

                    Application.assets[languages[i]].Add(fileName, filePath);
                }
            }

            stopwatch.Stop();
            Debug.LogFormat($"Assets load time: {stopwatch.ElapsedMilliseconds} ms");

            successEvent.Invoke();
        }
    }
}
