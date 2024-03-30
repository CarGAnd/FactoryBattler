using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridText : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private TextMeshPro text;

    private void Start() {
        text.transform.forward = Vector3.up;
        text.transform.up = Vector3.forward;
    }

    private void Update() {
        Vector2Int mouseGridPos = grid.GetCellCoords(mouseInput.GetMousePosOnGrid(grid));
        Vector3 cellPos = grid.GetCellCenter(mouseGridPos);
        Vector3 textPos = cellPos + Vector3.up * 0.6f;
        text.transform.position = textPos;
        text.text = string.Format("({0}, {1})", mouseGridPos.x, mouseGridPos.y);
    }
}
