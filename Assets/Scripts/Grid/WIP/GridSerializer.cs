using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using GridSystem;

public class GridSerializer<T> where T : ISaveable
{
    private List<object> SaveGrid(CellGrid<T> grid) {
        List<object> savedObjects = new List<object>();
        List<T> gridObjects = grid.GetAllPlacedObjects();
        foreach(T gridObject in gridObjects) {
            object objectSaveData = gridObject.Serialize();
            savedObjects.Add(objectSaveData);
        }
        return savedObjects;
    }    

    private string SerializeSavedObjects(List<object> savedObjects) {
        return JsonConvert.SerializeObject(savedObjects);
    }

    private List<object> DeserializeSavedObjects(string jsonData) {
        return JsonConvert.DeserializeObject<List<object>>(jsonData);
    }

    public string GridToJson(CellGrid<T> grid) {
        List<object> savedObjects = SaveGrid(grid);
        return SerializeSavedObjects(savedObjects);
    }

    public List<object> JsonToGridObjects(string jsonData) {
        List<object> savedObjects = DeserializeSavedObjects(jsonData);
        return savedObjects;
    }
}

public interface ISaveable {
    object Serialize();
    void Deserialize(object data);
}

