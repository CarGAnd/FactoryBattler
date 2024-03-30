using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridPrePlacer : OdinEditorWindow {

    [MenuItem("Grid/Level Editor")]
    private static void OpenWindow() {
        GetWindow<GridPrePlacer>().Show();
    }

    [HorizontalGroup("row1")]
    public FactoryGrid snappingGrid;
    public GridInitializer gridInitializer;
    public GridObjectSO objectToCreate;

    [Button("Find in scene")]
    private void FindGridInScene() {
        snappingGrid = FindObjectOfType<FactoryGrid>();
        gridInitializer = FindObjectOfType<GridInitializer>();
    }

    [Button("Create new Item")]
    private void PlaceSelectedItemsOnGrid() {
        CreateObject(objectToCreate);    
    }

    [Button("Fix Broken References")]
    private void FixBrokenReferences() {
        PrePlacedObjectData[] prePlacedObjects = FindObjectsByType<PrePlacedObjectData>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
        foreach(PrePlacedObjectData pp in prePlacedObjects) {
            gridInitializer.AddNewObject(pp);
        }
    }

    private void CreateObject(GridObjectSO objectToCreate) {
        Ray screenCenterRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 1.0f));
        Vector3 worldPosition = snappingGrid.RaycastGridPlane(screenCenterRay);
        Facing startingFacing = Facing.West;
        Vector2Int layoutDimensions = objectToCreate.GetLayoutShapeDimensions(startingFacing);
        Vector2Int gridPosition = snappingGrid.GetSubgridOriginCoord(worldPosition, layoutDimensions);
        worldPosition = snappingGrid.GetSubgridCenter(gridPosition, layoutDimensions);
        GameObject previewObject = (GameObject) PrefabUtility.InstantiatePrefab(objectToCreate.PreviewPrefab);
        previewObject.transform.SetPositionAndRotation(worldPosition, snappingGrid.Rotation * startingFacing.GetRotationFromFacing());
        PrePlacedObjectData objectData = previewObject.AddComponent<PrePlacedObjectData>();
        objectData.facing = startingFacing;
        objectData.objectDefinition = objectToCreate;
        objectData.grid = snappingGrid;
        objectData.gridPosition = gridPosition;
        gridInitializer.AddNewObject(objectData);
        Selection.activeObject = previewObject;
    }
}
