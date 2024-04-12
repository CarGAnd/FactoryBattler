using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
    [SerializeField] private GameObject grid1;
    [SerializeField] private GameObject grid2;
    [SerializeField] private PlayerModeManager playerModeManager;
    [SerializeField] private GridLoader gridLoader;

    private string jsonData;
    private FactoryGrid activeGrid;
    private Builder activeBuilder;

    private void SetActivePlayer(int playerNumber) {
        GameObject activeGridObject = playerNumber == 1 ? grid1 : grid2;
        Builder builder = activeGridObject.transform.GetComponentInChildren<Builder>();
        FactoryGrid grid = activeGridObject.transform.GetComponentInChildren<FactoryGrid>();
        activeBuilder = builder;
        activeGrid = grid;
        playerModeManager.UpdateGridReference(grid, builder);
    }

    private void Start() {
        SetActivePlayer(1);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            SetActivePlayer(1);
        }
        else if (Input.GetKeyDown(KeyCode.F2)) {
            SetActivePlayer(2);
        }
    }

    [Button("Test Save")]
    private void Save() {
        jsonData = gridLoader.SaveGrid(activeGrid);
    }

    [Button("Test Load")]
    private void Load() {
        gridLoader.LoadDataToGrid(activeGrid, activeBuilder, jsonData);
    }

    [Button("Clear Grid")]
    private void ClearGrid() {
        activeGrid.ClearGrid();
    }

}
