using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public interface IGridType {

        Vector2Int WorldToGrid(Vector3 worldPosition);

        // Calculates the world position of a cell based on its row and column.
        Vector3 GridToWorld(Vector2Int cellCoord);

        Vector3 GetCellCenter(Vector2Int cellCoord);
    }

    public class SquareGridLayout : IGridType {

        private static Vector2Int[] fourWayNeighbors = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1)
        };

        private static Vector2Int[] eightWayNeighbors = new Vector2Int[]
        {
            new Vector2Int(1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(-1, 0),
            new Vector2Int(0, -1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1)
        };

        //Return the worldPosition of the given row and column in a grid with 1x1 cells, no offset, and no rotation
        public Vector3 GridToWorld(Vector2Int cellCoord) {
            return new Vector3(cellCoord.x, 0, cellCoord.y);
        }

        public Vector3 GetCellCenter(Vector2Int cellCoord) {
            return GridToWorld(cellCoord) + new Vector3(0.5f, 0, 0.5f);
        }

        //Return the cell coordinates of the given position in a grid with 1x1 cells, no offset, and no rotation
        public Vector2Int WorldToGrid(Vector3 normalizedPosition) {
            int gridX = (int)normalizedPosition.x;
            int gridY = (int)normalizedPosition.z;

            return new Vector2Int(gridX, gridY);
        }
    }

    public class HexGridLayout : IGridType {

        public Vector3 GridToWorld(Vector2Int cellCoord) {
            throw new System.NotImplementedException();
        }

        public Vector3 GetCellCenter(Vector2Int cellCoord) {
            throw new System.NotImplementedException();
        }

        public Vector2Int WorldToGrid(Vector3 worldPosition) {
            throw new System.NotImplementedException();
        }
    }
}
