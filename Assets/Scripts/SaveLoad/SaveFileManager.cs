using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveFileManager
{
	public static string GetSavePath(string saveName) {
		string saveGamePath = string.Concat(Application.persistentDataPath, "/", saveName, ".json");
		return saveGamePath;
    }

	public static string GetSaveFolder() {
		return Application.persistentDataPath;
    }

    public static void SaveData(string data, string saveName)
	{
		string saveGamePath = GetSavePath(saveName);
		File.WriteAllText(saveGamePath, data);
	}

    public static string LoadData(string saveName)
	{
		if (SaveFileExists(saveName))
		{
			string saveGamePath = GetSavePath(saveName);
			string text = File.ReadAllText(saveGamePath);
			return text;
		}
		else
		{
			return null;
		}
	}

	public static bool SaveFileExists(string saveName)
	{
		string saveGamePath = GetSavePath(saveName);
		return File.Exists(saveGamePath);
	}
	
	public static List<string> GetAvailableSaves() {
        List<string> saveNames = new List<string>();
        string saveFolder = GetSaveFolder();
        string[] saveFilesPaths = Directory.GetFiles(saveFolder);

        foreach(string path in saveFilesPaths) {
			string fullFileName = Path.GetFileName(path);
            string onlyFileName = fullFileName.Split('.')[0];
            saveNames.Add(onlyFileName);
        }
        return saveNames;
    }

	public static void DeleteSave(string saveName) {
        if (SaveFileExists(saveName)) {
			string savePath = GetSavePath(saveName);
			File.Delete(savePath);
        }
        else {
			Debug.LogWarning("Save file with the name " + saveName + " does not exist");
        }
    }
}
