using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public class CellGrid<T> : ISearchable {
        
        private static Vector2Int[] neighborDirections = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        
        public int Columns { get; private set; }
        public int Rows { get; private set; }

        private Cell<T>[,] cells;

        public CellGrid(int numColumns, int numRows) {
            this.Columns = numColumns;
            this.Rows = numRows;
            CreateGrid(numColumns, numRows);
        }

        // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
        public void PlaceObject(T gridObject, Vector2Int originPosition, List<Vector2Int> shapeLayout) {
            if (shapeLayout == null) {
                PlaceObject(gridObject, originPosition);
                return;
            }

            List<Cell<T>> sharedCells = new List<Cell<T>>();

            foreach (Vector2Int deltaCoord in shapeLayout) {
                Vector2Int coord = originPosition + deltaCoord;
                if (CellWithinBounds(coord)) {
                    Cell<T> cell = GetCellAt(coord);
                    sharedCells.Add(cell);
                }
            }

            foreach (Cell<T> c in sharedCells) {
                c.SetOccupyingObject(gridObject, originPosition, sharedCells);
            }
        }

        public void PlaceObject(T gridObject, Vector2Int position) {
            Cell<T> cell = GetCellAt(position);
            cell.SetOccupyingObject(gridObject);
        }

        public void RemoveObject(Vector2Int cellPosition) {
            Cell<T> firstCell = GetCellAt(cellPosition);
            List<Cell<T>> sharedCells = firstCell.GetSharedCells();

            if (sharedCells == null) {
                firstCell.RemoveOccupyingObject();
            }
            else {
                foreach (Cell<T> c in sharedCells) {
                    c.RemoveOccupyingObject();
                }
            }
        }

        public void MoveObject(Vector2Int from, Vector2Int to) {
            T gridObject = GetObjectAt(from);
            Cell<T> fromCell = GetCellAt(from);
            List<Cell<T>> sharedCells = fromCell.GetSharedCells();
            if (sharedCells == null) {
                sharedCells = new List<Cell<T>>() { fromCell };
            }
            Vector2Int objectOrigin = fromCell.GetOccupyingObjectOrigin();
            List<Vector2Int> objectLayout = new List<Vector2Int>();
            foreach (Cell<T> c in sharedCells) {
                Vector2Int originDelta = c.GetCellCoordinates() - objectOrigin;
                objectLayout.Add(originDelta);
            }
            RemoveObject(from);
            PlaceObject(gridObject, to, objectLayout);
        }

        public T GetObjectAt(Vector2Int cellPosition) {
            if (CellWithinBounds(cellPosition)) {
                return GetCellAt(cellPosition).GetOccupyingObject();
            }
            else {
                return default(T);
            }
        }

        public V GetObjectAsType<V>(Vector2Int cellPosition) {
            T obj = GetObjectAt(cellPosition);
            if (obj is V v) {
                return v;
            }
            else {
                return default(V);
            }
        }

        public List<T> GetAllPlacedObjects() {
            HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();
            List<T> placedObjects = new List<T>();
            for (int x = 0; x < Columns; x++) {
                for (int y = 0; y < Rows; y++) {
                    Vector2Int coord = new Vector2Int(x, y);
                    if (PositionIsOccupied(coord) && !visitedPositions.Contains(coord)) {
                        T obj = GetObjectAt(coord);
                        placedObjects.Add(obj);
                        List<Vector2Int> sharedPositions = GetSharedPositions(coord);
                        foreach (Vector2Int position in sharedPositions) {
                            visitedPositions.Add(position);
                        }
                    }
                }
            }
            return placedObjects;
        }

        public List<Vector2Int> GetSharedPositions(Vector2Int cellPosition) {
            List<Vector2Int> sharedPositions = new List<Vector2Int>();
            List<Cell<T>> sharedCells = GetCellAt(cellPosition).GetSharedCells();
            if (sharedCells == null) {
                return sharedPositions;
            }
            foreach (Cell<T> c in sharedCells) {
                sharedPositions.Add(c.GetCellCoordinates());
            }
            return sharedPositions;
        }

        public Vector2Int GetObjectOrigin(Vector2Int cellPosition) {
            return GetCellAt(cellPosition).GetOccupyingObjectOrigin();
        }

        public bool PositionIsOccupied(Vector2Int cellPosition) {
            if (CellWithinBounds(cellPosition)) {
                return GetCellAt(cellPosition).IsOccupied();
            }
            else {
                return false;
            }
        }

        public bool AllPositionsAreFree(List<Vector2Int> positions) {
            foreach (Vector2Int position in positions) {
                if (PositionIsOccupied(position)) {
                    return false;
                }
            }
            return true;
        }

        public bool CellWithinBounds(Vector2Int cellCoord) {
            return cellCoord.y >= 0 && cellCoord.y < Rows && cellCoord.x >= 0 && cellCoord.x < Columns;
        }

        public List<Vector2Int> GetNeighbors(Vector2Int cellPosition) {
            List<Vector2Int> neighbors = new List<Vector2Int>(neighborDirections.Length);
            foreach (Vector2Int direction in neighborDirections) {
                Vector2Int neighborValue = cellPosition + direction;
                if (CellWithinBounds(neighborValue)) {
                    neighbors.Add(neighborValue);
                }
            }
            return neighbors;
        }

        public Path FindPath(Vector2Int startPosition, Vector2Int endPosition) {
            List<Vector2Int> positions = null;

            if (PositionIsOccupied(endPosition)) {
                return new Path(positions);
            }

            positions = GridBFS.FindPath(this, startPosition, (Vector2Int coord) => coord == endPosition, (Vector2Int coord) => CellWithinBounds(coord) && !PositionIsOccupied(coord));
            return new Path(positions);
        }

        public Vector2Int FindClosestUnoccupiedCell(Vector2Int cellPosition) {
            List<Vector2Int> path = GridBFS.FindPath(this, cellPosition, (Vector2Int coord) => !PositionIsOccupied(coord), (Vector2Int coord) => CellWithinBounds(coord));
            return path[path.Count - 1];
        }

        private Cell<T> GetCellAt(Vector2Int coord) {
            return cells[coord.y, coord.x];
        }

        private void CreateGrid(int columns, int rows) {
            cells = new Cell<T>[rows, columns];
            for (int y = 0; y < rows; y++) {
                for (int x = 0; x < columns; x++) {
                    cells[y, x] = new Cell<T>(y, x);
                }
            }
        }
    }
}
