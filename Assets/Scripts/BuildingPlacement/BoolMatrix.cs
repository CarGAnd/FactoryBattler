using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolMatrix
{
    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    
    [SerializeField] private bool[] values;

    public bool GetValueAt(int x, int y) {
        return values[(Height - y - 1) * Width + x];
    }

    public bool[,] GetAllValues() {
        bool[,] allValues = new bool[Height, Width];
        for(int y = 0; y < Height; y++) {
            for(int x = 0; x < Width; x++) {
                allValues[y, x] = values[y * Width + x];
            }
        }
        return allValues;
    }

    public List<Vector2Int> GetTrueValues() {
        List<Vector2Int> trueValues = new List<Vector2Int>();
        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                if (GetValueAt(x, y)) {
                    trueValues.Add(new Vector2Int(x, y));
                }
            }
        }
        return trueValues;
    }

}
