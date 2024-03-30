using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyerBelt : AssemblyPiece
{
    public ConveyerBelt(AssemblyPieceData data, AssemblyLineSystem assemblyLineSystem) : base(data, assemblyLineSystem)
    {

    }
    public override string ToString()
    {
        return "Conveyer Belt at : " + cellCoords;
    }
    //Maybe this class is more specific towards animation properties.
}
