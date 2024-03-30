using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class DragAndDropEditor
{
    [InitializeOnLoadMethod]
    static void OnLoad() {
        DragAndDrop.AddDropHandler(OnSceneDrop);
    }

    private static DragAndDropVisualMode OnSceneDrop(Object dropUpon, Vector3 worldPosition, Vector2 viewportPosition, Transform parentForDraggedObjects, bool perform) {
        if (perform && DragAndDrop.objectReferences[0] is GridObjectSO) {
            GridObjectSO gridObject = (GridObjectSO)DragAndDrop.objectReferences[0];
            GameObject spawnedObject = (GameObject) PrefabUtility.InstantiatePrefab(gridObject.PreviewPrefab);
            Ray ray = HandleUtility.GUIPointToWorldRay(viewportPosition);
            FactoryGrid grid = GameObject.FindAnyObjectByType<FactoryGrid>();
            Vector3 rayHitPos = grid.RaycastGridPlane(ray);
            Vector2Int cellPos = grid.GetCellCoords(rayHitPos);
            Vector3 cellWorld = grid.GetCellCenter(cellPos);
            spawnedObject.transform.position = cellWorld;
            PrePlacedObjectData objectData = spawnedObject.AddComponent<PrePlacedObjectData>();
            objectData.grid = grid;
            objectData.gridPosition = cellPos;
            objectData.objectDefinition = gridObject;
            objectData.facing = Facing.West;
            Selection.activeObject = spawnedObject;

        }
        return DragAndDropVisualMode.Move;
    }
}
