using System.Collections;
using System.Collections.Generic;
using PlayerSystem;
using UnityEngine;

[RequireComponent(typeof(ModuleInputOutput))]
public abstract class Module : MonoBehaviour, IGridObject, IAssemblyLineUser, IPlayerOwned
{
    private ModuleInputOutput inputOutput;
    private ModuleSO moduleSettings;
    private Vector2Int gridPosition;
    private FactoryGrid grid;
    private IPlayer owner;
    public IPlayer Owner { get => owner; set => owner = value; }

    private void Awake() {
        inputOutput = GetComponent<ModuleInputOutput>();
    }

    public void Initialize(ModuleSO moduleSettings, Facing facing) {
        inputOutput.Initialize(moduleSettings, facing);
        this.moduleSettings = moduleSettings;
    } 

    protected void SendObjectOut(AssemblyTravelingObject obj) {
        inputOutput.SendToOutput(obj);
    }

    protected bool OutputHasRoom() {
        return inputOutput.OutputHasRoom();
    }

    protected AssemblyTravelingObject GetObjectIn() {
        return inputOutput.ReceiveFromInput();    
    }

    public void ConnectToAssemblyLine(AssemblyLineSystem assemblyLineSystem) {
        inputOutput.ConnectToAssemblyLine(assemblyLineSystem);
    }

    public void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid) {
        inputOutput.OnPlacedOnGrid(startCell, grid);
        this.gridPosition = startCell;
        this.grid = grid;
    }

    public void RemoveFromGrid(FactoryGrid grid) {
        inputOutput.Destroy();
    }

    public void DestroyObject() {
        grid.RemoveObject(gridPosition);
        Destroy(gameObject);
    }

    public ObjectPlacementData Serialize() {
        ObjectPlacementData data = new ObjectPlacementData
        {
            prefabId = moduleSettings.ID,
            x = gridPosition.x,
            y = gridPosition.y,
            facing = Facing.East,
        };
        return data;
    }

    public void Deserialize(ObjectPlacementData data) {
    
    }

    public GameObject GetGameObject() {
        return gameObject;
    }
}
