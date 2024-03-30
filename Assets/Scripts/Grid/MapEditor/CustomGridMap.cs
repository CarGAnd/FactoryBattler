using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGridMap : MonoBehaviour
{
    [SerializeField] private FactoryGrid grid;
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
            rotMatrix.SetTRS(grid.Origin, grid.Rotation, Vector3.one);
            Gizmos.matrix = rotMatrix;
            for(int y = 0; y < currentHeight; y++) {
                for(int x = 0; x < currentWidth; x++) {
                    Color c = GetMaskValue(x, y) ? Color.green : Color.red;
                    c.a = 0.5f;
                    Gizmos.color = c;
                    Vector3 center = grid.GetCellCenter(new Vector2Int(x, y));
                    Vector3 cubePos = Quaternion.Inverse(grid.Rotation) * (center - grid.Origin);
                    Gizmos.DrawCube(cubePos, new Vector3(grid.CellSize.x, 0.1f, grid.CellSize.y));
                }
            }
            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
