using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class BuildingDatabase : ScriptableObject
{
    [SerializeField] private List<GridObjectSO> buildings;

    private Dictionary<string, GridObjectSO> database;

    private void OnEnable() {
        database = new Dictionary<string, GridObjectSO>();
        foreach(GridObjectSO building in buildings) {
            database.Add(building.ID, building);
        }
    }

    public GridObjectSO GetBuildingByID(string id) {
        GridObjectSO building;
        database.TryGetValue(id, out building);
        return building;
    }
}
