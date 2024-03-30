using System.Collections.Generic;
using UnityEngine;

public interface IPlacementStrategy {
    void UpdateInput(MouseInput mouseInput);
    List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid);
}

public class NoPlacement : IPlacementStrategy {

    public NoPlacement() {
    
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        return new List<Vector2Int>();    
    }

    public void UpdateInput(MouseInput mouseInput) {
        
    }
}
