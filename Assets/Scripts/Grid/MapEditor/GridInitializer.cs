using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FactoryGrid))]
public class GridInitializer : MonoBehaviour
{
    [SerializeField] private bool showGizmos;
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private List<PrePlacedObjectData> prePlacedObjects;
    private FactoryGrid grid;
    
    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<FactoryGrid>();
        foreach(PrePlacedObjectData objectData in prePlacedObjects) {
            IGridObject gridObject = objectData.gameObject.GetComponent<IGridObject>();
            //If the object in the editor has an IGridObject component, we can keep the existing object and just connect it to the grid. Otherwise we create a new object from the object definition SO 
            //TODO: keeping the existing object is disabled until we can connect an object to the assembly line system after object creating. 
            //Currently it is only possible to connect an object to the assembly line system when the object is created
            if(gridObject != null && false) {
                modulePlacer.ConnectExistingBuilding(gridObject, objectData.gridPosition, objectData.objectDefinition.GetLayoutShape(objectData.facing));
            }
            else {
                modulePlacer.PlaceModule(objectData.objectDefinition, objectData.gridPosition, objectData.facing);
                Destroy(objectData.gameObject);
            }
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) {
            return;
        }

        grid = GetComponent<FactoryGrid>();
        Gizmos.color = Color.yellow;
        Matrix4x4 rotMatrix = new Matrix4x4();
        rotMatrix.SetTRS(grid.Origin, grid.Rotation, Vector3.one);
        Gizmos.matrix = rotMatrix;
        foreach(PrePlacedObjectData objectData in prePlacedObjects) {
            GridObjectSO gridObject = objectData.objectDefinition;
            foreach(Vector2Int delta in gridObject.GetLayoutShape(objectData.facing)) {
                Vector2Int gridPos = delta + objectData.gridPosition;
                Vector3 pos = grid.GetCellCenter(gridPos);
                Vector3 cubePos = Quaternion.Inverse(grid.Rotation) * (pos - grid.Origin);
                Vector3 cubeSize = new Vector3(1, 0.01f, 1) * Mathf.Min(grid.CellSize.x, grid.CellSize.y);
                Gizmos.DrawCube(cubePos, cubeSize);
            }
        }
    }

    public void AddNewObject(PrePlacedObjectData newObjectData) {
        if (!prePlacedObjects.Contains(newObjectData)) {
            prePlacedObjects.Add(newObjectData);
        }
    }

    public void RemoveObject(PrePlacedObjectData objectToRemove) {
        prePlacedObjects.Remove(objectToRemove);
    }
}

