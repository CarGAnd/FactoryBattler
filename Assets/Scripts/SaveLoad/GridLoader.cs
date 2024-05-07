using PlayerSystem;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour
{
    [SerializeField] private BuildingDatabase buildingDatabase;

    public string SaveGrid(FactoryGrid grid) {
        GridSerializer serializer = new GridSerializer();
        string jsonData = serializer.GridToJson(grid);
        return jsonData;
    }

    public void LoadDataToGrid(FactoryGrid grid, Builder builder, string jsonData) {
        grid.ClearGrid();
        GridSerializer serializer = new GridSerializer();
        List<ObjectPlacementData> savedObjects = serializer.JsonToGridObjects(jsonData);
        foreach (ObjectPlacementData savedObject in savedObjects) {
            PlaceSavedObject(savedObject, builder);
        }
    }

    private void PlaceSavedObject(ObjectPlacementData placementData, Builder builder) {
        IGridObject gridObject = builder.TryPlaceBuilding(buildingDatabase.GetBuildingByID(placementData.prefabId), new Vector2Int(placementData.x, placementData.y), placementData.facing);
    }
}

public class ObjectPlacementData {
    public string prefabId;
    public int x;
    public int y;
    public Facing facing;
    public object buildingData;
}
