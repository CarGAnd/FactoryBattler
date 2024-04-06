using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;
using GridSystem;

[CustomEditor(typeof(CustomGridMap))]
public class CustomGridMapEditor : Editor
{
    private SerializedProperty gridSizeProperty;
    private SerializedProperty gridLayoutProperty;
    private SerializedProperty customSpawnMask;
    private SerializedProperty showCustomLayout;
    private SerializedProperty currentHeight;
    private SerializedProperty currentWidth;

    private CustomGridMap customLayout;
    private GridSize gridSize;
    private GridSystem.GridLayout gridLayout;

    private bool isPainting;
    private bool leftMousePressed;
    private bool rightMousePressed;

    private void OnEnable() {
        customLayout = (CustomGridMap)target;
        gridSizeProperty = serializedObject.FindProperty("gridSize");
        gridLayoutProperty = serializedObject.FindProperty("gridLayout");
        customSpawnMask = serializedObject.FindProperty("customSpawnMask");
        showCustomLayout = serializedObject.FindProperty("showCustomLayout");
        currentHeight = serializedObject.FindProperty("currentHeight");
        currentWidth = serializedObject.FindProperty("currentWidth");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        DrawPaintButton();

        gridLayout = (GridSystem.GridLayout)gridLayoutProperty.objectReferenceValue;
        gridSize = (GridSize)gridSizeProperty.objectReferenceValue;
        if(gridSize == null) {
            return;
        }

        if (isPainting) {
            if(GUILayout.Button("Fill Grid")) {
                FillGrid();
            }

            if(GUILayout.Button("Clear Grid")) {
                ClearGrid();
            }
        }

        if(gridSize.Rows != currentHeight.intValue || gridSize.Columns != currentWidth.intValue || customSpawnMask.arraySize != currentHeight.intValue * currentWidth.intValue) {
            Undo.RegisterCompleteObjectUndo(customLayout, "Resize");
            customSpawnMask.arraySize = currentWidth.intValue * currentHeight.intValue;
            bool[] newMask = new bool[gridSize.Rows * gridSize.Columns];
            int newWidth = Mathf.Min(currentWidth.intValue, gridSize.Columns);
            for (int y = 0; y < Mathf.Min(currentHeight.intValue, gridSize.Rows); y++) {
                for(int x = 0; x < newWidth; x++) {
                    newMask[y * gridSize.Columns + x] = customSpawnMask.GetArrayElementAtIndex(y * currentWidth.intValue + x).boolValue;
                }
            }
            currentWidth.intValue = gridSize.Columns;
            currentHeight.intValue = gridSize.Rows;
            customSpawnMask.arraySize = currentWidth.intValue * currentHeight.intValue;

            for (int y = 0; y < currentHeight.intValue; y++) {
                for (int x = 0; x < currentWidth.intValue; x++) {
                    int index = y * currentWidth.intValue + x;
                    customSpawnMask.GetArrayElementAtIndex(index).boolValue = newMask[index];
                }
            }
            EditorWindow view = EditorWindow.GetWindow<SceneView>();
            view.Repaint();
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void SetAllGridValues(bool value) {
        for(int i = 0; i < customSpawnMask.arraySize; i++) {
            customSpawnMask.GetArrayElementAtIndex(i).boolValue = value;
        }
    }

    private void SetGridValue(int x, int y, bool value) {
        int index = currentWidth.intValue * y + x;
        customSpawnMask.GetArrayElementAtIndex(index).boolValue = value;
    }

    private void FillGrid() {
        SetAllGridValues(true);
    }

    private void ClearGrid() {
        SetAllGridValues(false);
    }

    private void TogglePaintMode() {
        isPainting = !isPainting;
        if (isPainting) {
            showCustomLayout.boolValue = true;
        }
    }

    private void DrawPaintButton() {
        Color guiColor = GUI.color;
        Color c = isPainting ? Color.red : Color.green;
        string text = isPainting ? "Stop Painting" : "Start Painting";
        GUI.color = c;

        if(GUILayout.Button(text)) {
            TogglePaintMode();
        }

        GUI.color = guiColor;
    }

    private void OnSceneGUI() {
        if (!isPainting) {
            return;
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) {
            leftMousePressed = true;
            e.Use();
        }
        if (e.type == EventType.MouseUp && e.button == 0) {
            leftMousePressed = false;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1) {
            rightMousePressed = true;
            e.Use();
        }
        if (e.type == EventType.MouseUp && e.button == 1) {
            rightMousePressed = false;
            e.Use();
        }

        if (leftMousePressed) {
            Paint(e.mousePosition, true);
        }

        if (rightMousePressed) {
            Paint(e.mousePosition, false);
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void Paint(Vector2 clickPosition, bool value) {
        Ray ray = HandleUtility.GUIPointToWorldRay(clickPosition);
        Vector3 worldPosition = gridLayout.RaycastGridPlane(ray);
        Vector2Int gridPosition = gridLayout.WorldToGrid(worldPosition);
        SetGridValue(gridPosition.x, gridPosition.y, value);
        //Manually update the scene view as the update rate would otherwise be very choppy
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();
    }
}
