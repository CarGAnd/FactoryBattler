using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePlacer : MonoBehaviour
{
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

    public Vector2Int GetModulePlacementPosition(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        Vector2Int buildingDimensions = moduleData.GetLayoutShapeDimensions(facing);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        return gridPosition;
    }

    public IGridObject TryPlaceModule(GridObjectSO moduleData, Vector2Int lowerLeftPosition, Facing facing) {
        if (CanPlaceModule(moduleData, lowerLeftPosition, facing)) {
            IGridObject placedObject = PlaceModule(moduleData, lowerLeftPosition, facing);
            return placedObject;
        }
        else {
            return null;
        }
    }

    private bool CanPlaceModule(GridObjectSO gridObject, Vector2Int lowerLeft, Facing facing) {
        List<Vector2Int> buildingPositions = gridObject.GetLayoutShape(facing);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += lowerLeft;
        }
        bool allPositionsAreBuildable = AllPositionsAreBuildable(buildingPositions);
        return allPositionsAreBuildable;
    }

    private bool AllPositionsAreBuildable(List<Vector2Int> positions) {
        foreach(Vector2Int position in positions) {
            if (!PositionsIsBuildable(position)) {
                return false;
            }
        }
        return true;
    }

    private bool PositionsIsBuildable(Vector2Int position) {
        return grid.CellWithinBounds(position) && !grid.PositionIsOccupied(position);
    }

    private IGridObject PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft, Facing facing) {
        Quaternion rotation = grid.Rotation * facing.GetRotationFromFacing();
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(facing));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, rotation, facing, assemblyLineSystem);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(facing));
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
        return gridObject;
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }   

    public void ConnectExistingBuilding(IGridObject gridObject, Vector2Int lowerLeft, List<Vector2Int> layoutShape) {
        grid.PlaceObject(gridObject, lowerLeft, layoutShape);
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
    }
 
}
