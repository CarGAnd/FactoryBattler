using UnityEditor;
using UnityEngine;
using System.Linq;
using System.Text;
using System.IO;

namespace AttributeSystem {
    public class AttributeKeysAssetPostprocessor : AssetPostprocessor
    {
        private static readonly string AttributesPath = "Assets/Attribute System/ScriptableObjects/Attributes";
        private static readonly string OutputFile = "Assets/Attribute System/Scripts/AutoGeneration/AttributeKeys.cs";

        private static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            var allAffectedAssets = importedAssets.Concat(deletedAssets).Concat(movedAssets).Concat(movedFromAssetPaths);

            bool scriptableObjectChanged = allAffectedAssets.Any(assetPath => IsScriptableObject(assetPath));
            bool isRelevantPath = allAffectedAssets.Any(assetPath => IsRelevantPath(assetPath));

            if (scriptableObjectChanged || isRelevantPath)
            {
                Debug.Log("ScriptableObject has changed. Updating AttributeKeys...");
                GenerateAttributeKeys();
            }
        }

        private static bool IsRelevantPath(string assetPath)
        {
            // Normalize the asset path
            string normalizedPath = assetPath.Replace("\\", "/").Trim();
            return normalizedPath.StartsWith(AttributesPath, System.StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsScriptableObject(string assetPath)
        {
            Debug.Log($"Path checked: {assetPath}");
            var asset = AssetDatabase.LoadAssetAtPath<Object>(assetPath);
            return asset is ScriptableObject;
        }

        private static void GenerateAttributeKeys()
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("// !IMPORTANT: This file is auto-generated. Modifications are not saved.");
            builder.AppendLine("public static class AttributeKeys");
            builder.AppendLine("{");

            var attributes = AssetDatabase.FindAssets("t:ScriptableObject", new[] { AttributesPath })
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(Path.GetFileNameWithoutExtension)
                .Distinct()
                .OrderBy(name => name);

            foreach (var attr in attributes)
            {
                builder.AppendLine($"    public const string {attr} = \"{attr}\";");
            }

            builder.AppendLine("}");

            File.WriteAllText(OutputFile, builder.ToString());
            AssetDatabase.Refresh();

            Debug.Log("AttributeKeys generated at " + OutputFile);
        }
    }
}