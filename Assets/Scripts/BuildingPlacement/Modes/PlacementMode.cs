using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class PlacementMode : MonoBehaviour, IMouseMode {

    [HideInInspector] public UnityEvent<Facing> moduleRotated;
    [HideInInspector] public UnityEvent<GridObjectSO> moduleChanged;
    [HideInInspector] public UnityEvent enterPlacementMode;
    [HideInInspector] public UnityEvent exitPlacementMode;
    [HideInInspector] public UnityEvent<FactoryGrid> gridSwitched;

    public Facing CurrentFacing { get; private set; }
    public Vector3 CurrentMouseWorldPos { get; private set; }
    public FactoryGrid Grid { get; private set; }

    private PlayerModeManager playerModeManager;
    private IPlacementStrategy placementHandler;
    private Builder builder;
    private GridObjectSO currentBuilding;

    public void Initialize(FactoryGrid grid, Builder builder, PlayerModeManager playerModeManager){
        this.Grid = grid;
        this.builder = builder;
        this.playerModeManager = playerModeManager;
        SetModuleRotation(Facing.West);
        placementHandler = new NoPlacement();
    }

    public void ChangeGrid(FactoryGrid grid, Builder builder) {
        this.Grid = grid;
        this.builder = builder;
        if(currentBuilding == null) {
            placementHandler = new NoPlacement();
        }
        else {
            placementHandler = currentBuilding.GetPlacementHandler(grid, this);
        }
        gridSwitched?.Invoke(grid);
    }

    public void UpdateInput(MouseInput mouseInput, Vector3 mousePosOnGrid) {
        CurrentMouseWorldPos = mousePosOnGrid;
        placementHandler.UpdateInput(mouseInput);
        if (mouseInput.CancelModulePlacement()) {
            playerModeManager.GoToSelectionMode();
        }
    }

    public void RotateModuleClockwise() {
        SetModuleRotation(CurrentFacing.RotatedDirection(1));
    }

    public void RotateModuleCounterClockwise() {
        SetModuleRotation(CurrentFacing.RotatedDirection(-1));
    }

    public List<Vector2Int> GetHoveredPositions() {
        return placementHandler.GetHoveredPositions(CurrentMouseWorldPos);
    }

    public void SetSelectedBuilding(GridObjectSO newBuilding) {
        if(newBuilding == null) {
            playerModeManager.GoToSelectionMode();
            placementHandler = new NoPlacement();
        }
        else {
            placementHandler = newBuilding.GetPlacementHandler(Grid, this);
        }
        currentBuilding = newBuilding;
        moduleChanged?.Invoke(newBuilding);
    }

    public void UpdateRotationInput(MouseInput mouseInput) {
        if (mouseInput.RotateModuleCounterClockwise()) {
            RotateModuleCounterClockwise();
        }
        if (mouseInput.RotateModuleClockwise()) {
            RotateModuleClockwise();
        }
    }

    public void SetModuleRotation(Facing facing) {
        CurrentFacing = facing;
        moduleRotated?.Invoke(facing);
    }

    public IGridObject TryPlaceModule(GridObjectSO gridObject, Vector3 worldPos, Facing facing) {
        Vector2Int gridPosition = builder.GetModulePlacementPosition(gridObject, worldPos, facing);
        return TryPlaceModule(gridObject, gridPosition, facing);
    }

    public IGridObject TryPlaceModule(GridObjectSO gridObject, Vector2Int gridPos, Facing facing) {
        return builder.TryPlaceBuilding(gridObject, gridPos, facing, playerModeManager.Owner);
    }

    public void EnterMode() {
        enterPlacementMode.Invoke();
    }

    public void ExitMode() {
        exitPlacementMode.Invoke();
    }
}


