using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionModeVisual : MonoBehaviour
{
    [SerializeField] private SelectionMode selectionMode;
    [SerializeField] private PlayerModeManager playerModeManager;
    [SerializeField] private GameObject indicatorPrefab;

    private FactoryGrid grid;
    private GameObject indicatorInstance;

    private void Awake() {
        grid = playerModeManager.Grid;
        CreateIndicatorObject();
    }

    private void OnEnable() {
        selectionMode.enterSelectionMode.AddListener(OnEnterSelectionMode);
        selectionMode.exitSelectionMode.AddListener(OnExitSelectionMode);
    }

    private void OnDisable() {
        selectionMode.enterSelectionMode.RemoveListener(OnEnterSelectionMode);
        selectionMode.exitSelectionMode.RemoveListener(OnExitSelectionMode);
    }

    private void OnEnterSelectionMode() {
        indicatorInstance.SetActive(true);
    }

    private void OnExitSelectionMode() {
        indicatorInstance.SetActive(false);
    }

    private void Update() {
        UpdateIndicator();
    }

    private void UpdateIndicator() {
        Vector2Int buildPosition = grid.GetCellCoords(selectionMode.LastMouseGridPosition);
        indicatorInstance.transform.position = grid.GetCellCenter(buildPosition);
    }

    private void CreateIndicatorObject() {
        indicatorInstance = Instantiate(indicatorPrefab);
        indicatorInstance.transform.localScale = Vector3.one * grid.CellSize;
        Quaternion oldRot = indicatorInstance.transform.rotation;
        indicatorInstance.transform.rotation = grid.Rotation * oldRot;
        indicatorInstance.transform.parent = transform;
        indicatorInstance.SetActive(false);
    }

}
