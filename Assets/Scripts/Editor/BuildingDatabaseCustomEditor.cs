using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.Utilities.Editor;
using System.Linq;

[CustomEditor(typeof(BuildingDatabase))]
public class BuildingDatabaseCustomEditor : Editor
{
    private SerializedProperty buildingsProperty;

    private void OnEnable() {
        buildingsProperty = serializedObject.FindProperty("buildings");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        if (GUILayout.Button("Refresh")) {
            FindAllGridObjects();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void FindAllGridObjects() {
        List<GridObjectSO> gridObjects = AssetUtilities.GetAllAssetsOfType<GridObjectSO>().ToList();
        buildingsProperty.arraySize = gridObjects.Count;

        for(int i = 0; i < gridObjects.Count; i++) {
            buildingsProperty.GetArrayElementAtIndex(i).objectReferenceValue = gridObjects[i];
        }
    }

}
