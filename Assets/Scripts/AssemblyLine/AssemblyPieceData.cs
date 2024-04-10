using UnityEngine;

[CreateAssetMenu(fileName = "AssemblyPieceData", menuName = "AssemblyPieces/AssemblyPiece", order = 1)]
public class AssemblyPieceData : GridObjectSO
{
    public int movementDistance;
    public int cost;
    public AssemblyPieceType type;
    public Facing ObjectFacing { get; private set; }
    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, Facing facing) {
        ObjectFacing = facing;
        Position = position;
        Rotation = rotation;
        //Something about referencing cost variable and adding cost to it.

        AssemblyPiece assemblyPiece = CreateAssemblyPiece(this);
        return assemblyPiece;
    }

    public override IPlacementStrategy GetPlacementHandler(FactoryGrid grid, PlacementMode placementMode) {
        return new PathPlacer(this, grid, placementMode);
    }

    private AssemblyPiece CreateAssemblyPiece(AssemblyPieceData assemblyPieceData)
    {
        AssemblyPieceType type = assemblyPieceData.type;
        AssemblyPiece newPiece = null;

        switch (type)
        {
            case AssemblyPieceType.ConveyerBelt:
                newPiece = new ConveyerBelt(assemblyPieceData);
                break;
            case AssemblyPieceType.Cannon:
                //newPiece = new Cannon(data);
                break;
            case AssemblyPieceType.Tunnel:
                newPiece = new Tunnel(assemblyPieceData);
                break; 
        }
        return newPiece;
    }

}
