using UnityEngine;
using UnityEngine.Events;

public class DeleteMode : IMouseMode {

    [HideInInspector] public UnityEvent enterDeleteMode;
    [HideInInspector] public UnityEvent exitDeleteMode;

    public Vector3 LastMouseGridPosition { get; private set; }

    private FactoryGrid grid;
    private PlayerModeManager playerModeManager;

    public DeleteMode(FactoryGrid grid, PlayerModeManager playerModeManager) {
        this.grid = grid;
        this.playerModeManager = playerModeManager;
    }

    public void EnterMode() {
        enterDeleteMode.Invoke();
    }

    public void ExitMode() {
        exitDeleteMode.Invoke();
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
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }   
}


