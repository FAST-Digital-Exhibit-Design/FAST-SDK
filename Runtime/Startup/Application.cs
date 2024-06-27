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
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Networking;

namespace FAST
{
    /// <summary>
    /// Access a FAST application's runtime data.
    /// </summary>
    /// <remarks>
    /// This class contains static methods for looking up information about and controlling the runtime data. 
    /// It is included in the <c>Startup Prefab</c>.
    /// </remarks>
    public class Application : Singleton<Application>
    {
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// Set to <see langword="true"/> if the cursor should be hidden on startup.
        /// </summary>
        [SerializeField]
        private bool hideCursor = true;

        protected override void Awake()
        {
            base.Awake();
#if !UNITY_EDITOR
            Cursor.visible = !hideCursor;
#endif
        }

        /// <summary>
        /// Indicates whether to change to the next or previous language.
        /// </summary>
        public enum ChangeLanguageMode { Previous = -1, Next = 1 }

        /// <summary>
        /// Returns the path to the root folder where this activity is installed.
        /// </summary>
        /// <remarks>
        /// This path is set by the <see cref="FAST.InstallationLoader"/> during startup.
        /// </remarks>
        public static string activityDirectory = "";

        /// <summary>
        /// Returns the path to the Application folder where this activity is installed.
        /// </summary>
        /// <remarks>
        /// This path is set by the <see cref="FAST.InstallationLoader"/> during startup.
        /// </remarks>
        public static string applicationDirectory = "";

        /// <summary>
        /// Returns the path to the Assets folder where this activity is installed.
        /// </summary>
        /// <remarks>
        /// This path is set by the <see cref="FAST.InstallationLoader"/> during startup.
        /// </remarks>
        public static string assetsDirectory = "";

        /// <summary>
        /// Returns the path to the Logs folder where this activity is installed.
        /// </summary>
        /// <remarks>
        /// This path is set by the <see cref="FAST.InstallationLoader"/> during startup.
        /// </remarks>
        public static string logsDirectory = "";

        /// <summary>
        /// Returns the path to the activity settings files where this activity is installed.
        /// </summary>
        /// <remarks>
        /// This path is set by the <see cref="FAST.SettingsLoader"/> during startup.
        /// </remarks>
        public static string settingsPath = "";

        /// <summary>
        /// The activity settings.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.SettingsLoader"/> during startup.
        /// </remarks>
        public static dynamic settings;

        /// <summary>
        /// The current skin for the activity.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.AssetsLoader"/> during startup.
        /// </remarks>
        public static string skin = "";

        /// <summary>
        /// The available languages for the activity.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.AssetsLoader"/> during startup.
        /// </remarks>
        public static string[] languages = { };

        /// <summary>
        /// The current language.
        /// </summary>
        /// <remarks>
        /// This is initialized in <see cref="FAST.Application.InitializeLanguage()"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static string language = "";

        /// <summary>
        /// The index of the current language within the <see cref="FAST.Application.languages"/> array.
        /// </summary>
        /// <remarks>
        /// This is initialized in <see cref="FAST.Application.InitializeLanguage()"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static int languageIndex;

        /// <summary>
        /// The dictionary of assets from file.
        /// </summary>
        /// <remarks>
        /// This is initialized by <see cref="FAST.AssetsLoader"/> and used in every class that derives 
        /// from <see cref="FAST.AssetFromFile"/>.
        /// </remarks>
        public static Dictionary<string, Dictionary<string, System.Object>> assets = new();

        /// <summary>
        /// An array of all <see cref="FAST.SerialConnection"/>s loaded at startup.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.StartupManager"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static SerialConnection[] serialConnections = { };

        /// <summary>
        /// An array of all <see cref="FAST.UdpConnection"/>s loaded at startup.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.StartupManager"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static UdpConnection[] udpConnections = { };

        /// <summary>
        /// An array of all <c style="color:DarkRed;"><see cref="FAST.WebRequest"/></c>s loaded at startup.
        /// </summary>
        /// <remarks>
        /// This is initialized by the <see cref="FAST.StartupManager"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static WebRequest[] webRequests = { };

        private static AssetFromFile[] assetsFromFile;

        /// <summary>
        /// Makes a copy of the previous log file with a timestamp and saves it to the 
        /// <see cref="FAST.Application.logsDirectory"/>.
        /// </summary>
        /// <remarks>
        /// This is called by the <see cref="FAST.StartupManager"/> during startup 
        /// after all loaders have completed.
        /// </remarks>
        public static void CopyPreviousLog()
        {
            LogSettings logSettings = settings.logSettings;

            // Don't bother copying logs in the Editor because they include all console output for the entire session.
            if (!logSettings.isLogging || UnityEngine.Application.isEditor) {
                return;
            }

            try {
                DirectoryInfo directoryInfo = new(logsDirectory);
                FileInfo[] fileInfos = directoryInfo.GetFiles("*.log").OrderBy(p => p.CreationTimeUtc).ToArray();
                for (int i = 0; i <= (fileInfos.Length - logSettings.numberOfLogs); i++) {
                    Debug.Log($"Deleting log: {fileInfos[i].FullName}");
                    fileInfos[i].Delete();
                }
            }
            catch (Exception exception) {
                Debug.LogWarning(exception.Message);
            }

            string unityLogPath = UnityEngine.Application.consoleLogPath.Replace(".log", "-prev.log");
            string newLogPath = Path.Combine(logsDirectory, Path.GetFileName(unityLogPath)).Replace(".log", $"{DateTime.Now:-yyyyMMdd-HHmmss}.log");
            try {
                File.Copy(unityLogPath, newLogPath, true);
            }
            catch (Exception exception) {
                Debug.LogWarning(exception.Message);
            }
        }

        /// <summary>
        /// Intializes the current language and all <see cref="FAST.AssetFromFile"/> assets.
        /// </summary>
        /// <remarks>
        /// This is called by the <see cref="FAST.StartupManager"/> during startup 
        /// after the activity scene has been loaded.
        /// </remarks>
        public static void InitializeLanguage()
        {
            assetsFromFile = UnityEngine.Object.FindObjectsByType<AssetFromFile>(FindObjectsInactive.Include, FindObjectsSortMode.None);

            languageIndex = 0;
            language = languages[languageIndex];

            LoadAssetsFromFile();
        }

        /// <summary>
        /// Changes the current language and all <see cref="FAST.AssetFromFile"/> assets.
        /// </summary>
        /// <param name="mode">Either the next or previous language in the 
        /// <see cref="FAST.Application.languages"/> array.</param>
        public static void ChangeLanguage(ChangeLanguageMode mode)
        {
            languageIndex = (languageIndex + (int)mode) % languages.Length;
            language = languages[languageIndex];
            LoadAssetsFromFile();
        }

        /// <summary>
        /// Changes the current language and all <see cref="FAST.AssetFromFile"/> assets.
        /// </summary>
        /// <param name="index">The index of the language to use in the 
        /// <see cref="FAST.Application.languages"/> array.</param>
        public static void ChangeLanguageByIndex(int index)
        {
            if (languageIndex >= languages.Length) {
                return;
            }
            languageIndex = index;
            language = languages[languageIndex];
            LoadAssetsFromFile();
        }

        /// <summary>
        /// Changes the current language and all <see cref="FAST.AssetFromFile"/> assets.
        /// </summary>
        /// <param name="name">The name of the language to use in the 
        /// <see cref="FAST.Application.languages"/> array.</param>
        public static void ChangeLanguageByName(string name)
        {
            bool isLanguageFound = false;
            for (int i = 0; i < languages.Length; i++) {
                if (languages[i].Equals(name)) {
                    isLanguageFound = true;
                    languageIndex = i;
                    language = name;
                    break;
                }
            }

            if (!isLanguageFound) {
                return;
            }

            LoadAssetsFromFile();
        }

        private static void LoadAssetsFromFile()
        {
            foreach (var assetFromFile in assetsFromFile) {
                assetFromFile.Load(language);
            }
        }

        /// <summary>
        /// Writes the activity settings to file.
        /// </summary>
        /// <remarks>
        /// This is called by the <see cref="FAST.StartupManager"/> during startup 
        /// after all loaders have completed sync changes to the activity settings attributes. 
        /// It is also called by <see cref="FAST.SettingsLoader"/> when the 
        /// activity settings file is not found.
        /// </remarks>
        /// <returns><see langword="true"/> if the write succeeded, 
        /// <see langword="false"/> if it failed.</returns>
        public static bool WriteSettings()
        {
            bool result = true;
            try {
                settings.Update();

                Type type = settings.GetType();
                Debug.Log(type.Name);
                XmlSerializer serializer;
                FileStream stream;

                serializer = new XmlSerializer(type);
                stream = new FileStream(settingsPath, FileMode.Create);
                serializer.Serialize(stream, settings);
                stream.Close();
            }
            catch (Exception exception) {
                Debug.Log("Couldn't write settings file.");
                Debug.Log(exception.Message);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Reads the activity settings from file.
        /// </summary>
        /// <remarks>
        /// This is called by the <see cref="FAST.SettingsLoader"/> during startup 
        /// to initialize the avtibity settings. 
        /// </remarks>
        /// <returns><see langword="true"/> if the read succeeded, 
        /// <see langword="false"/> if it failed.</returns>
        public static bool ReadSettings()
        {
            bool result = true;
            try {
                Type type = settings.GetType();
                XmlSerializer serializer;
                FileStream stream;
                serializer = new XmlSerializer(type);
                stream = new FileStream(settingsPath, FileMode.Open);
                // *NOTE* This creates a new object and changes the settings reference
                settings = serializer.Deserialize(stream);
                stream.Close();
            }
            catch (Exception exception) {
                Debug.Log("Couldn't read settings file.");
                Debug.Log(exception.Message);
                result = false;
            }

            return result;
        }

        /// <summary>
        /// Quits the Unity Player or Editor.
        /// </summary>
        public static void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            UnityEngine.Application.Quit();
        }

        /// <summary>
        /// Toggles the application between fullscreen and windowed.
        /// </summary>
        public static void ToggleFullscreen()
        {
            Screen.fullScreen = !Screen.fullScreen;
        }

        /// <summary>
        /// Toggles the mouse cursor on/off.
        /// </summary>
        public static void ToggleCursor()
        {
            Cursor.visible = !Cursor.visible;
        }
    }
}
