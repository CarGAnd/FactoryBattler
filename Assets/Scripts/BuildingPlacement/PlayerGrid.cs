using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;

using PlayerSystem;
using UnityEngine;

public class PlayerGrid : MonoBehaviour, IPlayerGrid
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

    public IPlayer Owner { get => owner; set => owner = value; }
    public FactoryGrid FactoryGrid { get => grid; }

    private IPlayer owner;
    private ModulePlacer modulePlacer;

    private void Awake() {
        this.modulePlacer = new ModulePlacer(grid);
    }

    public IGridObject TryPlaceBuilding(GridObjectSO building, Vector2Int position, Facing rotation) {
        IGridObject newBuilding = null;
        
        if(modulePlacer.CanPlaceModule(building, position, rotation)) {
            newBuilding = modulePlacer.PlaceModule(building, position, rotation);
            SetupBuilding(newBuilding, position);
        }
        return newBuilding;
    }

    private void SetupBuilding(IGridObject building, Vector2Int gridPosition) {
        if(building is IAssemblyLineUser assemblyLineUser) {
            assemblyLineUser.ConnectToAssemblyLine(assemblyLineSystem);
        }
        if (building is IPlayerOwned playerOwned) {
            playerOwned.SetOwner(Owner);
        }
        
        building.OnPlacedOnGrid(gridPosition, grid);
    }

    public void ConnectExistingBuildingToGrid(GridObjectSO buildingData, IGridObject gridObject, Vector2Int gridPos, Facing rotation) {
        modulePlacer.PlaceExistingBuilding(gridObject, gridPos, buildingData.GetLayoutShape(rotation));
    }

    public List<IGridObject> GetAllPlacedBuildings() {
        return grid.GetAllPlacedObjects();
    }

    void IPlayerGrid.TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation) {
        TryPlaceBuilding(buildingData, coord, rotation);
    }

    public IGridObject GetBuildingAtPosition(Vector2Int position) {
        return grid.GetObjectAt(position);
    }

    public void RemoveBuilding(Vector2Int position) {
        modulePlacer.RemoveModule(position);
    }
}

public interface IPlayerGrid : IPlayerOwned {
    FactoryGrid FactoryGrid { get; }
    void TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation);
    void RemoveBuilding(Vector2Int position);
}
