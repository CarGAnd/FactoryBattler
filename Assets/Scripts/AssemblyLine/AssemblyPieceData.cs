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
    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, Facing facing, AssemblyLineSystem assemblyLineSystem) {
        ObjectFacing = facing;
        Position = position;
        Rotation = rotation;
        //Something about referencing cost variable and adding cost to it.

        AssemblyPiece assemblyPiece = assemblyLineSystem.CreateAssemblyPiece(this);
        return assemblyPiece;
    }

    public override IPlacementStrategy GetPlacementHandler(FactoryGrid grid, PlacementMode placementMode) {
        return new PathPlacer(this, grid, placementMode);
    }
}
