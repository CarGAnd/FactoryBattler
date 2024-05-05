using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridObject : ISaveable {
    void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid);

    // Removes the object from the grid.
    void RemoveFromGrid(FactoryGrid grid);
    
    // Destroy the gridObject and any associated prefab
    void DestroyObject();

    //Get the associated gameObject
    GameObject GetGameObject();
}

public interface IGridInteractable : IGridObject {
    // Method called when the object is selected (e.g., through user interaction).
    void OnSelected();

    // Returns whether the object is currently selected.
    bool IsSelected();

    // Returns whether the object is currently placed on the grid.
    bool IsPlaced();
}
