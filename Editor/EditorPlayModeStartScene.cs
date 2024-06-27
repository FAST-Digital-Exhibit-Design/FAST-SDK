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

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FAST
{
    [InitializeOnLoad]
    public class EditorPlayModeStartScene
    {
        private const string kAutoLoadMenuItem = "FAST/Auto-Load Start Scene on Play (Scene 0)";
        private const string kStartSceneMenuItem = "FAST/Change Start Scene (Scene 0)...";
        private const string kEditorPrefAutoLoadStart = "LoadStartSceneOnPlay";

        static EditorPlayModeStartScene()
        {
            EditorBuildSettings.sceneListChanged += () => { SetPlayModeStartScene(isLogging: true); };

            SetAutoLoad();
            SetPlayModeStartScene(isLogging: false);
        }

        [MenuItem(kAutoLoadMenuItem, priority = 0)]
        static void ToggleAutoLoad()
        {
            IsLoadStartSceneOnPlay = !IsLoadStartSceneOnPlay;

            SetAutoLoad();
            SetPlayModeStartScene(isLogging: true);
        }

        [MenuItem(kStartSceneMenuItem, priority = 11)]
        static void ChangeStartScene()
        {
            EditorWindow.GetWindow(typeof(BuildPlayerWindow));
        }


        static void SetPlayModeStartScene(bool isLogging)
        {
            if (IsLoadStartSceneOnPlay) {
                EditorBuildSettingsScene scene0 = null;
                foreach (var scene in EditorBuildSettings.scenes) {
                    if (scene.enabled) {
                        scene0 = scene;
                        break;
                    }
                }
                if (scene0 != null) {
                    SceneAsset startScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(scene0.path);
                    EditorSceneManager.playModeStartScene = startScene;
                    if (isLogging) {
                        Debug.Log($"[FAST SDK] The play mode start scene has been set to build scene[0]: {EditorSceneManager.playModeStartScene.name}");
                    }
                }
                else {
                    EditorSceneManager.playModeStartScene = null;
                    if (isLogging) {
                        Debug.LogError($"[FAST SDK] No start scene is available. Please configure the start scene as scene[0] in the <b>Build Settings</b> window.");
                        Debug.Log($"[FAST SDK] The play mode start scene has defaulted to the current open scene: {EditorSceneManager.GetActiveScene().name}");
                        ChangeStartScene();
                    }
                }
            }
            else {
                EditorSceneManager.playModeStartScene = null;
                if (isLogging) {
                    Debug.Log($"[FAST SDK] The play mode start scene has defaulted to the current open scene: {EditorSceneManager.GetActiveScene().name}");
                }
            }
        }

        static void SetAutoLoad()
        {
            Menu.SetChecked(kAutoLoadMenuItem, IsLoadStartSceneOnPlay);
        }
        static bool IsLoadStartSceneOnPlay
        {
            get
            {
                return EditorPrefs.GetBool($"{UnityEngine.Application.productName}.{kEditorPrefAutoLoadStart}", false);
            }
            set
            {
                EditorPrefs.SetBool($"{UnityEngine.Application.productName}.{kEditorPrefAutoLoadStart}", value);
            }
        }
    }
}
