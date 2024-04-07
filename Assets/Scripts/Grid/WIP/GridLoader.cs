using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour
{
    [SerializeField] private FactoryGrid factoryGrid;
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private GridObjectSO testObject;

    private string jsonData;

    [Button("Test Save")]
    private void Save() {
        GridSerializer serializer = new GridSerializer();
        jsonData = serializer.GridToJson(factoryGrid);
    }

    [Button("Test Load")]
    private void Load() {
        GridSerializer serializer = new GridSerializer();
        List<ObjectPlacementData> savedObjects = serializer.JsonToGridObjects(jsonData);
        foreach(ObjectPlacementData savedObject in savedObjects) {
            PlaceSavedObject(savedObject);
        }
    }

    [Button("Clear Grid")]
    private void ClearGrid() {
        for(int x = 0; x < factoryGrid.Columns; x++) {
            for(int y = 0; y < factoryGrid.Rows; y++) {
                Vector2Int gridPosition = new Vector2Int(x, y);
                IGridObject gridObject = factoryGrid.GetObjectAt(gridPosition);
                if(gridObject != null) {
                    gridObject.DestroyObject();
                }
            }
        }
    }

    private void PlaceSavedObject(ObjectPlacementData placementData) {
        IGridObject gridObject = modulePlacer.PlaceModule(testObject, new Vector2Int(placementData.x, placementData.y), placementData.facing);
    }
}

public class ObjectPlacementData {
    public string prefabId;
    public int x;
    public int y;
    public Facing facing;
    public object buildingData;
}
