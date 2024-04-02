using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BuildingSelector : MonoBehaviour
{
    [HideInInspector] public UnityEvent<GridObjectSO> selectedObjectChanged;

    [SerializeField] private List<GridObjectSO> placeableObjects;

    public GridObjectSO SelectedObject { get; private set; }

    public void SetObjectSelection(GridObjectSO newSelectedObject) {
        SelectedObject = newSelectedObject;
        selectedObjectChanged?.Invoke(newSelectedObject);
    }

    private void Update() {
        int keyPressed = GetNumberKeyPressed();
        if(keyPressed >= 0) {
            if(keyPressed < placeableObjects.Count) {
                SetObjectSelection(placeableObjects[keyPressed]);
            }
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            SetObjectSelection(null);
        }
    }

    private int GetNumberKeyPressed() {
        int keyPressed = -1;
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            keyPressed = 1;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            keyPressed = 2;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            keyPressed = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            keyPressed = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            keyPressed = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            keyPressed = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            keyPressed = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            keyPressed = 8;
        }
        return keyPressed - 1;
    }
}
