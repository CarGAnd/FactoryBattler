using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteModeVisuals : MonoBehaviour
{
    [SerializeField] private DeleteMode deleteMode;
    [SerializeField] private PlayerModeManager playerModeManager;
    [SerializeField] private GameObject indicatorPrefab;

    private CellMarker deleteMarker;

    private FactoryGrid grid;
    private bool isActive;

    private void Awake() {
        grid = playerModeManager.Grid;
        deleteMarker = new CellMarker(CreateIndicatorObject);
    }

    private void OnEnable() {
        deleteMode.enterDeleteMode.AddListener(OnEnterDeleteMode);
        deleteMode.exitDeleteMode.AddListener(OnExitDeleteMove);
    }

    private void OnDisable() {
        deleteMode.enterDeleteMode.RemoveListener(OnEnterDeleteMode);
        deleteMode.exitDeleteMode.RemoveListener(OnExitDeleteMove);
    }

    private void OnEnterDeleteMode() {
        isActive = true;
    }

    private void OnExitDeleteMove() {
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
            deleteMarker.GetMarker(i).transform.position = grid.GetCellCenter(buildPosition);
        }
    }

    private GameObject CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * grid.CellSize;
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = grid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        return newIndicatorObject;
    }
}
