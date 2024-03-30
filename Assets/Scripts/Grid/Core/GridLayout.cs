using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public class GridLayout : MonoBehaviour {
        [field: SerializeField] public Vector3 Origin { get; private set; }
        [field: SerializeField] public Quaternion Rotation { get; private set; }
        [field: SerializeField] public Vector2 CellSize { get; private set; }

        public Vector3 RotationPivot { get { return Vector3.zero; } }
        private IGridType layout = new SquareGridLayout();

        // Cell via world position
        public Vector2Int WorldToGrid(Vector3 worldPosition) {
            Vector3 normalizedPosition = RemoveScaleRotationOffset(worldPosition);
            //The grid now has origin at 0,0 with a rotation of 0 and a cellsize of 1x1
            Vector2Int cellCoordinates = layout.WorldToGrid(normalizedPosition);
            return cellCoordinates;
        }

        public Vector3 GetCellCenter(Vector2Int cellCoord) {
            //Calculate the position in a grid with no rotation, the origin at (0,0), and a cellsize of 1x1
            Vector3 normalizedPosition = layout.GetCellCenter(cellCoord);
            //Apply scale, rotation and offset to the position
            Vector3 worldPosition = ApplyScaleRotationOffset(normalizedPosition);
            return worldPosition;
        }

        public Vector3 SnapToCellCenter(Vector3 worldPosition) {
            Vector2Int cellCoords = WorldToGrid(worldPosition);
            return GetCellCenter(cellCoords);
        }

        // World position via cell
        public Vector3 GridToWorld(Vector2Int gridPosition) {
            //Calculate the position in a grid with no rotation, the origin at (0,0), and a cellsize of 1x1
            Vector3 normalizedPosition = layout.GridToWorld(gridPosition);
            //Apply scale, rotation and offset to the position
            Vector3 worldPosition = ApplyScaleRotationOffset(normalizedPosition);
            return worldPosition;
        }

        private Vector3 ApplyScaleRotationOffset(Vector3 normalizedPosition) {
            //Scale the grid to have the correct cellsize
            Vector3 scaledWorldPosition = new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y);
            //Move the grid so that the rotation pivot is at 0,0
            Vector3 pivotRelative = scaledWorldPosition - RotationPivot;
            //Rotate the grid
            Vector3 rotatedGrid = Rotation * pivotRelative;
            //Move the grid to the correct offset
            Vector3 offsetGrid = rotatedGrid + Origin + RotationPivot;
            return offsetGrid;
        }

        private Vector3 RemoveScaleRotationOffset(Vector3 worldPosition) {
            //Move grid so rotation Pivot is at 0,0
            Vector3 pivotRelative = worldPosition - RotationPivot - Origin;
            //Rotate the grid around 0,0 with the inverse of the grids rotation, giving the resulting grid a rotation of 0
            Vector3 unrotatedGrid = Quaternion.Inverse(Rotation) * pivotRelative;
            //Move the grid to have the origin at 0,0
            Vector3 offsetGrid = unrotatedGrid + RotationPivot;
            //Scale the grid to have a cellsize of 1x1
            Vector3 normalizedPosition = Vector3.Scale(offsetGrid, new Vector3(1f / CellSize.x, 1, 1f / CellSize.y));
            return normalizedPosition;
        }

        public Vector3 SnapToCell(Vector3 worldPosition) {
            Vector2Int cellCoord = WorldToGrid(worldPosition);
            return GridToWorld(cellCoord);
        }

        public Vector3 GetSubgridCenter(Vector2Int bottomLeft, Vector2Int dimensions) {
            Vector3 bottomLeftPos = layout.GridToWorld(bottomLeft);
            Vector3 normalizedCenter = bottomLeftPos + new Vector3(dimensions.x, 0, dimensions.y) / 2f;
            Vector3 worldPos = ApplyScaleRotationOffset(normalizedCenter);
            return worldPos;
        }

        public Vector2Int GetSubgridBottomLeft(Vector3 subgridCenter, Vector2Int subgridDimensions) {
            Vector3 normalizedPosition = RemoveScaleRotationOffset(subgridCenter);
            Vector3 offset = new Vector3(1 / 2f * (subgridDimensions.x - 1), 0, 1 / 2f * (subgridDimensions.y - 1));
            Vector3 offsetHitPos = normalizedPosition - offset;
            return layout.WorldToGrid(offsetHitPos);
        }

        public List<Vector2Int> GetPositionsInSubgrid(Vector2Int bottomLeft, Vector2Int subgridDimensions) {
            List<Vector2Int> positions = new List<Vector2Int>();
            for (int x = 0; x < subgridDimensions.x; x++) {
                for (int y = 0; y < subgridDimensions.y; y++) {
                    positions.Add(new Vector2Int(x, y) + bottomLeft);
                }
            }
            return positions;
        }

        public Vector3 RaycastGridPlane(Ray ray) {
            Plane plane = new Plane(Rotation * Vector3.up, Origin);
            plane.Raycast(ray, out float distance);
            Vector3 worldPosition = ray.GetPoint(distance);
            return worldPosition;
        }

        public void SetOrigin(Vector3 newOrigin) {
            Origin = newOrigin;
        }

        public void SetRotation(Quaternion newRotation) {
            Rotation = newRotation;
        }

        public void SetCellSize(float length, float width) {
            CellSize = new Vector2(width, length);
        }
    }
}
