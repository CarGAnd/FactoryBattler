using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ModuleSO))]
public class ModuleSOCustomEditor : Editor 
{
    private ModuleSO targetModuleSO;

    private SerializedProperty modulePrefab;
    private SerializedProperty previewPrefab;

    private SerializedProperty boolMatrix;
    private SerializedProperty boolArray;

    private SerializedProperty portLayout;
    private SerializedProperty inputPortArray;
    private SerializedProperty outputPortArray;
    private SerializedProperty portLayoutWidth;
    private SerializedProperty portLayoutHeight;
    private SerializedProperty IdProperty;

    private bool layoutFoldedOut;

    private void OnEnable() {
        targetModuleSO = (ModuleSO)target;

        modulePrefab = serializedObject.FindProperty("<ModulePrefab>k__BackingField");
        previewPrefab = serializedObject.FindProperty("<PreviewPrefab>k__BackingField");

        boolMatrix = serializedObject.FindProperty("buildingLayout");
        boolArray = boolMatrix.FindPropertyRelative("values");

        portLayout = serializedObject.FindProperty("portLayout");
        inputPortArray = portLayout.FindPropertyRelative("inputs");
        outputPortArray = portLayout.FindPropertyRelative("outputs");
        portLayoutWidth = portLayout.FindPropertyRelative("width");
        portLayoutHeight = portLayout.FindPropertyRelative("height");

        IdProperty = serializedObject.FindProperty("id");
    }

    public override void OnInspectorGUI() {
        EditorGUILayout.PropertyField(IdProperty);
        EditorGUILayout.PropertyField(modulePrefab);
        EditorGUILayout.PropertyField(previewPrefab);
        EditorGUILayout.PropertyField(boolMatrix);

        Vector2 matrixButtonSize = new Vector2(35, 35);
        inputPortArray.arraySize = targetModuleSO.Width * targetModuleSO.Height;
        outputPortArray.arraySize = targetModuleSO.Width * targetModuleSO.Height;
        portLayoutWidth.intValue = targetModuleSO.Width;
        portLayoutHeight.intValue = targetModuleSO.Height;

        layoutFoldedOut = EditorGUILayout.Foldout(layoutFoldedOut, "Ports");
        if (layoutFoldedOut) {
            EditorGUILayout.LabelField("Inputs");
            
            Rect inputRect = EditorGUILayout.GetControlRect();
            GUILayoutUtility.GetRect(inputRect.width, matrixButtonSize.y * targetModuleSO.Height);

            Vector2 inputStart = new Vector2(inputRect.width / 2f - matrixButtonSize.x * targetModuleSO.Width / 2f, inputRect.y);

            DrawIntMatrix(inputPortArray, targetModuleSO.Width, targetModuleSO.Height, matrixButtonSize, inputStart);

            EditorGUILayout.LabelField("Outputs");

            Rect outputRect = EditorGUILayout.GetControlRect();
            GUILayoutUtility.GetRect(outputRect.width, matrixButtonSize.y * targetModuleSO.Height);

            Vector2 outputStart = new Vector2(outputRect.width / 2f - matrixButtonSize.x * targetModuleSO.Width / 2f, outputRect.y);

            DrawIntMatrix(outputPortArray, targetModuleSO.Width, targetModuleSO.Height, matrixButtonSize, outputStart);
        }

        serializedObject.ApplyModifiedProperties();
    }

    private void DrawIntMatrix(SerializedProperty intArray, int width, int height, Vector2 cellSize, Vector2 startPosition) {
        string[] textMapping = new string[] {"", "↑", "→", "↓", "←"};
        Color oldColor = GUI.color;
        Vector2 matrixStart = new Vector2(startPosition.x, startPosition.y);
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                int arrayIndex = y * width + x;
                int cellValue = 0;

                bool isValidPortPosition = boolArray.GetArrayElementAtIndex(arrayIndex).boolValue;
                if (!isValidPortPosition) {
                    intArray.GetArrayElementAtIndex(arrayIndex).intValue = 0;
                    GUI.color = Color.red;
                }
                else {
                    cellValue = intArray.GetArrayElementAtIndex(arrayIndex).intValue;
                    GUI.color = cellValue > 0 ? Color.green : oldColor;
                }
                
                Rect buttonRect = new Rect(matrixStart.x + cellSize.x * x, matrixStart.y + cellSize.y * y, cellSize.x, cellSize.y);
                if(GUI.Button(buttonRect, textMapping[cellValue])) {
                    intArray.GetArrayElementAtIndex(arrayIndex).intValue = (intArray.GetArrayElementAtIndex(arrayIndex).intValue + 1) % textMapping.Length;
                }
            }
        }
        GUI.color = oldColor;
    }

}
