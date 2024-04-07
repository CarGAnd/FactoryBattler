using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using GridSystem;

public class GridSerializer
{
    private List<ObjectPlacementData> SaveGrid(FactoryGrid grid) {
        List<ObjectPlacementData> savedObjects = new List<ObjectPlacementData>();
        List<IGridObject> gridObjects = grid.GetAllPlacedObjects();
        foreach(IGridObject gridObject in gridObjects) {
            ObjectPlacementData objectSaveData = gridObject.Serialize();
            savedObjects.Add(objectSaveData);
        }
        return savedObjects;
    }    

    private string SerializeSavedObjects(List<ObjectPlacementData> savedObjects) {
        return JsonConvert.SerializeObject(savedObjects);
    }

    private List<ObjectPlacementData> DeserializeSavedObjects(string jsonData) {
        return JsonConvert.DeserializeObject<List<ObjectPlacementData>>(jsonData);
    }

    public string GridToJson(FactoryGrid grid) {
        List<ObjectPlacementData> savedObjects = SaveGrid(grid);
        return SerializeSavedObjects(savedObjects);
    }

    public List<ObjectPlacementData> JsonToGridObjects(string jsonData) {
        List<ObjectPlacementData> savedObjects = DeserializeSavedObjects(jsonData);
        return savedObjects;
    }
}

public interface ISaveable {
    ObjectPlacementData Serialize();
    void Deserialize(ObjectPlacementData data);
}

