using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GridSystem {

    public class GridSize : MonoBehaviour
    {
        [field: SerializeField] public int Rows { get; private set; }
        [field: SerializeField] public int Columns { get; private set; }
    }
}
