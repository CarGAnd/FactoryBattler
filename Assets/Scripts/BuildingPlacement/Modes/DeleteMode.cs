using UnityEngine;
using UnityEngine.Events;

public class DeleteMode : IPlayerMode {

    public Vector3 LastMouseGridPosition { get; private set; }

    private FactoryGrid grid;
    private PlayerModeManager playerModeManager;
    private IBuilder builder;

    public DeleteMode(FactoryGrid grid, IBuilder builder, PlayerModeManager playerModeManager) {
        this.grid = grid;
        this.builder = builder;
        this.playerModeManager = playerModeManager;
    }

    public void EnterMode() {
    }

    public void ExitMode() {
    }

    public void UpdateInput(MouseInput mouseInput, Vector3 mousePositionOnGrid) {
        LastMouseGridPosition = mousePositionOnGrid;
        if (mouseInput.CancelModulePlacement()) {
            playerModeManager.GoToSelectionMode();
        }
        if (mouseInput.DeleteModule()) {
            RemoveModule(grid.GetCellCoords(mousePositionOnGrid));
        }
    }

    public void RemoveModule(Vector2Int gridPosition) {
        builder.RemoveBuilding(gridPosition);
    }   
}


