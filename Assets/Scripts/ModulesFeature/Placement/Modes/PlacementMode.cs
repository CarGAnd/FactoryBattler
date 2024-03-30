﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlacementMode : MonoBehaviour, IMouseMode {

    [HideInInspector] public UnityEvent moduleRotated;
    [HideInInspector] public UnityEvent<GridObjectSO> moduleChanged;
    [HideInInspector] public UnityEvent enterPlacementMode;
    [HideInInspector] public UnityEvent exitPlacementMode;

    public Quaternion CurrentPlacementRotation { get; private set; }
    public Facing CurrentFacing { get; private set; }
    public Vector3 CurrentMouseWorldPos { get; private set; }
    
    private FactoryGrid grid;
    private PlayerModeManager playerModeManager;
    private IPlacementStrategy placementHandler;
    private ModulePlacer modulePlacer;
    private GridObjectSO currentBuilding;

    public void Initialize(FactoryGrid grid, ModulePlacer modulePlacer, PlayerModeManager playerModeManager){
        this.grid = grid;
        this.playerModeManager = playerModeManager;
        SetModuleRotation(Facing.West);
        placementHandler = new NoPlacement();
        this.modulePlacer = modulePlacer;
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
        CurrentPlacementRotation = grid.Rotation * CurrentFacing.GetRotationFromFacing();
        moduleRotated?.Invoke();
    }

    public IGridObject TryPlaceModule(GridObjectSO gridObject, Vector3 worldPos, Facing facing) {
        Vector2Int gridPosition = modulePlacer.GetModulePlacementPosition(gridObject, worldPos, facing);
        return TryPlaceModule(gridObject, gridPosition, facing);
    }

    public IGridObject TryPlaceModule(GridObjectSO gridObject, Vector2Int gridPos, Facing facing) {
        //Anything extra like cost could go here
        return modulePlacer.TryPlaceModule(gridObject, gridPos, facing);
    }

    public void EnterMode() {
        enterPlacementMode.Invoke();
    }

    public void ExitMode() {
        exitPlacementMode.Invoke();
    }
}


