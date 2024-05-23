using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteModeVisuals : MonoBehaviour
{
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private PlacementStateMachine placementStateMachine;

    private DeleteMode deleteMode;
    private CellMarker deleteMarker;

    private FactoryGrid grid;
    private bool isActive;
    private Vector3 indicatorOffset = Vector3.up * 0.01f;

    private void Awake() {
        grid = placementStateMachine.Grid;
        deleteMarker = new CellMarker(CreateIndicatorObject);
    }

    private void OnEnable() {
        placementStateMachine.modeChanged.AddListener(OnModeChanged);
    }

    private void OnDisable() {
        placementStateMachine.modeChanged.RemoveListener(OnModeChanged);
    }

    private void OnModeChanged(IPlayerMode newMode) {
        if(newMode is DeleteMode delMode) {
            if(deleteMode != delMode) {
                deleteMode = delMode;
                grid = placementStateMachine.Grid;
            }
            OnEnterDeleteMode();
        }
        else {
            OnExitDeleteMode();
        }
    }

    private void OnEnterDeleteMode() {
        isActive = true;
    }

    private void OnExitDeleteMode() {
        deleteMarker.RemoveAllMarkers();
        isActive = false;
    }

    private void Update() {
        if (!isActive) {
            return;
        }
        Vector2Int gridPos = grid.GetCellCoords(deleteMode.LastMouseGridPosition);
        if (grid.CellWithinBounds(gridPos)) {
            UpdateGroundIndicators(gridPos); 
        }
    }

    private void UpdateGroundIndicators(Vector2Int hoveredPosition) {
        List<Vector2Int> sharedPositions = grid.GetSharedPositions(hoveredPosition);
        if(sharedPositions.Count == 0) {
            sharedPositions.Add(hoveredPosition);
        }
        deleteMarker.MarkPositions(sharedPositions, grid);
        for(int i = 0; i < sharedPositions.Count; i++) {
            Vector2Int buildPosition = sharedPositions[i];
            deleteMarker.GetMarker(i).transform.position = grid.GetCellCenter(buildPosition) + grid.Rotation * indicatorOffset;
        }
    }

    private GameObject CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = new Vector3(grid.CellSize.x, grid.CellSize.y, 1);
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = grid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        newIndicatorObject.GetComponent<Renderer>().material.color = Color.red;
        return newIndicatorObject;
    }
}
