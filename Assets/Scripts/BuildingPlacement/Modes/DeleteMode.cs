using UnityEngine;

public class DeleteMode : IPlayerMode {

    public Vector3 LastMouseGridPosition { get; private set; }

    private FactoryGrid grid;
    private PlacementStateMachine placementStateMachine;
    private IPlayerGrid builder;

    public DeleteMode(FactoryGrid grid, IPlayerGrid builder, PlacementStateMachine placementStateMachine) {
        this.grid = grid;
        this.builder = builder;
        this.placementStateMachine = placementStateMachine;
    }

    public void EnterMode() {
    }

    public void ExitMode() {
    }

    public void UpdateInput(MouseInput mouseInput, Vector3 mousePositionOnGrid) {
        LastMouseGridPosition = mousePositionOnGrid;
        if (mouseInput.CancelModulePlacement()) {
            placementStateMachine.GoToSelectionMode();
        }
        if (mouseInput.DeleteModule()) {
            RemoveModule(grid.GetCellCoords(mousePositionOnGrid));
        }
    }

    public void RemoveModule(Vector2Int gridPosition) {
        builder.RemoveBuilding(gridPosition);
    }   
}


