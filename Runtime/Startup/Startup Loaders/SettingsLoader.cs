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
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Xml;
using System.Xml.Serialization;

namespace FAST
{
    /// <summary>
    /// Loads the activity configuration and settings XML files on startup.
    /// </summary>
    /// <remarks>
    /// If the configuration file doesn't exist, it will be written to the 
    /// <see cref="FAST.Application.activityDirectory"/>. And if the settings 
    /// file doesn't exist, it will be written to the <see cref="FAST.Application.assetsDirectory"/>.
    /// </remarks>
    public class SettingsLoader : StartupLoader
    {
#if UNITY_EDITOR
        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The script that derives from <see cref="FAST.BaseSettings"/> to define the settings for this activity.
        /// </summary>
        [NaughtyAttributes.Required,
         NaughtyAttributes.ValidateInput("IsValidSettings", "This script must derive from FAST.BaseSettings."),
         SerializeField,
         Tooltip("The script that derives from <b>FAST.BaseSettings</b> to define the settings for this activity.")]
        private MonoScript settingsScript;
#endif

        [SerializeField, HideInInspector]
        private string scriptName;

        /// <summary>
        /// <b style="color: DarkCyan;">Settings, Runtime</b><br/>
        /// The configuration settings loaded at runtime.
        /// </summary>
        /// <remarks>
        /// The skin is specified here and used to load the correct activity settings.
        /// </remarks>
        [SerializeField, Header("Runtime")]
        private ConfigSettings configSettings;

        private string configPath;
        private string settingsPath;

        protected override IEnumerator ExecuteLoad()
        {
            // Read the application config settings
            configPath = Path.Combine(Application.activityDirectory, "config.xml");

            loadingTitle = "Loading settings . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = "Config file: " + configPath;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!File.Exists(configPath)) {
                Debug.LogWarning($"WARNING\nConfig file cannot be found.");

                if (!WriteConfig()) {
                    errorTitle = "File not found!";
                    errorMessage = "The config file cannot be found or created.";
                    Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                    errorEvent.Invoke(errorTitle, errorMessage);
                    yield break;
                }
                Debug.LogWarning($"WARNING\nNew config file created.");
            }

            if (!ReadConfig()) {
                errorTitle = "File not accessible!";
                errorMessage = "The config file cannot be read. The XML may be incorrectly formatted or malformed.";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            Application.skin = configSettings.skin;

            // Read the activity settings
            Application.settingsPath = Path.Combine(Application.assetsDirectory, Application.skin, $"{Application.skin}-settings.xml");
            Application.settings = Activator.CreateInstance("Assembly-CSharp", scriptName).Unwrap();
            settingsPath = Application.settingsPath;

            loadingTitle = "Loading settings . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = "Settings file: " + settingsPath;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!File.Exists(settingsPath)) {
                Debug.LogWarning($"WARNING\nSettings file cannot be found.");
                Application.settings.Initialize();

                if (!Application.WriteSettings()) {
                    errorTitle = "File not found!";
                    errorMessage = "The settings file cannot be found or created.";
                    Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                    errorEvent.Invoke(errorTitle, errorMessage);
                    yield break;
                }
                Debug.LogWarning($"WARNING\nNew settings file created.");
            }

            if (!Application.ReadSettings()) {
                errorTitle = "File not accessible!";
                errorMessage = "The settings file cannot be read. The XML may be incorrectly formatted or malformed.";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            successEvent.Invoke();
        }

        private bool WriteConfig()
        {
            bool result = true;
            try {
                Type type = typeof(ConfigSettings);
                XmlSerializer serializer;
                FileStream stream;

                serializer = new XmlSerializer(type);
                stream = new FileStream(configPath, FileMode.Create);
                serializer.Serialize(stream, configSettings);
                stream.Close();
            }
            catch (Exception exception) {
                Debug.Log("Couldn't write config file.");
                Debug.Log(exception.Message);
                result = false;
            }

            return result;
        }

        private bool ReadConfig()
        {
            bool result = true;
            try {
                Type type = typeof(ConfigSettings);
                XmlSerializer serializer;
                FileStream stream;
                serializer = new XmlSerializer(type);
                stream = new FileStream(configPath, FileMode.Open);
                // *NOTE* This creates a new object and changes the settings reference
                configSettings = serializer.Deserialize(stream) as ConfigSettings;
                stream.Close();
            }
            catch (Exception exception) {
                Debug.Log("Couldn't read config file.");
                Debug.Log(exception.Message);
                result = false;
            }

            return result;
        }

#if UNITY_EDITOR
        private bool IsValidSettings(MonoScript script)
        {
            if (script == null) {
                return false;
            }
            Type type = Type.GetType($"{script.name}, Assembly-CSharp");
            return typeof(BaseSettings) == type.BaseType;
        }

        public void OnAfterDeserialize() => GetScriptName();
        public void OnBeforeSerialize() => GetScriptName();
        public void OnValidate() => GetScriptName();
        private void GetScriptName()
        {
            if (settingsScript == null) {
                scriptName = "";
                return;
            }
            scriptName = settingsScript.name;
        }
#endif
    }
}
