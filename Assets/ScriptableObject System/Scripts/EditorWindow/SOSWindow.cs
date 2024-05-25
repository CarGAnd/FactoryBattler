using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using SOS;
using UnityEditor;
using UnityEngine;

namespace SOS {
    public class SOSWindow : OdinMenuEditorWindow
    {
        private SOSSettings settings;

        [MenuItem("SOS/Settings")]
        private static void OpenWindow()
        {
            GetWindow<SOSWindow>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            settings = AssetDatabase.LoadAssetAtPath<SOSSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SOSSettings_01", null)[0]));

            OdinMenuTree tree = new OdinMenuTree(true, GetTreeConfig()) {
                    { "SOS Settings",                             settings,                                                   EditorIcons.SettingsCog },
                };

            return tree;
        }

        // Setup Odin Menu Tree with Search and caching expandend states
            private OdinMenuTreeDrawingConfig GetTreeConfig() {
                OdinMenuTreeDrawingConfig config = new OdinMenuTreeDrawingConfig
                {
                    DrawSearchToolbar = true,
                    DefaultMenuStyle = OdinMenuStyle.TreeViewStyle,
                    UseCachedExpandedStates = true
                };
                return config;
            }
    }
}

public static class EditorUtils
{
    public static string RootPath(Type script)
    {
        var g = AssetDatabase.FindAssets ( $"t:Script {nameof(script)}" );
        return AssetDatabase.GUIDToAssetPath ( g [ 0 ] );
    }
}