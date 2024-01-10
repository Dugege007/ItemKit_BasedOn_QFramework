#if LiveScriptReload_Enabled
using System;
using System.Collections.Generic;
using FastScriptReload.Editor;
using ImmersiveVRTools.Editor.Common.Utilities;
using ImmersiveVRTools.Editor.Common.WelcomeScreen;
using ImmersiveVRTools.Editor.Common.WelcomeScreen.GuiElements;
using ImmersiveVRTools.Editor.Common.WelcomeScreen.PreferenceDefinition;
using ImmersiveVRTools.Editor.Common.WelcomeScreen.Utilities;
using LiveScriptReload.Runtime;
using UnityEditor;
using UnityEngine;

namespace LiveScriptReload.Editor
{
    public class LiveScriptReloadWelcomeScreen: FastScriptReloadWelcomeScreen
    {
        public new static string GenerateGetUpdatesUrl(string userId, string versionId)
        {
            return $"{BaseUrl}/updates/live-script-reload/{userId}?CurrentVersion={versionId}";
        }
        public new static string VersionId = "1.3";
        private static readonly string ProjectIconName = "ProductIcon64";
        public new static readonly string ProjectName = "live-script-reload";

        private static Vector2 _WindowSizePx = new Vector2(650, 500);
        private static string _WindowTitle = "Live Script Reload";
        
        public static ChangeMainViewButton BuildSection { get; private set; }

        public void OpenBuildSection()
        {
            BuildSection.OnClick(this);
        }
        
        private static readonly ScrollViewGuiSection MainScrollViewSection = new ScrollViewGuiSection(
            "", (screen) =>
            {
                GenerateCommonWelcomeText(FastScriptReloadPreference.ProductName, screen);

                GUILayout.Label("Quick adjustments:", screen.LabelStyle);
                using (LayoutHelper.LabelWidth(350))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(FastScriptReloadPreference.BatchScriptChangesAndReloadEveryNSeconds);
                    ProductPreferenceBase.RenderGuiAndPersistInput(FastScriptReloadPreference.EnableAutoReloadForChangedFiles);
                    ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.IsAssetIncludedInBuild);
                }
            }
        );
        
        private static readonly List<GuiSection> LeftSections = CreateLeftSections(new List<ChangeMainViewButton> { new ChangeMainViewButton("Network", (screen) =>
            {
                const int sectionBreakHeight = 15;
                
                EditorGUILayout.HelpBox("By default asset will automatically connect to editor running over the network using simple broadcast discovery." +
                                        "\r\n\r\nIf your device can't connect, likely your network doesn't allow it." +
                                        "\r\n\r\n- In first instance, make sure your Firewall allows connection to port specified below" +
                                        "\r\n- You can also force client to use specific IP instead of default network broadcast discovery", MessageType.Info);
                
                using (LayoutHelper.LabelWidth(380))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.AssemblyChangesOverNetwork_UsePort);
                }
                
                GUILayout.Space(sectionBreakHeight);
                EditorGUILayout.HelpBox(
                    "Only force specific IP address if auto discovery doesn't work." +
                    "\r\nIP address should point to a device that runs Unity editor where changes originate from" +
                    "\r\nIP address needs to be reachable from network that client is on" +
                    "\r\n\r\nMake sure IP address is in correct format, eg 86.59.251.139 - there's no validation",
                    MessageType.Warning
                );
                
                using (LayoutHelper.LabelWidth(380))
                {
                    ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.IsAssemblyChangesOverNetworkForceUseIPEnabled);

                    if ((bool)LiveScriptReloadPreference.IsAssemblyChangesOverNetworkForceUseIPEnabled.GetEditorPersistedValueOrDefault())
                    {
                        ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.AssemblyChangesOverNetworkUseIP);
                    }
                }
                
                EditorGUILayout.HelpBox("After changing client connection details you need to create new build.", MessageType.Warning);

                GUILayout.Label("Connected Client:", screen.LabelStyle);
                if (!Application.isPlaying)
                {
                    EditorGUILayout.HelpBox("Client can only connect in play-mode.", MessageType.Warning);
                }
                else
                {
                    //HACK: accessing Instance will create singleton in a scene, since it's not playmode it'll be persisted, check if exists before
                    var networkedAssemblyChangesSender = GameObject.FindObjectOfType<NetworkedAssemblyChangesSender>();
                    if (networkedAssemblyChangesSender == null)
                    {
                        GUILayout.Label("None");
                    }
                    else
                    {
                        if (NetworkedAssemblyChangesSender.Instance.ConnectedClientDetails != null)
                        {
                            GUILayout.Label($"{NetworkedAssemblyChangesSender.Instance.ConnectedClientDetails.Device}" +
                                            $"{(NetworkedAssemblyChangesSender.Instance.ConnectedClientDetails.IsEditor ? " (Editor)" : "")} " +
                                            $"from {NetworkedAssemblyChangesSender.Instance.ConnectedPeer?.EndPoint}");
                        }
                        else
                        {
                            GUILayout.Label("None");
                        }
                    }
                }
            }),
            (BuildSection = new ChangeMainViewButton("Build",
                (screen) =>
                {
                    const int sectionBreakHeight = 15;
                    
                    EditorGUILayout.HelpBox("By default tool will be included in any build for ease of testing.\r\n\r\nDo not include it in production builds as it can be exploited.",
                        MessageType.Warning
                    );
                    GUILayout.Space(sectionBreakHeight);

                    using (LayoutHelper.LabelWidth(350))
                    {
                        ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.IsAssetIncludedInBuild);
                        GUILayout.Space(sectionBreakHeight);
                        ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.IsBuildWarningDisplayedInUnityBuildWindow);
                        EditorGUILayout.HelpBox("Warning on build screen is designed to prevent anyone on the team from accidentally building release version with asset enabled." +
                                                $"\r\n\r\nIdeally you'll include removal of build symbol '{LiveScriptReloadPreference.BuildSymbol_LiveScriptReload_IncludeInBuild}' in your CI/publish process. ",
                            MessageType.Info
                        );
                        
                        GUILayout.Space(sectionBreakHeight);
                        ProductPreferenceBase.RenderGuiAndPersistInput(LiveScriptReloadPreference.StopShowingIL2CppBackendNotSupportedDialogBox);
                    }
                }
            ))
        }, new LaunchSceneButton("Basic Example", (s) => GetScenePath("ExampleSceneNetworked"), (screen) =>
        {
            GUILayout.Label(
                $@"Asset is very simple to use:

1) Build a standalone version of app via File -> Build settings
1a) choose your preferred target, for demo it's easiest to test local build eg Windows / Mac
1b) add single scene to build: 'ExampleSceneNetworked' (Assets/LiveScriptReload/Scenes/) 
2) Run created standalone build

3) Move to editor and hit play to start.
2) Go to 'FunctionLibrary.cs' (Assets/Plugins/FastScriptReload/Examples/Scripts/)",
                screen.TextStyle
            );

            CreateOpenFunctionLibraryOnRippleMethodButton();

            GUILayout.Label(
                @"3) Change 'Ripple' method (eg change line before return statement to 'p.z = v * 10'
4) Save file
5) Changes will be visible both in editor and in standalone instance you're running

Rinse and repeat for other platforms you want to use this one, eg Android",
                screen.TextStyle
            );
            
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("There are some limitations to what can be Hot-Reloaded, documentation lists them under 'limitations' section.", MessageType.Warning);
        }), MainScrollViewSection);
        
        private static readonly string RedirectBaseUrl = "https://immersivevrtools.com/redirect/live-script-reload"; 
        private static readonly GuiSection TopSection = CreateTopSectionButtons(RedirectBaseUrl);

        private static readonly GuiSection BottomSection = new GuiSection(
            "I want to make this tool better. And I need your help!",
            $"It'd be great if you could share your feedback (good and bad) with me. I'm very keen to make this tool better and that can only happen with your help. Please use:",
            new List<ClickableElement>
            {
                new OpenUrlButton(" Unity Forum", $"{RedirectBaseUrl}/unity-forum"),
                new OpenUrlButton(" or Write a Short Review", $"{RedirectBaseUrl}/asset-store-review"),
            }
        );

        public override string WindowTitle { get; } = _WindowTitle;
        public override Vector2 WindowSizePx { get; } = _WindowSizePx;


        [MenuItem("Window/Live Script Reload/Start Screen", false, 1999)]
        public new static LiveScriptReloadWelcomeScreen Init()
        {
            return OpenWindow<LiveScriptReloadWelcomeScreen>(_WindowTitle, _WindowSizePx);
        }
    
        [MenuItem("Window/Live Script Reload/Force Reload", true, 1999)]
        public new static bool ForceReloadValidate()
        {
            return EditorApplication.isPlaying;
        }
    
        [MenuItem("Window/Live Script Reload/Force Reload", false, 1999)]
        public new static void ForceReload()
        {
            FastScriptReloadManager.Instance.TriggerReloadForChangedFiles();
        }

        public new void OnEnable()
        {
            OnEnableCommon(ProjectIconName);
        }

        public new void OnGUI()
        {
            RenderGUI(LeftSections, TopSection, BottomSection, MainScrollViewSection);
        }
    }

    public class LiveScriptReloadPreference : FastScriptReloadPreference
    {
        public const string BuildSymbol_LiveScriptReload_IncludeInBuild = "LiveScriptReload_IncludeInBuild_Enabled";
        
        private static readonly int DefaultPort = 54213;

        public new const string ProductName = "Live Script Reload";
        private static string[] ProductKeywords = new[] { "productivity", "tools" };
        
        public static readonly IntProjectEditorPreferenceDefinition AssemblyChangesOverNetwork_UsePort = new IntProjectEditorPreferenceDefinition(
            "Network port in use for clients to receive Hot-Reload updates", "AssemblyChangesOverNetworkUsePort", DefaultPort,
            (newValue, oldValue) => AdjustNetworkConfiguration(c => c.PortInUse = (int)newValue),
            (value) => AdjustNetworkConfiguration(c => c.PortInUse = (int)value)
        );

        public static readonly ToggleProjectEditorPreferenceDefinition IsAssemblyChangesOverNetworkForceUseIPEnabled = new ToggleProjectEditorPreferenceDefinition(
            "Force specific IP address for clients to receive Hot-Reload updates", "IsAssemblyChangesOverNetworkForceUseIPEnabled", false,
            (newValue, oldValue) => AdjustNetworkConfiguration(c => c.IsUseSpecificIpAddressEnabled = (bool)newValue),
            (val) => AdjustNetworkConfiguration(c => c.IsUseSpecificIpAddressEnabled = (bool)val)
        );
        
        public static readonly TextProjectEditorPreferenceDefinition AssemblyChangesOverNetworkUseIP = new TextProjectEditorPreferenceDefinition(
            "IP Address for clients to receive Hot-Reload updates", "AssemblyChangesOverNetworkUseIP", string.Empty,
            (newValue, oldValue) => AdjustNetworkConfiguration(c => c.UseSpecificIpAddress = (string)newValue),
            val => AdjustNetworkConfiguration(c => c.UseSpecificIpAddress = (string)val)
        );
        
        public static readonly ToggleProjectEditorPreferenceDefinition IsAssetIncludedInBuild = new ToggleProjectEditorPreferenceDefinition(
            "Enable Hot-Reload for build", "IsAssetIncludedInBuild", true,
            (newVal, oldVal) => BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_LiveScriptReload_IncludeInBuild, (bool)newVal),
            val => BuildDefineSymbolManager.SetBuildDefineSymbolState(BuildSymbol_LiveScriptReload_IncludeInBuild, (bool)val)
        );        
        public static readonly ToggleProjectEditorPreferenceDefinition IsBuildWarningDisplayedInUnityBuildWindow = new ToggleProjectEditorPreferenceDefinition(
            "Should unity Build Screen display Hot-Reload warning", "IsBuildWarningDisplayedInUnityBuildWindow", true);

        public static readonly ToggleProjectEditorPreferenceDefinition StopShowingIL2CppBackendNotSupportedDialogBox = new ToggleProjectEditorPreferenceDefinition(
            "Stop Showing IL2CPP backend not supported dialog", "DontShowIL2CppBackendNotSupportedDialogBox", false);

        private static void AdjustNetworkConfiguration(Action<NetworkConnectionConfiguration> adjustFn)
        {
            var configSo = NetworkConnectionConfiguration.GetDefaultInstance();
            adjustFn(configSo);
            EditorUtility.SetDirty(configSo);
        }
        
        public new static List<ProjectEditorPreferenceDefinitionBase> PreferenceDefinitions = new List<ProjectEditorPreferenceDefinitionBase>()
        {
            CreateDefaultShowOptionPreferenceDefinition(),
            BatchScriptChangesAndReloadEveryNSeconds,
            EnableAutoReloadForChangedFiles,
            EnableExperimentalThisCallLimitationFix,
            LogHowToFixMessageOnCompilationError,
            AssemblyChangesOverNetwork_UsePort,
            IsAssemblyChangesOverNetworkForceUseIPEnabled,
            AssemblyChangesOverNetworkUseIP,
            IsAssetIncludedInBuild,
            IsBuildWarningDisplayedInUnityBuildWindow,
            StopShowingIL2CppBackendNotSupportedDialogBox
        };

        private static bool PrefsLoaded = false;


#if UNITY_2019_1_OR_NEWER
        [SettingsProvider]
        public static SettingsProvider ImpostorsSettings()
        {
            return GenerateProvider(ProductName, ProductKeywords, PreferencesGUI);
        }

#else
	[PreferenceItem(ProductName)]
#endif
        public new static void PreferencesGUI()
        {
            if (!PrefsLoaded)
            {
                LoadDefaults(PreferenceDefinitions);
                PrefsLoaded = true;
            }

            RenderGuiCommon(PreferenceDefinitions);
        }
    }

    [InitializeOnLoad]
    public class LiveScriptReloadWelcomeScreenInitializer : FastScriptReloadWelcomeScreenInitializer
    {
        static LiveScriptReloadWelcomeScreenInitializer()
        {
            var userId = ProductPreferenceBase.CreateDefaultUserIdDefinition(LiveScriptReloadWelcomeScreen.ProjectName).GetEditorPersistedValueOrDefault().ToString();

            HandleUnityStartup(
                () => LiveScriptReloadWelcomeScreen.Init(),
                LiveScriptReloadWelcomeScreen.GenerateGetUpdatesUrl(userId, LiveScriptReloadWelcomeScreen.VersionId),
                new List<ProjectEditorPreferenceDefinitionBase>(),
                (isFirstRun) =>
                {
                    AutoDetectAndSetShaderMode();
                }
            );
            
            InitCommon();
            
            PerformIl2CppEnabledCheck();

            BuildDefineSymbolManager.SetBuildDefineSymbolState(LiveScriptReloadPreference.BuildSymbol_LiveScriptReload_IncludeInBuild,
                (bool)LiveScriptReloadPreference.IsAssetIncludedInBuild.GetEditorPersistedValueOrDefault()
            );
        }

        private static void PerformIl2CppEnabledCheck()
        {
            var scriptingBacked = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
            if (scriptingBacked == ScriptingImplementation.IL2CPP)
            {
                Debug.LogWarning("Live Script Reload - IL2CPP scripting backend is not supporting code Hot-Reload.");

                if (!(bool)LiveScriptReloadPreference.StopShowingIL2CppBackendNotSupportedDialogBox.GetEditorPersistedValueOrDefault())
                {
                    var chosenOption = EditorUtility.DisplayDialogComplex("Live Script Reload - not supported",
                        "IL2CPP scripting backend is not supporting code Hot-Reload." +
                        $"\n\nFor tool to properly work it needs to be using Mono backend." +
                        $"\r\n\r\nIt should be a seamless change and you can simply revert it back to IL2CPP for release build",
                        "Ok, change to Mono backend",
                        "No, don't change (stop showing this message)",
                        "No, don't change"
                    );

                    switch (chosenOption)
                    {
                        // change.
                        case 0:
                            PlayerSettings.SetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup, ScriptingImplementation.Mono2x);
                            break;

                        // don't change and stop showing message.
                        case 1:
                            LiveScriptReloadPreference.StopShowingIL2CppBackendNotSupportedDialogBox.SetEditorPersistedValue(true);

                            break;

                        // don't change
                        case 2:

                            break;

                        default:
                            Debug.LogError("Unrecognized option.");
                            break;
                    }
                }
            }
        }
    }
}
#endif