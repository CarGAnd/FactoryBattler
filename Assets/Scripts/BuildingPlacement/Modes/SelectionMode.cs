using UnityEngine;
using UnityEngine.Events;

public class SelectionMode : MonoBehaviour, IMouseMode {

    [HideInInspector] public UnityEvent enterSelectionMode;
    [HideInInspector] public UnityEvent exitSelectionMode;

    public Vector3 LastMouseGridPosition { get; private set; }
    private FactoryGrid grid;

    public void Initialize(FactoryGrid grid) {
        this.grid = grid;
    }

    public void EnterMode() {
        enterSelectionMode.Invoke();
    }

    public void ExitMode() {
        exitSelectionMode.Invoke();
    }

    public void UpdateInput(MouseInput mouseInput, Vector3 mousePosOnGrid) {
        LastMouseGridPosition = mousePosOnGrid;

        if (mouseInput.DeleteModule()) {
            RemoveModule(grid.GetCellCoords(LastMouseGridPosition));
        }
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }   
}


