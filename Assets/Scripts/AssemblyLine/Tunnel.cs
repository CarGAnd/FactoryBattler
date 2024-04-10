
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Tunnel : AssemblyPiece
{
    public bool IsConnected {get; private set;}
    public bool IsReceiver {get; private set;}
    private bool canReceive = true;

    private Tunnel connectedTunnel;
    public Tunnel(AssemblyPieceData data) : base(data)
    {

    }

    public override void SetNextTransportable(ITransportable nextTransportable)
    {
        nextPiece = nextTransportable;
        if(connectedTunnel != null && nextPiece is Tunnel && !IsReceiver)
        {
            connectedTunnel.canReceive = false;
        }
        else if(nextPiece == null)
        {
            if(!IsReceiver)
            {
                IsConnected = false;
                canReceive = true;
            }
        }
    }

    public override Vector2Int GetNextCellCoords()
    {
        if(nextPiece != null)
        {
            return nextPiece.GetGridCoords();
        }
        else if (!IsConnected && TryConnectTunnel())
        {
            if(nextPiece != null)
            {
                return nextPiece.GetGridCoords();
            }
            else
            {
                return cellCoords + facing.GetIntDirection();
            }
        }
        else
        {
            return cellCoords + facing.GetIntDirection();
        }
    }

    public override List<Facing> GetInputDirections()
    {
        if(canReceive)
        return new List<Facing>() { Facing };

        return new List<Facing>();
    }
    public override string ToString()
    {
        return "Tunnel : " + cellCoords;
    }

    public override void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid)
    {
        base.OnPlacedOnGrid(startCell, grid);

    }
    public bool TryConnectTunnel()
    {
        Tunnel queryTunnel = null;
        for(int i = 1; i <= distance; i++)
        {
            Vector2Int nextCell = cellCoords + facing.GetIntDirection() * i;
            if(grid.GetObjectAt(nextCell) is Tunnel)
            {
                queryTunnel = grid.GetObjectAt(nextCell) as Tunnel;
            }
            if(queryTunnel != null && !queryTunnel.IsConnected)
            {
                if(queryTunnel.facing == facing)
                {
                    IsConnected = true;
                    queryTunnel.IsConnected = true;
                    queryTunnel.IsReceiver = true;
                    connectedTunnel = queryTunnel;
                    queryTunnel.connectedTunnel = this;
                    nextPiece = queryTunnel;
                        
                    List<Facing> connectionDirections = Facing.GetPerpendicularFacings();
                    Quaternion entryRotation = connectionDirections[0].GetRotationFromFacing()*grid.Rotation;
                    Quaternion exitRotation = connectionDirections[1].GetRotationFromFacing()*grid.Rotation;
                    gameObject.transform.rotation = entryRotation;
                    queryTunnel.gameObject.transform.rotation = exitRotation;
                
                    return true;
                    
                }
            }
        }
        for (int i = 1; i <= distance; i++)
        {
            Vector2Int nextCell = cellCoords - (facing.GetIntDirection() * i);
            if (grid.GetObjectAt(nextCell) is Tunnel)
            {
                queryTunnel = grid.GetObjectAt(nextCell) as Tunnel;
            }
            if (queryTunnel != null && !queryTunnel.IsConnected)
            {
                if (queryTunnel.facing == facing)
                {
                    connectedTunnel = queryTunnel;
                    queryTunnel.connectedTunnel = this;
                    queryTunnel.IsConnected = true;
                    IsReceiver = true;
                    IsConnected = true;
                    queryTunnel.SetNextTransportable(this);
                    
                    List<Facing> connectionDirections = Facing.GetPerpendicularFacings();
                    Quaternion entryRotation = connectionDirections[0].GetRotationFromFacing()*grid.Rotation;
                    Quaternion exitRotation = connectionDirections[1].GetRotationFromFacing()*grid.Rotation;
                    gameObject.transform.rotation = exitRotation;
                    queryTunnel.gameObject.transform.rotation = entryRotation;
                    
                    return true;
                }
            }
        }

        return false;
    }

    public override void RemoveFromGrid(FactoryGrid grid)
    {
        if(IsConnected && !IsReceiver)
        {
            connectedTunnel.IsConnected = false;
            connectedTunnel.IsReceiver = false;
            connectedTunnel.canReceive = true;
            connectedTunnel.connectedTunnel = null;
        }
        else if(IsConnected)
        {
            connectedTunnel.IsConnected = false;
            connectedTunnel.connectedTunnel = null;
        }
        connectedTunnel = null;
        base.RemoveFromGrid(grid);
    }
}
