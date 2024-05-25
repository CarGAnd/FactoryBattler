using UnityEditor;
using System;

namespace SOS {
    public static class SettingsHelperSOS
    {
        public static SOSSettings GetSettings() {
            return AssetDatabase.LoadAssetAtPath<SOSSettings>(AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("SOSSettings_01", null)[0]));
        }
    }
}
