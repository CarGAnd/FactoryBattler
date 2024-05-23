using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;
using UnityEngine.Events;

public class PlacementMode : IPlayerMode {

    [HideInInspector] public UnityEvent<Facing> moduleRotated;
    [HideInInspector] public UnityEvent<GridObjectSO> moduleChanged;
    [HideInInspector] public UnityEvent<FactoryGrid> gridSwitched;

    public Facing CurrentFacing { get; private set; }
    public Vector3 CurrentMouseWorldPos { get; private set; }
    
    private FactoryGrid grid;
    private PlacementStateMachine placementStateMachine;
    private IPlacementStrategy placementHandler;
    private IBuilder builder;
    private GridObjectSO currentBuilding;

    public PlacementMode(FactoryGrid grid, IBuilder builder, PlacementStateMachine placementStateMachine){
        this.grid = grid;
        this.builder = builder;
        this.placementStateMachine = placementStateMachine;
        SetModuleRotation(Facing.West);
        placementHandler = new NoPlacement();

        moduleRotated = new UnityEvent<Facing>();
        moduleChanged = new UnityEvent<GridObjectSO>();
        gridSwitched = new UnityEvent<FactoryGrid>();
    }

    public void ChangeGrid(FactoryGrid grid, IBuilder builder) {
        this.grid = grid;
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
            placementStateMachine.GoToSelectionMode();
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
            placementStateMachine.GoToSelectionMode();
            placementHandler = new NoPlacement();
        }
        else {
            placementHandler = newBuilding.GetPlacementHandler(grid, this);
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

    public void TryPlaceModule(GridObjectSO gridObject, Vector3 worldPos, Facing facing) {
        Vector2Int gridPosition = grid.GetPlacementPosition(gridObject.GetLayoutShapeDimensions(facing), worldPos);
        TryPlaceModule(gridObject, gridPosition, facing);
    }
    
    public void TryPlaceModule(GridObjectSO gridObject, Vector2Int gridPos, Facing facing) {
        builder.TryPlaceBuilding(gridObject, gridPos, facing);
    }

    public void EnterMode() {
    }

    public void ExitMode() {
    }
}


