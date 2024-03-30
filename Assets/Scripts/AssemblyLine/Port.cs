using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class Port : ITransportable
{
    public UnityEvent<AssemblyTravelingObject, Vector2Int, Vector2Int> sentObject;

    private Vector2Int position;
    private Facing inputDirection;
    private Facing outputDirection;
    private Vector2Int outputPosition;
    private ITransportable connectedObject;
    private AssemblyTravelingObject outputObject;
    private AssemblyTravelingObject inputObject;

    public Facing Facing => outputDirection;

    public Port(Vector2Int position, Facing inputFacing, Facing outputFacing) {
        this.position = position;
        this.inputDirection = inputFacing;
        this.outputDirection = outputFacing;
        this.outputPosition = position + outputDirection.GetIntDirection();
        sentObject = new UnityEvent<AssemblyTravelingObject, Vector2Int, Vector2Int>();
    }

    public AssemblyTravelingObject ReceiveFromPort() {
        AssemblyTravelingObject obj = inputObject;
        inputObject = null;
        return obj;
    }

    public void SendToPort(AssemblyTravelingObject obj) {
        outputObject = obj;
    }
    
    public void ReceivedObject(AssemblyTravelingObject aObject) {
        inputObject = aObject;
    }

    private void SendObject() {
        connectedObject.ReceivedObject(outputObject);
        sentObject.Invoke(outputObject, position, outputPosition);
        outputObject = null;
    }

    public bool HasInput() {
        return inputObject != null;
    }

    public bool HasOutput() {
        return outputObject != null;
    }

    public TransportState GetState() {
        return HasInput() ? TransportState.Occupied : TransportState.Available;
    }

    public void TransportTick() {
        if(!HasOutput()) {
            return;
        }

        if(connectedObject != null && connectedObject.GetState() == TransportState.Available) {
            SendObject();
        }
    }

    public Vector2Int GetNextCellCoords() {
        return outputPosition;
    }

    public Vector2Int GetGridCoords() {
        return position;
    }

    public void SetNextTransportable(ITransportable nextTransportable) {
        this.connectedObject = nextTransportable;
    }

    public List<Facing> GetInputDirections() {
        return new List<Facing>() { inputDirection };
    }
}
