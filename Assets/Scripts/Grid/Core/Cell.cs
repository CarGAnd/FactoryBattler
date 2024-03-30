using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {

    internal class Cell<T> {
        private int row;
        private int column;
        private CellState state;
        private T occupyingObject;
        //The list of cells that has the same occupying object
        private List<Cell<T>> sharedCells;
        private Vector2Int occupyingObjectOrigin;

        public Cell(int row, int column, CellState state = CellState.EMPTY) {
            this.row = row;
            this.column = column;
            this.state = state;
        }

        public CellState GetState() {
            return state;
        }

        public T GetOccupyingObject() {
            return occupyingObject;
        }

        public void SetOccupyingObject(T occupyingObject, Vector2Int objectOriginCell, List<Cell<T>> sharedCells = null) {
            if (occupyingObject != null) {
                this.occupyingObject = occupyingObject;
                this.occupyingObjectOrigin = objectOriginCell;
                this.sharedCells = sharedCells;
                state = CellState.OCCUPIED;
            }
            else {
                RemoveOccupyingObject();
            }
        }

        public void SetOccupyingObject(T occupyingObject) {
            SetOccupyingObject(occupyingObject, new Vector2Int(column, row), null);
        }

        public List<Cell<T>> GetSharedCells() {
            return sharedCells;
        }

        public Vector2Int GetOccupyingObjectOrigin() {
            return occupyingObjectOrigin;
        }

        public void RemoveOccupyingObject() {
            occupyingObject = default(T);
            sharedCells = null;
            state = CellState.EMPTY;
        }

        //This refers to the coordinates within the grid, More or less just the row and column.
        public Vector2Int GetCellCoordinates() {
            return new Vector2Int(column, row);
        }

        public bool IsOccupied() {
            return state == CellState.OCCUPIED;
        }
    }

    public enum CellState {
        EMPTY,
        OCCUPIED
    }
}
