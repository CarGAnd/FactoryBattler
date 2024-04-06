using System.Collections.Generic;
using UnityEngine;

public class PlaceAsYouGo : IPlacementStrategy {

    private bool isDragging;
    private Vector2Int lastPlacedPosition;
    private GridObjectSO gridObject;
    private Facing lastFacing;
    private bool lastObjectWasPlaced;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public PlaceAsYouGo(GridObjectSO gridObject, FactoryGrid grid, PlacementMode placementMode) {
        this.gridObject = gridObject;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
    }

    public void UpdateInput(MouseInput mouseInput) {
        Vector3 mousePosOnGrid = mouseInput.GetMousePosOnGrid(grid);
        if (mouseInput.PlaceModuleStarted()) {
            isDragging = true;
            IGridObject placedObject = placementMode.TryPlaceModule(gridObject, mousePosOnGrid, placementMode.CurrentFacing);
            lastObjectWasPlaced = placedObject != null;
            lastFacing = placementMode.CurrentFacing;
            lastPlacedPosition = grid.GetCellCoords(mousePosOnGrid);
        }    
        if(mouseInput.PlaceModuleEnded() && isDragging) {
            isDragging = false;
        }
        if (isDragging) {
            Vector2Int currentMouseGridPos = grid.GetCellCoords(mousePosOnGrid);
            if(currentMouseGridPos != lastPlacedPosition) {
                Vector2Int dir = currentMouseGridPos - lastPlacedPosition;
                Facing facing = FacingFromDirection(dir);
                IGridObject placedObject = placementMode.TryPlaceModule(gridObject, currentMouseGridPos, facing);
                if(facing != lastFacing && lastObjectWasPlaced) {
                    grid.GetObjectAt(lastPlacedPosition).DestroyObject();
                    placementMode.TryPlaceModule(gridObject, lastPlacedPosition, facing);
                }
                lastPlacedPosition = currentMouseGridPos;
                lastFacing = facing;
                lastObjectWasPlaced = placedObject != null;
            }
        }
        placementMode.UpdateRotationInput(mouseInput);
    }

    private Facing FacingFromDirection(Vector2Int direction) {
        if(direction == Vector2Int.left) {
            return Facing.West;
        }
        else if(direction == Vector2Int.up) {
            return Facing.North;
        }
        else if(direction == Vector2Int.right) {
            return Facing.East;
        }
        else {
            return Facing.South;
        }
    }
}
