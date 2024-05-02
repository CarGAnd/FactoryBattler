using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Builder : NetworkBehaviour
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;
    [SerializeField] private BuildingDatabase buildingDatabase;

    private ModulePlacer modulePlacer;

    private void Awake() {
        this.modulePlacer = new ModulePlacer(grid);
    }

    [Rpc(SendTo.Server)]
    public void TryPlaceBuildingServerRpc(FixedString128Bytes buildingId, Vector2Int pos, Facing rotation) {
        GridObjectSO building = buildingDatabase.GetBuildingByID(buildingId.ToString());
        IGridObject placedBuilding = TryPlaceBuilding(building, pos, rotation);
        if(placedBuilding != null) {
            TryPlaceBuildingClientRpc(buildingId, pos, rotation);
        }
    }

    [Rpc(SendTo.NotServer)]
    public void TryPlaceBuildingClientRpc(FixedString128Bytes buildingId, Vector2Int pos, Facing rotation) {
        GridObjectSO building = buildingDatabase.GetBuildingByID(buildingId.ToString());
        TryPlaceBuilding(building, pos, rotation);
    }

    public IGridObject TryPlaceBuilding(GridObjectSO building, Vector2Int position, Facing rotation) {
        IGridObject newBuilding = null;
        
        if(modulePlacer.CanPlaceModule(building, position, rotation)) {
            newBuilding = modulePlacer.PlaceModule(building, position, rotation);
            SetupBuilding(newBuilding, position);
        }
        return newBuilding;
    }

    public Vector2Int GetModulePlacementPosition(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        return modulePlacer.GetModulePlacementPosition(moduleData, mouseHitPosition, facing);    
    }

    private void SetupBuilding(IGridObject building, Vector2Int gridPosition) {
        if(building is IAssemblyLineUser) {
            ((IAssemblyLineUser)building).ConnectToAssemblyLine(assemblyLineSystem);
        }
        building.OnPlacedOnGrid(gridPosition, grid);
    }
}
