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
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace FAST
{
    /// <summary>
    /// Loads the <see cref="FAST.MarkerTrackingSettings"/> from the current skin folder where 
    /// this activity is installed on startup.
    /// </summary>
    public class MarkerTrackingSettingsLoader : StartupLoader
    {
        private string markerTrackingSettingsPath;

        protected override IEnumerator ExecuteLoad()
        {
            string skinPath = Path.Combine(Application.assetsDirectory, Application.skin);

            loadingTitle = $"Searching for marker tracking settings . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = $"Search pattern: {Path.Combine(skinPath, "*-MarkerTracking-settings.xml")}";
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            string[] paths = Directory.GetFiles(skinPath, "*-MarkerTracking-settings.xml", SearchOption.AllDirectories);
            if (paths.Length == 0) {
                errorTitle = "File not found!";
                errorMessage = "The marker tracking settings file cannot be found." +
                    $"It is expected within the {Application.skin} folder where this application is installed: " +
                    $"\n\t{Application.activityDirectory}\n\t\tAssets\n\t\t\t{Application.skin}\n\t\t\t*-MarkerTracking-settings.xml";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            else if (paths.Length > 1) {
                errorTitle = "Too many files found!";
                errorMessage = $"{paths.Length} marker tracking settings files were found. Only the first file will be loaded.";
                Debug.LogWarning($"\nWARNING\n{errorTitle}\n{errorMessage}\n");
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            markerTrackingSettingsPath = paths[0];
            loadingTitle = "Loading marker tracking settings . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = "File: " + markerTrackingSettingsPath;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!ReadSettings()) {
                errorTitle = "File not accessible!";
                errorMessage = "The marker tracking settings file cannot be read. The XML may be incorrectly formatted or malformed.";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            successEvent.Invoke();
        }

        private bool ReadSettings()
        {
            bool result = true;
            try {
                Type type = typeof(MarkerTrackingSettings);
                XmlSerializer serializer;
                FileStream stream;
                serializer = new XmlSerializer(type);
                stream = new FileStream(markerTrackingSettingsPath, FileMode.Open);
                // *NOTE* This creates a new object and changes the settings reference
                Application.settings.markerTrackingSettings = serializer.Deserialize(stream) as MarkerTrackingSettings;
                stream.Close();
            }
            catch (Exception exception) {
                Debug.Log("Couldn't read marker tracking settings file.");
                Debug.Log(exception.Message);
                result = false;
            }

            return result;
        }
    }
}
