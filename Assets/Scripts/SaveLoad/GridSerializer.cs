using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

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

    public string GridToJson(FactoryGrid grid) {
        List<ObjectPlacementData> savedObjects = SaveGrid(grid);
        return JsonConvert.SerializeObject(savedObjects);
    }

    public List<ObjectPlacementData> JsonToGridObjects(string jsonData) {
        List<ObjectPlacementData> savedObjects = JsonConvert.DeserializeObject<List<ObjectPlacementData>>(jsonData);
        return savedObjects;
    }
}

public interface ISaveable {
    ObjectPlacementData Serialize();
    void Deserialize(ObjectPlacementData data);
}

