using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System;
using System.IO;

public class FolderStructure : EditorWindow
{
    private string[] folders =
    {
        "Resources",
        "Editor",
        "Art",
            "Art/Animation",
            "Art/Materials",
            "Art/Meshes",
            "Art/Shaders",
            "Art/Textures",
            "Art/UI",
                "Art/UI/Assets",
                "Art/UI/Fonts",
                "Art/UI/Icons",
        "Imported Packages",
        "Prefabs",
            "Prefabs/Art",
        "Audio",
        "Features",
        "UI",
        "Scenes",
            "Scenes/Alpha Scenes",
            "Scenes/Beta Scenes",
            "Scenes/Feature Scenes",
            "Scenes/Prototype Scenes",
        "Scripts",
            "Scripts/General Helper",
            "Scripts/System Managers",
            "Scripts/Example Feature",
                "Scripts/Example Feature/Managers",
                "Scripts/Example Feature/Core",
                "Scripts/Example Feature/Helper",
                "Scripts/Example Feature/Import",
                "Scripts/Example Feature/Scriptable Objects",
        "Scriptable Objects",
            "Scriptable Objects/Example Feature",
    };

    private string baseFolderPath;

    private TextField pathTextField;
    private Toggle createTextFilesToggle;

    [MenuItem("GumboGames/FolderTools")]
    public static void ShowFolderWindow() {
        EditorWindow wnd = GetWindow<FolderStructure>();
        wnd.titleContent = new GUIContent("Folder Tools");
    }

    private void Awake() {
        baseFolderPath = Application.dataPath;
    }

    public void CreateGUI() {
        Button createFoldersButton = new Button(new Action(CreateFolders));
        Label buttonText = new Label("Create folders");
        createFoldersButton.Add(buttonText);
        
        pathTextField = new TextField("Base Path");
        pathTextField.value = baseFolderPath;

        createTextFilesToggle = new Toggle("Create txt files for git");

        Button choosePathButton = new Button(new Action(ChooseFolderPath));
        choosePathButton.Add(new Label("Change path"));
        createTextFilesToggle.bindingPath = "createTextFilesForGit";

        rootVisualElement.Add(pathTextField);
        rootVisualElement.Add(createTextFilesToggle);
        rootVisualElement.Add(choosePathButton);
        rootVisualElement.Add(createFoldersButton);   
    }

    private void CreateFolders() {
        CreateFolders(baseFolderPath, folders);
    }

    private void ChooseFolderPath() {
        string openPath = baseFolderPath;
        if (String.IsNullOrEmpty(openPath)) {
            baseFolderPath = Application.dataPath;
        }
        string chosenPath = EditorUtility.OpenFolderPanel("Choose base folder path", baseFolderPath, "");
        baseFolderPath = chosenPath;
        pathTextField.value = baseFolderPath;
    }

    private void CreateFolders(string baseFolderPath, string[] folderNames) {
        foreach(string folder in folderNames) {
            string fullFolderPath = Path.Combine(baseFolderPath, folder);
            if (!Directory.Exists(fullFolderPath)) {
                Directory.CreateDirectory(fullFolderPath);
            }
            if (createTextFilesToggle.value) {
                string textFilePath = Path.Combine(fullFolderPath, "txtForGit.txt");
                using (StreamWriter sw = File.CreateText(textFilePath)) {

                }
            }
        }
        AssetDatabase.Refresh();
    }
}
