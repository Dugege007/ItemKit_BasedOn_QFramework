//WARN: keep the file outside of asmdef folder, otherwise it'll fail to compile as editor depends on a Runtime lib which in turn is only added if symbol is defined, that's to ensure child package is compiled correctly
#if !LiveScriptReload_Enabled && UNITY_EDITOR

using FastScriptReload.Editor;
using ImmersiveVRTools.Editor.Common.Utilities;
using UnityEditor;
using UnityEngine;

namespace LiveScriptReload.Scripts.Editor
{
    [InitializeOnLoad]
    public class EnsurePreprocessorDirectiveAdded : MonoBehaviour
    {
        public static string BuildSymbol_LiveScriptReload = "LiveScriptReload_Enabled";

        static EnsurePreprocessorDirectiveAdded()
        {
            Debug.LogError($"'{BuildSymbol_LiveScriptReload}' define symbol not present. " +
                           $"Live Script Reload will not work correctly (will try to auto-add), if you can still see error after please add via:" +
                           $"\r\n1) Edit -> Project Settings -> Player -> Scripting Define Symbols" +
                           $"2) Ensure '{BuildSymbol_LiveScriptReload}' is added for all platforms that you intend to use the tool on, eg Windows / Android");
        
            BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_LiveScriptReload, true);
        }
    }
}
#endif
