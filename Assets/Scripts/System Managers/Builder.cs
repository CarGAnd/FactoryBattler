using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;
    
    public IGridObject TryPlaceBuilding(GridObjectSO building, Vector2Int position, Facing rotation) {
        IGridObject newBuilding = null;
        
        if(modulePlacer.CanPlaceModule(building, position, rotation)) {
            newBuilding = modulePlacer.PlaceModule(building, position, rotation);
            ConnectBuilding(newBuilding);
        }
        return newBuilding;
    }

    public Vector2Int GetModulePlacementPosition(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        return modulePlacer.GetModulePlacementPosition(moduleData, mouseHitPosition, facing);    
    }

    private void ConnectBuilding(IGridObject building) {
        if(building is IAssemblyLineUser) {
            ((IAssemblyLineUser)building).ConnectToAssemblyLine(assemblyLineSystem);
        }
    }
}
