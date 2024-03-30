using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem {
    public interface ISearchable {
        List<Vector2Int> GetNeighbors(Vector2Int coord);
    }
}
