using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePlacer
{
    private FactoryGrid grid;

    public ModulePlacer(FactoryGrid grid) {
        this.grid = grid;
    }

    public bool CanPlaceModule(GridObjectSO gridObject, Vector2Int lowerLeft, Facing facing) {
        List<Vector2Int> buildingPositions = gridObject.GetLayoutShape(facing);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += lowerLeft;
        }
        bool allPositionsAreBuildable = AllPositionsAreBuildable(buildingPositions);
        return allPositionsAreBuildable;
    }

    private bool AllPositionsAreBuildable(List<Vector2Int> positions) {
        foreach(Vector2Int position in positions) {
            if (!PositionIsBuildable(position)) {
                return false;
            }
        }
        return true;
    }

    private bool PositionIsBuildable(Vector2Int position) {
        return grid.CellWithinBounds(position) && !grid.PositionIsOccupied(position);
    }

    public IGridObject PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft, Facing facing) {
        Quaternion rotation = grid.Rotation * facing.GetRotationFromFacing();
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(facing));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, rotation, facing);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(facing));
        return gridObject;
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }   

    public void PlaceExistingBuilding(IGridObject gridObject, Vector2Int lowerLeft, List<Vector2Int> layoutShape) {
        grid.PlaceObject(gridObject, lowerLeft, layoutShape);
    }
}
