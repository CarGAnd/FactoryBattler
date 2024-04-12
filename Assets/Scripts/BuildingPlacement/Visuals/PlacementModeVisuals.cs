using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementModeVisuals : MonoBehaviour
{
    [SerializeField] private PlacementMode placementMode;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private GameObject gridPlanePrefab;

    private CellMarker hoveredCellsMarker;

    private FactoryGrid buildGrid;
    private GameObject placementPreview;
    private GameObject arrowObject;
    private GameObject gridPlaneObject;
    private Vector2Int lastOriginCoord;

    private GridObjectSO selectedObjectData;
    private Vector2Int buildingDimensions;

    private Quaternion currentRotation;

    private Vector3 arrowObjectRotationOffset = new Vector3(90, 90, 0);
    private float groundIndicatorAlpha = 0.6f;
    private Vector3 indicatorOffset = Vector3.up * 0.01f;

    private void Start() {
        buildGrid = placementMode.Grid;
        CreateArrowObject();
        hoveredCellsMarker = new CellMarker(CreateIndicatorObject, buildGrid);
        //CreateGridPlane();
    }

    private void OnEnable() {
        placementMode.moduleChanged.AddListener(OnModuleChanged);
        placementMode.moduleRotated.AddListener(OnModuleRotated);
        placementMode.enterPlacementMode.AddListener(OnEnterPlacementMode);
        placementMode.exitPlacementMode.AddListener(OnExitPlacementMode);
        placementMode.gridSwitched.AddListener(OnGridSwitched);
    }

    private void OnDisable() {
        placementMode.moduleChanged.RemoveListener(OnModuleChanged);
        placementMode.moduleRotated.RemoveListener(OnModuleRotated);
        placementMode.enterPlacementMode.RemoveListener(OnEnterPlacementMode);
        placementMode.exitPlacementMode.RemoveListener(OnExitPlacementMode);
        placementMode.gridSwitched.RemoveListener(OnGridSwitched);
    }

    private void OnEnterPlacementMode() {
        placementPreview.SetActive(true);
        arrowObject.SetActive(true);
        //gridPlaneObject.SetActive(true);
        UpdateGroundIndicators();
    }

    private void OnExitPlacementMode() {
        placementPreview.SetActive(false);
        arrowObject.SetActive(false);
        //gridPlaneObject.SetActive(false);
        hoveredCellsMarker.RemoveAllMarkers();
    }

    private void OnGridSwitched(FactoryGrid newGrid) {
        this.buildGrid = newGrid;
    }

    private void OnModuleRotated(Facing newFacing) {
        if(selectedObjectData == null) {
            return;
        }

        Quaternion newRotation = buildGrid.Rotation * newFacing.GetRotationFromFacing();
        currentRotation = newRotation;
        placementPreview.transform.rotation = newRotation;

        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
    }

    private void OnModuleChanged(GridObjectSO newBuilding) {
        Destroy(placementPreview);
        arrowObject.SetActive(newBuilding != null);

        if(newBuilding == null) {
            hoveredCellsMarker.RemoveAllMarkers();
            return;
        }
        selectedObjectData = newBuilding;

        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        
        placementPreview = Instantiate(newBuilding.PreviewPrefab);
    
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
        OnModuleRotated(placementMode.CurrentFacing);
    }

    private void Update() {
        UpdatePreview();
    }

    private void UpdatePreview() {
        if(selectedObjectData == null) {
            return;
        }

        Vector3 mouseHitPosition = placementMode.CurrentMouseWorldPos;
        //UpdateGridPlane(mouseHitPosition);
        Vector2Int subgridOriginCoord = buildGrid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);

        if(subgridOriginCoord == lastOriginCoord) {
            //Mouse has not moved enough to move the building
            return;
        }

        UpdatePreviewPositions(subgridOriginCoord, buildingDimensions);
    }

    private void UpdatePreviewPositions(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        UpdatePreviewBuilding(buildingOriginCoord, buildingDimensions);
        UpdateGroundIndicators();
        UpdateArrowObject(buildingOriginCoord, buildingDimensions);
        
        lastOriginCoord = buildingOriginCoord;
    }

    private void UpdateGroundIndicators() {
        List<Vector2Int> hoveredPositions = placementMode.GetHoveredPositions();
        hoveredCellsMarker.MarkPositions(hoveredPositions);
        for(int i = 0; i < hoveredPositions.Count; i++) {
            GameObject markerObject = hoveredCellsMarker.GetMarker(i);
            Vector2Int buildPosition = hoveredPositions[i];
            bool isBuildable = buildGrid.CellWithinBounds(buildPosition) && !buildGrid.PositionIsOccupied(buildPosition);
            markerObject.transform.position = buildGrid.GetCellCenter(buildPosition) + buildGrid.Rotation * indicatorOffset;
            Color color = isBuildable ? Color.green : Color.red;
            color = new Vector4(color.r, color.g, color.b, groundIndicatorAlpha);
            markerObject.GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void UpdatePreviewBuilding(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        if(placementPreview != null) {
            Vector3 subgridCenter = buildGrid.GetSubgridCenter(buildingOriginCoord, buildingDimensions);
            placementPreview.transform.position = subgridCenter;
        }
    }

    private void UpdateArrowObject(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        Vector3 buildingCenter = buildGrid.GetSubgridCenter(buildingOriginCoord, buildingDimensions);
        Vector3 arrowDelta = currentRotation * (new Vector3(-buildGrid.CellSize.x * buildingDimensions.x, 0, 0) / 2f + Vector3.left * buildGrid.CellSize.x / 2f);
        Vector3 arrowPosition = buildingCenter + arrowDelta;
        arrowObject.transform.position = arrowPosition + buildGrid.Rotation * indicatorOffset;
        Quaternion moduleRot = currentRotation;
        arrowObject.transform.rotation = Quaternion.Euler(arrowObjectRotationOffset) * Quaternion.Euler(moduleRot.eulerAngles.x, moduleRot.eulerAngles.z, -moduleRot.eulerAngles.y);
    }

    private void UpdateGridPlane(Vector3 mouseWorldPos) {
        gridPlaneObject.GetComponent<Renderer>().material.SetVector("_CenterPos", new Vector4(mouseWorldPos.x, mouseWorldPos.y, mouseWorldPos.z));
    }

    private GameObject CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = new Vector3(buildGrid.CellSize.x, buildGrid.CellSize.y, 1);
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = buildGrid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        return newIndicatorObject;
    }

    private void CreateArrowObject() {
        arrowObject = Instantiate(arrowPrefab);
        arrowObject.transform.rotation = Quaternion.Euler(arrowObjectRotationOffset);
    }

    private void CreateGridPlane() {
        gridPlaneObject = Instantiate(gridPlanePrefab);
        Vector3 gridSize = new Vector3(buildGrid.CellSize.x * buildGrid.Columns, 0, buildGrid.CellSize.y * buildGrid.Rows);
        List<Vector2Int> positionsToRemove = FindAllOutOfBoundsPositions();

        GridPixelManager pixelManager = gridPlaneObject.GetComponent<GridPixelManager>();
        pixelManager.Initialize(new Vector2Int(buildGrid.Columns, buildGrid.Rows));
        pixelManager.TurnOffCells(positionsToRemove);

        gridPlaneObject.transform.position = buildGrid.Origin + gridSize / 2 + Vector3.up * 0.01f;
        gridPlaneObject.transform.rotation = Quaternion.Euler(0, buildGrid.Rotation.eulerAngles.y, 0);
        gridPlaneObject.transform.localScale = gridSize / 10f;

        Material gridPlaneMat = gridPlaneObject.GetComponent<Renderer>().material;
        gridPlaneMat.SetFloat("_Rotation", buildGrid.Rotation.eulerAngles.y);
        gridPlaneMat.SetVector("_TileSize", new Vector4(buildGrid.CellSize.x, buildGrid.CellSize.y, 0, 0));
        Vector2 gridOffset = new Vector2(-buildGrid.Origin.x, -buildGrid.Origin.z);
        gridPlaneMat.SetVector("_GridOffset", new Vector4(gridOffset.x, gridOffset.y));
    }

    private List<Vector2Int> FindAllOutOfBoundsPositions() {
        List<Vector2Int> positionsOutOfBounds = new List<Vector2Int>();
        for(int y = 0; y < buildGrid.Rows; y++) {
            for(int x = 0; x < buildGrid.Columns; x++) {
                Vector2Int position = new Vector2Int(x, y);
                if(!buildGrid.CellWithinBounds(position)) {
                    positionsOutOfBounds.Add(position);
                }
            }
        }
        return positionsOutOfBounds;
    }
}
