using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    [HideInInspector] public UnityEvent<AssemblyTravelingObject> receivedObject;
    [HideInInspector] public UnityEvent<AssemblyTravelingObject, Vector2Int, Vector2Int> sentObject;
    public FactoryGrid Grid { get; private set; }

    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private Facing facing;

    private List<Port> inputPorts;
    private List<Port> outputPorts;
    private List<Port> allPorts;

    private int maxStorageCount = 1;
    private ObjectStorage<AssemblyTravelingObject> inputStorage;
    private ObjectStorage<AssemblyTravelingObject> outputStorage;
    private AssemblyLineSystem assemblyLineSystem;

    public void Initialize(ModuleSO moduleSettings, Facing facing, AssemblyLineSystem assemblyLineSystem) {
        this.moduleSettings = moduleSettings;
        this.facing = facing;
        this.assemblyLineSystem = assemblyLineSystem;
    }

    private void Awake() {
        inputStorage = new ObjectStorage<AssemblyTravelingObject>(maxStorageCount);
        outputStorage = new ObjectStorage<AssemblyTravelingObject>(maxStorageCount);
        inputPorts = new List<Port>();
        outputPorts = new List<Port>();
        allPorts = new List<Port>();
    }

    public bool OutputHasRoom() {
        return outputStorage.HasRoom();
    }

    private void CreatePorts() {
        List<PortSettings> portSettings = moduleSettings.GetPorts(facing, originCell); 
        foreach (PortSettings ps in portSettings) {
            Port port = new Port(ps.gridPosition, ps.inputDirection, ps.outputDirection);
            if (ps.isInput) {
                inputPorts.Add(port);
            }
            if (ps.isOutput) {
                outputPorts.Add(port);
                port.sentObject.AddListener((AssemblyTravelingObject a, Vector2Int from, Vector2Int to) => sentObject.Invoke(a, from, to));
            }
            allPorts.Add(port);
        }
    }

    public AssemblyTravelingObject ReceiveFromInput() {
        if(inputStorage.HasObjectsStored()) {
            return inputStorage.GetFromStorage();
        }
        else {
            return null;
        }
    }

    public void SendToOutput(AssemblyTravelingObject aObject) {
        if (outputStorage.HasRoom()) {
            outputStorage.PutIntoStorage(aObject);
        }
    }

    private void Update() {
        if(this.Grid == null) {
            return;
        }
        Tick();
    }

    private void Tick() { 
        foreach(Port p in inputPorts) {
            if(p.HasInput() && inputStorage.HasRoom()) {
                AssemblyTravelingObject newObject = p.ReceiveFromPort();
                inputStorage.PutIntoStorage(newObject);
                receivedObject.Invoke(newObject);
            }
        }

        foreach(Port p in outputPorts) {
            if(!p.HasOutput() && outputStorage.HasObjectsStored()) {
                AssemblyTravelingObject obj = outputStorage.GetFromStorage();
                p.SendToPort(obj);
            }
        }
    }

    private void PlacePorts() {
        foreach(Port p in allPorts) {
            assemblyLineSystem.PlaceTransportablePiece(p);
        }
    }

    private void RemovePorts() {
        foreach (Port p in allPorts) {
            assemblyLineSystem.RemoveTransportablePiece(p);
        }
    }

    public void OnPlacedOnGrid(Vector2Int startCell, FactoryGrid grid) {
        this.originCell = startCell;
        this.Grid = grid;
        CreatePorts();
        PlacePorts();
    }

    public void DestroyObject() {
        RemoveFromGrid(Grid);
        Destroy(gameObject);
    }

    public void RemoveFromGrid(FactoryGrid grid) {
        RemovePorts();
        grid.RemoveObject(originCell + moduleSettings.GetLayoutShape(facing)[0]);
    }

    #region Debug
    private void OnDrawGizmos() {
        if(Grid == null) {
            return;
        }
        Matrix4x4 gizmoMatrix = new Matrix4x4();
        gizmoMatrix.SetTRS(Grid.Origin, Grid.Rotation, Vector3.one);
        Gizmos.matrix = gizmoMatrix;
  
        foreach(Port p in inputPorts) {
            Gizmos.color = Color.green;
            Vector2Int facingDirection = p.GetInputDirections()[0].GetIntDirection();
            Vector3 cellCenter = Grid.GetCellCenter(p.GetGridCoords() - facingDirection);
            Vector3 cubePos = Quaternion.Inverse(Grid.Rotation) * (cellCenter - Grid.Origin);
            Gizmos.DrawWireCube(cubePos, new Vector3(Grid.CellSize.x, 3, Grid.CellSize.y));
        }

        foreach(Port p in outputPorts) {
            Gizmos.color = Color.red;
            Vector3 cellCenter = Grid.GetCellCenter(p.GetNextCellCoords());
            Vector3 cubePos = Quaternion.Inverse(Grid.Rotation) * (cellCenter - Grid.Origin);
            Gizmos.DrawWireCube(cubePos, new Vector3(Grid.CellSize.x, 3, Grid.CellSize.y));    
        }
        Gizmos.matrix = Matrix4x4.identity;
    }
    #endregion
}

public class ObjectStorage<T> {

    private int maxCount;
    private List<T> objectStored;

    public ObjectStorage(int maxCount) {
        this.maxCount = maxCount;
        objectStored = new List<T>();
    }

    public bool HasRoom() {
        return objectStored.Count < maxCount;
    }

    public bool HasObjectsStored() {
        return objectStored.Count > 0;
    }

    public void PutIntoStorage(T obj) {
        if (HasRoom()) {
            objectStored.Add(obj);
        }
    }

    public T GetFromStorage() {
        if(HasObjectsStored()) {
            T obj = objectStored[0];
            objectStored.RemoveAt(0);
            return obj;
        }
        else {
            return default(T);
        }
    }
}
