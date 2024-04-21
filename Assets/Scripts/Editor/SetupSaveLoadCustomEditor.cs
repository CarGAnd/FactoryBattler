using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SetupSaveLoad))]
public class SetupSaveLoadCustomEditor : Editor 
{
    private SetupSaveLoad setupSaver;

    private int selectedSaveNumber;
    private string saveName;

    private void OnEnable() {
        setupSaver = (SetupSaveLoad)target;
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();

        GUI.enabled = EditorApplication.isPlaying;

        EditorGUILayout.Space(10);

        EditorGUILayout.LabelField("Save", EditorStyles.boldLabel);
        saveName = EditorGUILayout.TextField("Save Name", saveName);

        if(GUILayout.Button("Save To File")) {
            if (!string.IsNullOrEmpty(saveName)) { 
                setupSaver.SaveSetupToFile(saveName);
            }
            else {
                Debug.LogWarning("Cannot create a save with an empty name");
            }
        }

        if(GUILayout.Button("Save to Clipboard")) {
            setupSaver.SaveSetupToClipboard();
        }

        EditorGUILayout.Space(5);
        List<string> saveFileNames = SaveFileManager.GetAvailableSaves();

        EditorGUILayout.LabelField("Load", EditorStyles.boldLabel);
        selectedSaveNumber = EditorGUILayout.Popup("Save Name", selectedSaveNumber, saveFileNames.ToArray());

        if(GUILayout.Button("Load from file")) {
            if(selectedSaveNumber < saveFileNames.Count) {
                setupSaver.LoadFromFile(saveFileNames[selectedSaveNumber]);
            }
            else {
                Debug.LogWarning("Selected save is out of range. You likely don't have any saves created");
            }
        }

        if(GUILayout.Button("Load from clipboard")) {
            setupSaver.LoadFromClipBoard();
        }

        if(GUILayout.Button("Load save to clipboard")) {
            if(selectedSaveNumber < saveFileNames.Count) {
                setupSaver.LoadFileToClipBoard(saveFileNames[selectedSaveNumber]);
            }
            else {
                Debug.LogWarning("Selected save is out of range. You likely don't have any saves created");
            }
        }

        EditorGUILayout.Space(5);

        Color oldGuiColor = GUI.color;
        GUI.color = Color.red;
        if(GUILayout.Button("Delete save")) {
            if(selectedSaveNumber < saveFileNames.Count) {
                SaveFileManager.DeleteSave(saveFileNames[selectedSaveNumber]);
            }
            else {
                Debug.LogWarning("Selected save is out of range. You likely don't have any saves created");
            }
        }
        GUI.color = oldGuiColor;
        GUI.enabled = true;
    }
}
