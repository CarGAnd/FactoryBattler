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

    public string Base64Decode(string base64EncodedData) {
        byte[] base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    public string Base64Encode(string plainText) {
        byte[] plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
        return System.Convert.ToBase64String(plainTextBytes);
    }
}

public class ObjectPlacementData {
    public string prefabId;
    public int x;
    public int y;
    public Facing facing;
    public object buildingData;
}
