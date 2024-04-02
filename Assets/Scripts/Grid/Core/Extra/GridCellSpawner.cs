using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class GridCellSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GridSystem.GridLayout gridLayout;
    [SerializeField] private GridSize gridSize;
    
    [Header("Settings")]
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] private Transform cellParent;
    [SerializeField] private SpawnType spawnType;

    [SerializeField, ShowIf("@spawnType == SpawnType.Custom")] 
    private CustomGridMap customMap;

    private GameObject[,] cells;

    private void Awake() {
        CreateCells();
    }

    public void DestroyObjectAt(Vector2Int coord) {
        Destroy(cells[coord.y, coord.x]);
        cells[coord.y, coord.x] = null;
    }

    public void CreateObjectAt(Vector2Int coord) {
        Vector3 spawnPos = gridLayout.GetCellCenter(coord) + gridLayout.Rotation * spawnOffset;
        Quaternion rotation = gridLayout.Rotation;
        cells[coord.y, coord.x] = Instantiate(cellPrefab, spawnPos, rotation, cellParent);
        cells[coord.y, coord.x].transform.localScale = new Vector3(gridLayout.CellSize.x, 1, gridLayout.CellSize.y);
    }

    public GameObject GetGameObjectAt(Vector2Int coord) {
        return cells[coord.y, coord.x];
    }

    public GameObject GetGameObjectAt(Vector3 worldPos) {
        Vector2Int coord = gridLayout.WorldToGrid(worldPos);
        return GetGameObjectAt(coord);
    }

    public bool CellHasSpawnedPrefab(Vector2Int coord) {
        if (!IsWithingBounds(coord.x, coord.y)) {
            return false;
        }
        return cells[coord.y, coord.x] != null;
    }

    private bool IsWithingBounds(int x, int y) {
        return x >= 0 && x < gridSize.Columns && y >= 0 && y < gridSize.Rows; 
    }

    [Button("Destroy Prefabs", ButtonSizes.Medium)]
    private void DestroyCells() {
        for (int i = cellParent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(cellParent.GetChild(i).gameObject);
        }
        cells = null;
    }

    [Button("Create Prefabs", ButtonSizes.Medium)]
    private void CreateCells() {
        DestroyCells();
        cells = new GameObject[gridSize.Rows, gridSize.Columns];

        if(spawnType == SpawnType.FillGrid) {
            FillGridWithPrefabs();
        }
        else if(spawnType == SpawnType.Custom) {
            FillMaskWithPrefabs();
        }
    }

    private void FillGridWithPrefabs() {
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                CreateObjectAt(new Vector2Int(x, y));
            }
        }
    }

    private void FillMaskWithPrefabs() {
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                if (customMap.GetMaskValue(x, y)) {
                    CreateObjectAt(new Vector2Int(x,y));
                }
            }
        }
    }

    private enum SpawnType {
        FillGrid,
        Custom
    }
}


