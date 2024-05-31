using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

public class SetupSaveLoad : MonoBehaviour
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private PlayerGrid builder;
    [SerializeField] private GridLoader gridLoader;

    public void SaveSetupToFile(string saveName) {
        string jsonData = gridLoader.SaveGrid(grid);
        SaveFileManager.SaveData(jsonData, saveName);
    }

    public void SaveSetupToClipboard() {
        string jsonData = gridLoader.SaveGrid(grid);
        GUIUtility.systemCopyBuffer = jsonData;
    }

    public void LoadFromFile(string saveName) {
        string jsonData = SaveFileManager.LoadData(saveName);
        gridLoader.LoadDataToGrid(grid, builder, jsonData);
    }

    public void LoadFromClipBoard() {
        string jsonData = GUIUtility.systemCopyBuffer;
        gridLoader.LoadDataToGrid(grid, builder, jsonData);
    }

    public void LoadFileToClipBoard(string saveName) {
        string jsonData = SaveFileManager.LoadData(saveName);
        GUIUtility.systemCopyBuffer = jsonData;
    }

    public string Base64Decode(string base64EncodedData) {
        byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string Base64Encode(string plainText) {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }

}
