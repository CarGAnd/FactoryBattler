using PlayerSystem;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

    private ModulePlacer modulePlacer;

    private void Awake() {
        this.modulePlacer = new ModulePlacer(grid);
    }

    public IGridObject TryPlaceBuilding(GridObjectSO building, Vector2Int position, Facing rotation, IPlayer owner) {
        IGridObject newBuilding = null;
        
        if(modulePlacer.CanPlaceModule(building, position, rotation)) {
            newBuilding = modulePlacer.PlaceModule(building, position, rotation);
            SetupBuilding(newBuilding, position, owner);
        }
        return newBuilding;
    }

    public Vector2Int GetModulePlacementPosition(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        return modulePlacer.GetModulePlacementPosition(moduleData, mouseHitPosition, facing);    
    }

    private void SetupBuilding(IGridObject building, Vector2Int gridPosition, IPlayer owner) {
        if(building is IAssemblyLineUser assemblyLineUser) {
            assemblyLineUser.ConnectToAssemblyLine(assemblyLineSystem);
        }
        if (building is IPlayerOwned playerOwned) {
            playerOwned.SetOwner(owner);
        }
        // if (building is IPlayerOwned)
        building.OnPlacedOnGrid(gridPosition, grid);
    }
}
