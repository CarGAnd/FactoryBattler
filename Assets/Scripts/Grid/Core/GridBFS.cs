using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace GridSystem {
    public class GridBFS {
        public static List<Vector2Int> FindPath(ISearchable grid, Vector2Int startPosition, Func<Vector2Int, bool> goalCondition, Func<Vector2Int, bool> isWalkableCell) {
            if (goalCondition(startPosition)) {
                //if the condition is satisfied in the starting cell then the path is just the starting cell
                return new List<Vector2Int>() { startPosition };
            }

            Queue<PathCell> frontier = new Queue<PathCell>();
            frontier.Enqueue(new PathCell(null, startPosition));
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            while (frontier.Count > 0) {
                PathCell currentCell = frontier.Dequeue();
                Vector2Int currentCoord = currentCell.coord;
                List<Vector2Int> neighbors = grid.GetNeighbors(currentCoord);
                foreach (Vector2Int coord in neighbors) {
                    if (!isWalkableCell(coord)) {
                        continue;
                    }
                    PathCell cell = new PathCell(currentCell, coord);
                    if (goalCondition(coord)) {
                        return GetPath(cell);
                    }
                    else if (!visited.Contains(coord)) {
                        frontier.Enqueue(cell);
                        visited.Add(coord);
                    }
                }
            }
            //no path possible
            return null;
        }

        private static List<Vector2Int> GetPath(PathCell endCell) {
            List<Vector2Int> pathList = new List<Vector2Int>();
            while (endCell != null) {
                pathList.Add(endCell.coord);
                endCell = endCell.prevCell;
            }
            pathList.Reverse();
            return pathList;
        }

        private class PathCell {
            public PathCell prevCell;
            public Vector2Int coord;

            public PathCell(PathCell prevCell, Vector2Int coord) {
                this.prevCell = prevCell;
                this.coord = coord;
            }
        }
    }

    public class Path {

        public static Path CombinePaths(List<Path> paths) {
            List<Vector2Int> allPathPositions = new List<Vector2Int>();
            foreach(Path p in paths) {
                if (!p.isValid()) {
                    continue;
                }
                foreach(Vector2Int position in p.GetPositions()) {
                    allPathPositions.Add(position);
                }
            }
            return new Path(allPathPositions);
        }

        private List<Vector2Int> pathPositions;

        public Path(List<Vector2Int> pathCoords) {
            this.pathPositions = pathCoords;    
        }

        public List<Vector2Int> GetPositions() {
            return pathPositions;
        }

        public void RemoveFirstPosition() {
            pathPositions.RemoveAt(0);
        }

        public void RemoveLastPosition() {
            pathPositions.RemoveAt(pathPositions.Count - 1);
        }

        public int Length() {
            return pathPositions.Count;
        }

        public List<Vector2Int> GetDirections() {
            if (!isValid()) {
                return null;
            }

            if (pathPositions.Count == 1) {
                return new List<Vector2Int>() { Vector2Int.right };
            }

            List<Vector2Int> pathDirections = new List<Vector2Int>();
            for (int i = 0; i < pathPositions.Count - 1; i++) {
                pathDirections.Add(pathPositions[i + 1] - pathPositions[i]);
            }
            //We assume that the last object in the list has the same direction as the second last object, as we don't have a "next" object to compare to
            pathDirections.Add(pathDirections[pathPositions.Count - 2]);

            return pathDirections;
        }

        public bool isValid() {
            return pathPositions != null && pathPositions.Count > 0;
        }
    }
}
