using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(BoolMatrix))]
public class BoolMatrixPropertyDrawer : PropertyDrawer
{
    private int lastWidth;
    private int lastHeight;

    private float lineSpacing = 2;
    private Vector2 checkboxSize = new Vector2(25, 25);

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
        property.isExpanded = EditorGUI.Foldout(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), property.isExpanded, label);

        if (!property.isExpanded) {
            EditorGUI.EndProperty();
            return;
        }

        int indentLevel = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 1;

        Rect widthRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
        Rect heightRect = new Rect(position.x, position.y + lineSpacing + EditorGUIUtility.singleLineHeight * 2, position.width, EditorGUIUtility.singleLineHeight);
        Rect matrixRect = new Rect(position.width / 2 - checkboxSize.x * lastWidth / 2, heightRect.y + lineSpacing + EditorGUIUtility.singleLineHeight + 10, checkboxSize.x * lastWidth, checkboxSize.y * lastHeight);
        Rect clearButtonRect = new Rect(position.x, matrixRect.y + matrixRect.height + EditorGUIUtility.singleLineHeight, position.width / 2, 30);
        Rect fillButtonRect = new Rect(position.width / 2, matrixRect.y + matrixRect.height + EditorGUIUtility.singleLineHeight, position.width / 2, 30);

        SerializedProperty widthProperty = property.FindPropertyRelative("<Width>k__BackingField");
        SerializedProperty heightProperty = property.FindPropertyRelative("<Height>k__BackingField");
        SerializedProperty boolArray = property.FindPropertyRelative("values");

        EditorGUI.PropertyField(widthRect, widthProperty, new GUIContent("Width"));
        EditorGUI.PropertyField(heightRect, heightProperty, new GUIContent("Height"));

        if(widthProperty.intValue != lastWidth || heightProperty.intValue != lastHeight) {
            boolArray.arraySize = widthProperty.intValue * heightProperty.intValue;
            lastWidth = widthProperty.intValue;
            lastHeight = heightProperty.intValue;
        }

        string[] buttonTexts = new string[boolArray.arraySize];
        for(int i = 0; i < buttonTexts.Length; i++) {
            buttonTexts[i] = boolArray.GetArrayElementAtIndex(i).boolValue ? "O" : "";
        }
        
        Vector2 matrixStart = new Vector2(matrixRect.x, matrixRect.y);
        DrawBoolMatrix(boolArray, widthProperty.intValue, heightProperty.intValue, checkboxSize, matrixStart);
        
        if(GUI.Button(clearButtonRect, new GUIContent("Clear"))) {
            for(int i = 0; i < boolArray.arraySize; i++) {
                boolArray.GetArrayElementAtIndex(i).boolValue = false;
            }
        }
        
        if(GUI.Button(fillButtonRect, new GUIContent("Fill"))) {
            for(int i = 0; i < boolArray.arraySize; i++) {
                boolArray.GetArrayElementAtIndex(i).boolValue = true;
            }
        }

        EditorGUI.indentLevel = indentLevel;
        EditorGUI.EndProperty();
    }

    private void DrawBoolMatrix(SerializedProperty boolArray, int width, int height, Vector2 cellSize, Vector2 startPosition) {
        Color oldColor = GUI.color;
        Vector2 matrixStart = new Vector2(startPosition.x, startPosition.y);
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                int arrayIndex = y * width + x;
                GUI.color = boolArray.GetArrayElementAtIndex(arrayIndex).boolValue ? Color.green : Color.red; 
                Rect buttonRect = new Rect(matrixStart.x + cellSize.x * x, matrixStart.y + cellSize.y * y, cellSize.x, cellSize.y);
                if(GUI.Button(buttonRect, "")) {
                    boolArray.GetArrayElementAtIndex(arrayIndex).boolValue = !boolArray.GetArrayElementAtIndex(arrayIndex).boolValue;
                }
            }
        }
        GUI.color = oldColor;
    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label) {
        if (!property.isExpanded) {
            return EditorGUIUtility.singleLineHeight;
        }
        return 2 * lineSpacing + 
               4 *  EditorGUIUtility.singleLineHeight + 
               lastHeight * checkboxSize.y + 
               30 + 10;
    }
}
