#if LiveScriptReload_Enabled

using System;
using FastScriptReload.Runtime;
using HarmonyLib;
using ImmersiveVRTools.Editor.Common.Utilities;
using UnityEditor;
using UnityEngine;

namespace LiveScriptReload.Editor
{
    [InitializeOnLoad]
    [PreventHotReload]
    public class BuildWindowAdjustmentPatch : MonoBehaviour
    {
        private static bool _isPostfixThrowingException;
        
        static BuildWindowAdjustmentPatch()
        {
            try
            {
                if ((bool)LiveScriptReloadPreference.IsBuildWarningDisplayedInUnityBuildWindow.GetEditorPersistedValueOrDefault())
                {
                    var harmony = new Harmony(nameof(BuildWindowAdjustmentPatch));

                    var original = AccessTools.Method("UnityEditor.BuildPlayerWindow:ShowBuildTargetSettings");
                    var postfix = AccessTools.Method(typeof(BuildWindowAdjustmentPatch), nameof(ShowBuildTargetSettingsPostfix));

                    harmony.Patch(original, postfix: new HarmonyMethod(postfix));
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to add Live Script Reload Build Section to Unity Build Window - no warnings will be shown if you try to build with tool enabled. Error: {e}");
            }
        }

        private static void ShowBuildTargetSettingsPostfix()
        {
            if(_isPostfixThrowingException)
                return;

            try
            {
                if ((bool)LiveScriptReloadPreference.IsBuildWarningDisplayedInUnityBuildWindow.GetEditorPersistedValueOrDefault())
                {
                    DrawLiveScriptReloadBuildStatus();
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Unable to add Live Script Reload Build Section to Unity Build Window - no warnings will be shown if you try to build with tool enabled. Error: {e}");
                _isPostfixThrowingException = true;
            }
        }

        private static void DrawLiveScriptReloadBuildStatus()
        {
            EditorGUILayout.BeginHorizontal();
            var showIl2CppBackendEnabledWarning = false;
            
#if LiveScriptReload_IncludeInBuild_Enabled
            var label = "Live Script Reload: INCLUDED in build";
            var tooltip = "Live script reload functionality will be included in build, make sure this is intended as otherwise it'll create an exploit in your application.\r\n\r\nClick 'Adjust' button to change.";
            if (PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup) == ScriptingImplementation.IL2CPP)
            {
                showIl2CppBackendEnabledWarning = true;
            }
#else
            var label = "Live Script Reload: EXCLUDED from build";
            var tooltip = "Live script reload functionality will not be included in build, adjust if you intend to create test build.\r\n\r\nClick 'Adjust' button to change.";
#endif

            EditorGUILayout.LabelField(label);
            GuiTooltipHelper.AddHelperTooltip(tooltip, new Vector4(0, 5, 0, 0), 40);

            if (GUILayout.Button("Adjust"))
            {
                LiveScriptReloadWelcomeScreen.Init().OpenBuildSection();
            }

            EditorGUILayout.EndHorizontal();

            if (showIl2CppBackendEnabledWarning)
            {
                EditorGUILayout.HelpBox("Live Script Reload: IL2CPP backend is not supported. Hot-Reload won't work.", MessageType.Error);
            }
        }
    }
}

#endif