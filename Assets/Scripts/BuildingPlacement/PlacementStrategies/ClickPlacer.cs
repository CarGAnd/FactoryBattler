using System.Collections.Generic;
using UnityEngine;

public class ClickPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public ClickPlacer(GridObjectSO obj, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = obj;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        Vector2Int buildingDimensions = currentModule.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mousePosOnGrid, buildingDimensions);
        List<Vector2Int> buildingPositions = currentModule.GetLayoutShape(placementMode.CurrentFacing);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += gridPosition;
        }
        return buildingPositions;
    }

    public void UpdateInput(MouseInput mouseInput) {
        if (mouseInput.PlaceModule()) {
            placementMode.TryPlaceModule(currentModule, mouseInput.GetMousePosOnGrid(grid), placementMode.CurrentFacing);
        }
        placementMode.UpdateRotationInput(mouseInput);    
    }
}
