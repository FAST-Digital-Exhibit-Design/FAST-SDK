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
using UnityEngine;
using UnityEditor;

namespace FAST
{
    /// <summary>
    /// Verifies the file and folder structure where this activity is installed.
    /// </summary>
    public class InstallationLoader : StartupLoader
    {
        /// <summary>
        /// The name of the folder where the activity executable is installed.
        /// </summary>
        public const string kApplicationFolder = "Application";

        /// <summary>
        /// The name of the folder where the activity assets are installed.
        /// </summary>
        public const string kAssetsFolder = "Assets";

        /// <summary>
        /// The name of the folder where Unity Player logs are saved.
        /// </summary>
        public const string kLogsFolder = "Logs";

        /// <summary>
        /// <b style="color: DarkCyan;">Inspector</b><br/>
        /// The directory with the installation file structure to use while developing in the Unity Editor.
        /// </summary>
        /// <remarks>
        /// The path can be absolute or relative to the Unity project folder.
        /// </remarks>
        [NaughtyAttributes.InfoBox("This directory is required in the Editor. " +
         "The path can be absolute or relative to the Unity project folder."),
         SerializeField, Header("Required"),
         Tooltip("A directory with the installation file structure to use while developing in the Unity Editor."),
         ContextMenuItem("Select Directory as Relative Path", "SelectEditorInstallDirectoryAsRelativePath", order = 0),
         ContextMenuItem("Select Directory as Absolute Path", "SelectEditorInstallDirectoryAsAbsolutePath", order = 1)]
        private string editorInstallDirectory = "Build";

#if UNITY_EDITOR
        private void SelectEditorInstallDirectoryAsRelativePath()
        {
            string selectedDirectory = EditorUtility.OpenFolderPanel("Select Editor Install Directory (Relative Path)", "", "");
            if (!string.IsNullOrWhiteSpace(selectedDirectory)) {
                editorInstallDirectory = Path.GetRelativePath(Directory.GetCurrentDirectory(), selectedDirectory);
            }
        }
        private void SelectEditorInstallDirectoryAsAbsolutePath()
        {
            string selectedDirectory = EditorUtility.OpenFolderPanel("Select Editor Install Directory (Absolute Path)", "", "");
            if (!string.IsNullOrWhiteSpace(selectedDirectory)) {
                editorInstallDirectory = selectedDirectory;
            }
        }
#endif

        [Space(10), Header("Runtime")]
        [SerializeField]
        private string activityDirectory;
        [SerializeField]
        private string applicationDirectory;
        [SerializeField]
        private string assetsDirectory;
        [SerializeField]
        private string logsDirectory;

        protected override IEnumerator ExecuteLoad()
        {
            // Display and log the current directory for debugging, but no additional verification needed yet
            string currentDirectory;
#if UNITY_EDITOR
            try {
                currentDirectory = Path.GetFullPath(editorInstallDirectory);
            }
            catch {
                errorTitle = "Editor configuration incomplete!";
                errorMessage = "This application is not configured to run from the Editor. " +
                    "The <b>Editor Install Directory</b> variable must be set on the <b>InstallationLoader</b> script.";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
#else
            currentDirectory = Directory.GetCurrentDirectory();
#endif
            loadingTitle = "Verifying installation . . .";
            Debug.Log($"\n{loadingTitle}");
            loadingMessage = "Current directory: " + currentDirectory;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Determine the root and sub-directories
            if (currentDirectory.Contains(kApplicationFolder)) {
                activityDirectory = currentDirectory.Substring(0, currentDirectory.LastIndexOf(kApplicationFolder));
            }
            else {
                activityDirectory = currentDirectory;
            }
            applicationDirectory = Path.Combine(activityDirectory, kApplicationFolder);
            assetsDirectory = Path.Combine(activityDirectory, kAssetsFolder);
            logsDirectory = Path.Combine(activityDirectory, kLogsFolder);

            loadingMessage = "Activity directory: " + activityDirectory;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Verify the Application directory
            loadingMessage = "Application directory: " + applicationDirectory;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!Directory.Exists(applicationDirectory)) {
                errorTitle = "Folder not found!";
                errorMessage = "The Application folder could not be found. It is expected within the root folder where this application is installed: " +
                    $"\n\t{activityDirectory}\n\t\tApplication\n\t\t\t{UnityEngine.Application.productName}.exe\n";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Verify the Assets directory
            loadingMessage = "Assets directory: " + assetsDirectory;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!Directory.Exists(assetsDirectory)) {
                errorTitle = "Folder not found!";
                errorMessage = "The Assets folder could not be found. It is expected within the root folder where this application is installed: " +
                    $"\n\t{activityDirectory}\n\t\tAssets";
                Debug.LogError($"\nERROR\n{errorTitle}\n{errorMessage}\n");
                errorEvent.Invoke(errorTitle, errorMessage);
                yield break;
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            // Verify the Logs directory
            loadingMessage = "Logs directory: " + logsDirectory;
            Debug.Log($"{loadingMessage}");
            loadingEvent.Invoke(loadingTitle, loadingMessage);

            if (!Directory.Exists(logsDirectory)) {
                Directory.CreateDirectory(logsDirectory);
            }
            yield return new WaitForSecondsRealtime(loadingMessageDuration);

            Application.activityDirectory = activityDirectory;
            Application.applicationDirectory = applicationDirectory;
            Application.assetsDirectory = assetsDirectory;
            Application.logsDirectory = logsDirectory;

            successEvent.Invoke();
        }
    }
}
