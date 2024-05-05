using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Builder : MonoBehaviour, IBuilder
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

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

    public Vector2Int GetBuildingPlacementPosition(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        return modulePlacer.GetModulePlacementPosition(moduleData, mouseHitPosition, facing);    
    }

    public void SetupBuilding(IGridObject building, Vector2Int gridPosition) {
        if(building is IAssemblyLineUser) {
            ((IAssemblyLineUser)building).ConnectToAssemblyLine(assemblyLineSystem);
        }
        building.OnPlacedOnGrid(gridPosition, grid);
    }

    public List<IGridObject> GetAllPlacedBuildings() {
        return grid.GetAllPlacedObjects();
    }

    void IBuilder.TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation) {
        TryPlaceBuilding(buildingData, coord, rotation);
    }
}

public interface IBuilder {
    public void TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation);
    public Vector2Int GetBuildingPlacementPosition(GridObjectSO buildingData, Vector3 worldPos, Facing rotation);
}
