using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;
using UnityEngine.Events;

//This is mainly just a wrapper class for GridOfObjects + GridLayout, as they are usually used together
public class FactoryGrid : MonoBehaviour, ISearchable
{
    [HideInInspector] public UnityEvent<IGridObject, List<Vector2Int>> objectPlaced;

    public int Rows { get { return gridSize.Rows; } }
    public int Columns { get { return gridSize.Columns; } }

    public Quaternion Rotation { get { return gridLayout.Rotation; } }
    public Vector2 CellSize { get { return gridLayout.CellSize; } }
    public Vector3 Origin { get { return gridLayout.Origin; } }

    [SerializeField] private GridSystem.GridLayout gridLayout;
    [SerializeField] private GridSize gridSize;
    [SerializeField] private GridCellSpawner cellSpawner;

    private CellGrid<IGridObject> placementGrid;

    private void Awake() {
        placementGrid = new CellGrid<IGridObject>(Columns, Rows);
    }
 
    public void PlaceObject(IGridObject gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout) {
        placementGrid.PlaceObject(gridObject, startCell, shapeLayout);
        List<Vector2Int> positions = new List<Vector2Int>();
        foreach(Vector2Int delta in shapeLayout) {
            positions.Add(startCell + delta);
        }
        objectPlaced.Invoke(gridObject, positions);
    }

    public void PlaceObject(IGridObject gridObject, Vector2Int coordinate) {
        placementGrid.PlaceObject(gridObject, coordinate);
        objectPlaced.Invoke(gridObject, new List<Vector2Int>() { coordinate });
    }

    public void RemoveObject(Vector2Int coord) {
        placementGrid.RemoveObject(coord);
    }
    
    public void MoveObject(Vector2Int from, Vector2Int to) {
        placementGrid.MoveObject(from, to);
    }

    public IGridObject GetObjectAt(Vector2Int coord) {
        return placementGrid.GetObjectAt(coord);
    }

    public V GetObjectAsType<V>(Vector2Int coord) {
        return placementGrid.GetObjectAsType<V>(coord);    
    }

    public List<Vector2Int> GetSharedPositions(Vector2Int coord) {
        return placementGrid.GetSharedPositions(coord);    
    }

    public Vector2Int GetObjectOriginCoord(Vector2Int coord) {
        return placementGrid.GetObjectOrigin(coord);
    }

    public bool PositionIsOccupied(Vector2Int coord) {
        return placementGrid.PositionIsOccupied(coord);
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = gridLayout.WorldToGrid(worldPos);
        return PositionIsOccupied(cellCoords);
    }

    public bool CellWithinBounds(Vector2Int cellCoord) {
        return placementGrid.CellWithinBounds(cellCoord) && cellSpawner.CellHasSpawnedPrefab(cellCoord);
    }

    public List<Vector2Int> GetNeighbors(Vector2Int coord) {
        return placementGrid.GetNeighbors(coord);
    }

    public Path FindPath(Vector2Int startCoord, Vector2Int endCoord) {
        List<Vector2Int> positions = null;
        
        if(PositionIsOccupied(endCoord)) {
            return new Path(positions);
        }
        
        positions = GridBFS.FindPath(this, startCoord, (Vector2Int coord) => coord == endCoord, (Vector2Int coord) => CellWithinBounds(coord) && !PositionIsOccupied(coord));
        return new Path(positions);
    }

    public Vector2Int FindClosestUnoccupiedCell(Vector2Int startCoord) {
        List<Vector2Int> path = GridBFS.FindPath(this, startCoord, (Vector2Int coord) => !PositionIsOccupied(coord), (Vector2Int coord) => CellWithinBounds(coord));
        return path[path.Count - 1];
    }

    public Vector2Int GetCellCoords(Vector3 worldPosition) {
        return gridLayout.WorldToGrid(worldPosition);
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        return gridLayout.GetCellCenter(cellCoord);    
    }

    public Vector3 GetCellCenter(Vector3 worldPosition) {
        Vector2Int cellCoords = GetCellCoords(worldPosition);
        return GetCellCenter(cellCoords);
    }

    public Vector3 GetCellWorldPosition(Vector2Int cellCoord) {
        return gridLayout.GridToWorld(cellCoord);    
    }

    public Vector3 GetCellWorldPosition(Vector3 worldPosition) {
        return gridLayout.SnapToCell(worldPosition);    
    }

    public Vector3 GetSubgridCenter(Vector2Int bottomLeft, Vector2Int dimensions) {
        return gridLayout.GetSubgridCenter(bottomLeft, dimensions);
    }

    public Vector2Int GetSubgridOriginCoord(Vector3 subgridCenter, Vector2Int subgridDimensions) {
        return gridLayout.GetSubgridBottomLeft(subgridCenter, subgridDimensions);
    }

    public List<Vector2Int> GetPositionsInSubgrid(Vector2Int lowerLeft, Vector2Int subgridDimensions) {
        return gridLayout.GetPositionsInSubgrid(lowerLeft, subgridDimensions);
    }

    public Vector3 RaycastGridPlane(Ray ray) {
        return gridLayout.RaycastGridPlane(ray);
    }

    #region Debugging
    [SerializeField] private bool showOccupiedCells = true;
    [SerializeField] private bool showGridLines = true;

    private void OnDrawGizmos() {
        if(gridLayout == null) {
            return;
        }

        if (showOccupiedCells && placementGrid != null) {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            Matrix4x4 rotMatrix = new Matrix4x4();
            rotMatrix.SetTRS(Origin, Rotation, Vector3.one);
            Gizmos.matrix = rotMatrix;
            for (int y = 0; y < placementGrid.Rows; y++) {
                for (int x = 0; x < placementGrid.Columns; x++) {
                    Vector2Int position = new Vector2Int(x, y);
                    if (PositionIsOccupied(position)) {
                        Vector3 cellCenter = gridLayout.GetCellCenter(position);
                        Vector3 cubePos = Quaternion.Inverse(Rotation) * (cellCenter - Origin);
                        Vector3 cubeSize = new Vector3(1, 0.01f, 1) * Mathf.Min(gridLayout.CellSize.x, gridLayout.CellSize.y);
                        Gizmos.DrawCube(cubePos, cubeSize);
                    }
                }
            }
            Gizmos.matrix = Matrix4x4.identity;
        }


        if (showGridLines) {
            Gizmos.color = Color.green;
            for (int y = 0; y < Rows + 1; y++) {
                Vector3 start = GetCellWorldPosition(new Vector2Int(0, y));
                Vector3 end = GetCellWorldPosition(new Vector2Int(Columns, y));
                Gizmos.DrawLine(start, end);
            }

            for (int x = 0; x < Columns + 1; x++) {
                Vector3 start = GetCellWorldPosition(new Vector2Int(x, 0));
                Vector3 end = GetCellWorldPosition(new Vector2Int(x, Rows));
                Gizmos.DrawLine(start, end);
            }
        }
    }
    #endregion
}
