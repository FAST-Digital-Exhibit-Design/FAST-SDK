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

#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Compilation;

namespace FAST
{
    [InitializeOnLoad]
    public class ScriptingDefineSymbols
    {
        private static readonly string[] FastDefineSymbols = { "FAST_WIN", "FAST_OSX", "FAST_LINUX" };
        static ScriptingDefineSymbols()
        {
            CompilationPipeline.compilationStarted += (x) => UpdateFastDefineSymbols();
            AssemblyReloadEvents.beforeAssemblyReload += UpdateFastDefineSymbols;
        }

        private static void UpdateFastDefineSymbols()
        {
            string buildDefineSymbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);

            // Remove old FAST define symbols
            buildDefineSymbols = ClearFastDefineSymbols(buildDefineSymbols);
            // Add new FAST define symbols
            string newFastDefineSymbol = string.Empty;

            var activeBuildTarget = EditorUserBuildSettings.activeBuildTarget;
            var runtimePlatform = UnityEngine.Application.platform;

            Debug.Log($"[FAST] Current build target is: {activeBuildTarget}");
            Debug.Log($"[FAST] Current platform is: {runtimePlatform}");

            // Compilation is for a standalone player
            if (BuildPipeline.isBuildingPlayer) {
                if (activeBuildTarget == BuildTarget.StandaloneWindows || activeBuildTarget == BuildTarget.StandaloneWindows64) {
                    newFastDefineSymbol = "FAST_WIN";
                }
                else if (activeBuildTarget == BuildTarget.StandaloneOSX) {
                    newFastDefineSymbol = "FAST_OSX";
                }
                else if (activeBuildTarget == BuildTarget.StandaloneLinux64) {
                    newFastDefineSymbol = "FAST_LINUX";
                }
            }
            // Compilation is Editor script change
            else {
                if (runtimePlatform == RuntimePlatform.WindowsEditor) {
                    newFastDefineSymbol = "FAST_WIN";
                }
                else if (runtimePlatform == RuntimePlatform.OSXEditor) {
                    newFastDefineSymbol = "FAST_OSX";
                }
                else if (runtimePlatform == RuntimePlatform.LinuxEditor) {
                    newFastDefineSymbol = "FAST_LINUX";
                }
            }

            if (!string.IsNullOrEmpty(newFastDefineSymbol) && !buildDefineSymbols.Contains(newFastDefineSymbol)) {
                buildDefineSymbols = string.IsNullOrEmpty(buildDefineSymbols) ? newFastDefineSymbol : $"{buildDefineSymbols};{newFastDefineSymbol}";
                PlayerSettings.SetScriptingDefineSymbols(NamedBuildTarget.Standalone, buildDefineSymbols);
            }

            Debug.Log($"[FAST] Current define symbols for {NamedBuildTarget.Standalone}:\n{buildDefineSymbols}");
        }

        private static string ClearFastDefineSymbols(string defineSymbols)
        {
            foreach (var fastDefineSymbol in FastDefineSymbols) {
                defineSymbols = defineSymbols.Replace(fastDefineSymbol, "");
            }

            // Remove extra semicolons
            while (defineSymbols.Contains(";;")) {
                defineSymbols = defineSymbols.Replace(";;", ";");
            }
            return defineSymbols.Trim(';');
        }
    }
}
#endif
