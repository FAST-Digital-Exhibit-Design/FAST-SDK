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
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace FAST
{
    public class StartSceneSelectWindow : EditorWindow
    {
        private static SceneAsset startSceneAsset;

        public void Awake()
        {
            titleContent = new GUIContent("Select Start Scene", "Use the field below to select the start scene:");
        }

        public void OnGUI()
        {
            startSceneAsset = EditorSceneManager.playModeStartScene;

            GUILayout.Space(8);
            SceneAsset selectedSceneAsset = (SceneAsset)EditorGUILayout.ObjectField(startSceneAsset, typeof(SceneAsset), false);
            GUILayout.Space(4);

            if (selectedSceneAsset != startSceneAsset) {
                startSceneAsset = selectedSceneAsset;
                SetBuildScenes();
            }
        }

        public void SetBuildScenes()
        {
            // Find valid Scene paths and make a list of EditorBuildSettingsScene
            List<EditorBuildSettingsScene> editorBuildSettingsScenes = new (EditorBuildSettings.scenes);
            string scenePath = AssetDatabase.GetAssetPath(startSceneAsset);
            if (!string.IsNullOrEmpty(scenePath)) {
                EditorBuildSettingsScene[] existingScenes = editorBuildSettingsScenes.Where((x) => x.path == scenePath).ToArray();
                if (existingScenes.Length > 0) {
                    foreach (var scene in existingScenes) {
                        editorBuildSettingsScenes.Remove(scene);
                    }
                    editorBuildSettingsScenes.Insert(0, new EditorBuildSettingsScene(scenePath, true));
                }
                else {
                    editorBuildSettingsScenes[0] = new EditorBuildSettingsScene(scenePath, true);
                }
            }

            // Set the Build Settings window Scene list
            EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        }
    }
}
