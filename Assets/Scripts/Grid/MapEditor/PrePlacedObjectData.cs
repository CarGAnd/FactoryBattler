using UnityEngine;


[System.Serializable]
[ExecuteInEditMode]
public class PrePlacedObjectData : MonoBehaviour {
    public FactoryGrid grid;
    public Facing facing;
    public GridObjectSO objectDefinition;
    public Vector2Int gridPosition;

    private void OnDestroy() {
        GridInitializer gridInitializer = FindObjectOfType<GridInitializer>();
        if(gridInitializer != null) {
            gridInitializer.RemoveObject(this);
        }
    }

    private void Awake() {
        GridInitializer gridInitializer = FindObjectOfType<GridInitializer>();
        if (gridInitializer != null) {
            gridInitializer.AddNewObject(this);
        }
    }
}

