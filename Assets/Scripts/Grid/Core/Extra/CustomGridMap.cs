using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class CustomGridMap : MonoBehaviour
{
    [SerializeField] private GridSystem.GridLayout gridLayout;
    [SerializeField] private GridSize gridSize; //Used in custom inspector
    [SerializeField, HideInInspector] private bool[] customSpawnMask;
    [SerializeField, HideInInspector] private int currentWidth;
    [SerializeField, HideInInspector] private int currentHeight;

    public void SetMaskValue(int x, int y, bool value) {
        if (!IsWithingBounds(x, y)) {
            return;
        }
        customSpawnMask[y * currentWidth + x] = value;
    }

    private bool IsWithingBounds(int x, int y) {
        return x >= 0 && x < currentWidth && y >= 0 && y < currentHeight; 
    }

    public bool GetMaskValue(int x, int y) {
        return customSpawnMask[y * currentWidth + x];
    }

    [SerializeField] private bool showCustomLayout;

    private void OnDrawGizmosSelected() {
        if(showCustomLayout) {
            Matrix4x4 rotMatrix = new Matrix4x4();
            rotMatrix.SetTRS(gridLayout.Origin, gridLayout.Rotation, Vector3.one);
            Gizmos.matrix = rotMatrix;
            for(int y = 0; y < currentHeight; y++) {
                for(int x = 0; x < currentWidth; x++) {
                    Color c = GetMaskValue(x, y) ? Color.green : Color.red;
                    c.a = 0.5f;
                    Gizmos.color = c;
                    Vector3 center = gridLayout.GetCellCenter(new Vector2Int(x, y));
                    Vector3 cubePos = Quaternion.Inverse(gridLayout.Rotation) * (center - gridLayout.Origin);
                    Gizmos.DrawCube(cubePos, new Vector3(gridLayout.CellSize.x, 0.1f, gridLayout.CellSize.y));
                }
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
