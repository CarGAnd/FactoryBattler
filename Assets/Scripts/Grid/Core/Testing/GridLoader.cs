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
        
    }

    /*[Button("Test Load")]
    private void Load() {
        GridSerializer<IGridObject> serializer = new GridSerializer<IGridObject>();
        List<SavedObjectData> savedObjects = serializer.JsonToGrid(jsonData);
        foreach(SavedObjectData savedObject in savedObjects) {
            PlaceSavedObject(savedObject.id, new Vector2Int(savedObject.xPosition, savedObject.yPosition), savedObject.objectJsonData);
        }
    }*/

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
        IGridObject gridObject = modulePlacer.TryPlaceModule(testObject, new Vector2Int(placementData.x, placementData.y), placementData.facing);
        //gridObject.Deserialize(placementData.buildingData);
    }
}

public class ObjectPlacementData {
    public string id;
    public int x;
    public int y;
    public Facing facing;
    public object buildingData;
}
