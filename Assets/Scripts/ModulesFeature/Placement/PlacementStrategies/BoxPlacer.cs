using System.Collections.Generic;
using UnityEngine;

public class BoxPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;
    private Vector2Int startDragPos;
    private bool isDragging;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public BoxPlacer(GridObjectSO gridObject, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = gridObject;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        if (isDragging) {
            return GetPositionsInBox(startDragPos, grid.GetCellCoords(mousePosOnGrid), grid);
        }
        else {
            return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
        }
    }

    public void UpdateInput(MouseInput mouseInput) {
        Vector3 mousePosOnGrid = mouseInput.GetMousePosOnGrid(grid);
        if (mouseInput.PlaceModuleStarted()) {
            isDragging = true;
            startDragPos = grid.GetCellCoords(mousePosOnGrid);
        }    
        if(mouseInput.PlaceModuleEnded() && isDragging) {
            isDragging = false;
            Vector2Int endDragPos = grid.GetCellCoords(mousePosOnGrid);
            List<Vector2Int> draggedSubgrid = GetPositionsInBox(startDragPos, endDragPos, grid);
            foreach(Vector2Int pos in draggedSubgrid) {
                placementMode.TryPlaceModule(currentModule, pos, placementMode.CurrentFacing);
            }
        }
    }

    private List<Vector2Int> GetPositionsInBox(Vector2Int startPos, Vector2Int endPos, FactoryGrid grid) {
        Vector2Int lowerLeft = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
        Vector2Int upperRight = new Vector2Int(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));
        Vector2Int boxDimensions = upperRight - lowerLeft + Vector2Int.one;
        return grid.GetPositionsInSubgrid(lowerLeft, boxDimensions);
    }
}
