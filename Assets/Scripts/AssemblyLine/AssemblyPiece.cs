using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public abstract class AssemblyPiece : IGridInteractable, ITransportable
{
    protected Facing facing = default;
    public Facing Facing { get => facing; }
    protected Vector2Int cellCoords;
    protected ITransportable nextPiece;
    protected int distance;
    protected AssemblyTravelingObject travelingObject;
    protected FactoryGrid grid;
    protected AssemblyLineSystem assemblyLineSystem;
    protected GameObject gameObject;
    public abstract override string ToString();

    protected AssemblyPiece(AssemblyPieceData data, AssemblyLineSystem assemblyLineSystem)
    {
        this.facing = data.ObjectFacing;
        this.distance = data.movementDistance;
        this.assemblyLineSystem = assemblyLineSystem;
        this.gameObject = data.ModulePrefab;
    }
    public Vector2Int Movement()
    {
        Vector2Int movement = Vector2Int.zero;
        switch(facing)
        {
            case Facing.North:
                movement = Vector2Int.up;
                break;
            case Facing.East:
                movement = Vector2Int.right;
                break;
            case Facing.South:
                movement = Vector2Int.down;
                break;
            case Facing.West:
                movement = Vector2Int.left;
                break;
        }
        return movement * distance;
    }
///////////////////////////// ITransportable /////////////////////////////
    public virtual void SetNextTransportable(ITransportable nextTransportable)
    {
        this.nextPiece = nextTransportable;
    }
    public virtual void TransportTick()
    {
        if(GetState() == TransportState.Available)
        {
            return;
        }
        if(nextPiece != null && nextPiece.GetState() == TransportState.Available)
        {
            nextPiece.ReceivedObject(travelingObject);
            travelingObject.MoveToPiece(cellCoords, nextPiece.GetGridCoords(), grid);
            travelingObject = null;   
        }
    }

    public virtual void ReceivedObject(AssemblyTravelingObject travelingObject)
    {
        this.travelingObject = travelingObject;
    }
    public virtual Vector2Int GetNextCellCoords()
    {
        return cellCoords + Movement();
    }

    public TransportState GetState()
    {
        return travelingObject == null ? TransportState.Available : TransportState.Occupied;
    }

    public Vector2Int GetGridCoords()
    {
        return cellCoords;
    }

    public virtual List<Facing> GetInputDirections()
    {
        List<Facing> perpendicularFacings = facing.GetPerpendicularFacings();
        return new List<Facing> { facing, perpendicularFacings[0], perpendicularFacings[1] };
    }

///////////////////////////// IGridObject /////////////////////////////
    public virtual void RemoveFromGrid(FactoryGrid grid)
    {
        assemblyLineSystem.RemoveTransportablePiece(this);
        grid.RemoveObject(cellCoords);
        
    }
    public virtual void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid) {
        this.cellCoords = startCell;
        this.grid = grid;
        this.gameObject = MonoBehaviour.Instantiate(gameObject, grid.GetCellCenter(cellCoords), grid.Rotation * facing.GetRotationFromFacing());
        assemblyLineSystem.PlaceTransportablePiece(this);
    }
    public void DestroyObject() {
        RemoveFromGrid(grid);
        MonoBehaviour.Destroy(gameObject);
    }
///////////////////////////// IGridInteractable /////////////////////////////
    public bool IsPlaced()
    {
        throw new System.NotImplementedException();
    }

    public bool IsSelected()
    {
        throw new System.NotImplementedException();
    }

    public void OnSelected()
    {
        throw new System.NotImplementedException();
    }

    public object Serialize() {
        throw new NotImplementedException();
    }

    public void Deserialize(object data) {
        throw new NotImplementedException();
    }
}


