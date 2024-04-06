using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class PathPlacer : IPlacementStrategy {
    private GridObjectSO currentModule;
    private PlacementMode placementMode;
    private FactoryGrid grid;

    private bool isDragging;
    private Vector2Int startDragPos;

    private Vector2Int subpathStart;
    private List<Path> subPaths;
    private List<Vector2Int> currentTotalPath;
    private Vector3 lastMousePosition;

    public PathPlacer(GridObjectSO obj, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = obj;
        this.grid = grid;
        this.placementMode = placementMode;
        lastMousePosition = placementMode.CurrentMouseWorldPos;
    }

    public void UpdateInput(MouseInput mouseInput) {
        lastMousePosition = mouseInput.GetMousePosOnGrid(grid);
        if (mouseInput.PlaceModuleStarted()) {
            startDragPos = grid.GetCellCoords(lastMousePosition);
            subpathStart = startDragPos;
            isDragging = true;
            subPaths = new List<Path>();
            currentTotalPath = new List<Vector2Int>();
        }
        if (mouseInput.PlaceModuleEnded() && isDragging) {
            isDragging = false;
            Vector2Int endDragPos = grid.GetCellCoords(lastMousePosition);
            AddSubPathTo(endDragPos);
            Path totalPath = Path.CombinePaths(subPaths);
            
            if(!totalPath.isValid()) {
                //If no path is found, we cannot place modules
                return;
            }
            if(totalPath.Length() == 1) {
                //If the path has only one positions, treat it as placing a module normally
                placementMode.TryPlaceModule(currentModule, endDragPos, placementMode.CurrentFacing);
            }
            else {
                PlaceModulesAlongPath(currentModule, totalPath);
            }            
        }
        if (Input.GetKeyDown(KeyCode.C)) {
            Vector2Int mouseGridPos = grid.GetCellCoords(lastMousePosition);
            AddSubPathTo(mouseGridPos);
        }
        if (Input.GetKeyDown(KeyCode.V)) {
            RemoveSubPath();
        }
        placementMode.UpdateRotationInput(mouseInput);
    }

    private void AddSubPathTo(Vector2Int goalPos) {
        Path newSubPath = CreateSubPathTo(goalPos);
        if (!newSubPath.isValid()) {
            return;
        }
            
        if (newSubPath.Length() > 0) {
            subPaths.Add(newSubPath);
            subpathStart = goalPos;
            currentTotalPath = Path.CombinePaths(subPaths).GetPositions();
        }
    }

    private Path CreateSubPathTo(Vector2Int goalPos) {
        Path newPath = grid.FindPath(subpathStart, goalPos);
        if(newPath.isValid()) {
            //The returned path contains both the start and end position
            //for all subpaths after the first, the start position of subpath i is the end position of subpath i-1
            //Therefore we remove the start position of all subpaths after the first one to not have duplicate positions in the path
            if(subPaths.Count > 0 && newPath.Length() > 0) {
                newPath.RemoveFirstPosition();
            }
        }
        return newPath;
    }

    private void RemoveSubPath() {
        if (subPaths.Count > 0) {
            subPaths.RemoveAt(subPaths.Count - 1);
            currentTotalPath = Path.CombinePaths(subPaths).GetPositions();
            if (subPaths.Count > 0) {
                List<Vector2Int> pathPositions = subPaths[subPaths.Count - 1].GetPositions();
                subpathStart = pathPositions[pathPositions.Count - 1];
            }
            else {
                subpathStart = startDragPos;
            }
        }
    }

    private List<Vector2Int> GetCurrentTotalPath() {
        List<Path> paths = new List<Path>();
        paths.Add(new Path(currentTotalPath));
        
        Path lastPath = CreateSubPathTo(grid.GetCellCoords(lastMousePosition));
        paths.Add(lastPath);
       
        Path totalPath = Path.CombinePaths(paths);
        return totalPath.GetPositions();
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        if (!isDragging) {
            return new List<Vector2Int>() { grid.GetCellCoords(lastMousePosition) };
        }
        else {
            return GetCurrentTotalPath();    
        }
    }

    private void PlaceModulesAlongPath(GridObjectSO moduleData, Path path) {
        List<Vector2Int> pathPositions = path.GetPositions();
        List<Vector2Int> pathDirections = path.GetDirections();
        //The first n-1 modules are rotated to match the path
        for(int i = 0; i < pathPositions.Count - 1; i++) {
            placementMode.TryPlaceModule(moduleData, pathPositions[i], FacingFromDirection(pathDirections[i]));
        }
        //The last module is rotated according to the user input
        placementMode.TryPlaceModule(moduleData, pathPositions[pathPositions.Count - 1], placementMode.CurrentFacing);
    }

    private Facing FacingFromDirection(Vector2Int direction) {
        if(direction == Vector2Int.left) {
            return Facing.West;
        }
        else if(direction == Vector2Int.up) {
            return Facing.North;
        }
        else if(direction == Vector2Int.right) {
            return Facing.East;
        }
        else {
            return Facing.South;
        }
    }
}
