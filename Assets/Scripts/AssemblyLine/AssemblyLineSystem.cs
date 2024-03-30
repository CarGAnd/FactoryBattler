using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class AssemblyLineSystem : MonoBehaviour
{
    private UnityEvent TransportTick;
    [SerializeField] private FactoryGrid grid;
    [SerializeField] private float tickRate = 0.200f;
    private float currentTick = 0f;
    private List<AssemblyLine> assemblyLines;
    public List<AssemblyLine> AssemblyLines { get => assemblyLines; }
    private AssemblyLineManager assemblyLineManager;

    //To Be Removed -- It's part of debugging
    public GameObject travelingObjectPrefab;

    private void Awake()
    {
        assemblyLines = new List<AssemblyLine>();
        TransportTick = new UnityEvent();
        assemblyLineManager = new AssemblyLineManager(assemblyLines, this);
    }
    // This tick could probably be moved to a GameManager.
    private void Update()
    {
        currentTick += Time.deltaTime;
        if(currentTick >= tickRate)
        {
            TransportTick.Invoke();
            currentTick -= tickRate;
        }
    }
    [Button]
    public void DebugAssemblyLines()
    {
        foreach(AssemblyLine line in assemblyLines)
        {
            line.DebugLine();
        }
    }
    [Button]
    public void DebugNumberOfAssemblyLines()
    {
        Debug.Log("Number of Assembly Lines: " + assemblyLines.Count);
    }
    [Button]
    public void AddTravelingAssemblyPiece()
    {
        HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
        foreach(AssemblyLine line in assemblyLines)
        {
            visitedLines.Clear();
            SpawnAtAllConnectingLines(line, visitedLines);
        }

    }

    private void SpawnAtAllConnectingLines(AssemblyLine line, HashSet<AssemblyLine> visitedLines)
    {
        if (visitedLines.Contains(line))
            return;

        visitedLines.Add(line);
        List<AssemblyLine> connectingLines = line.GetAllConnections();

        foreach(AssemblyLine connectingLine in connectingLines)
        {
            SpawnAtAllConnectingLines(connectingLine, visitedLines);
        }

        SpawnTravelingPieceAtPiece(line.GetStartPiece());
    }

    private void SpawnTravelingPieceAtPiece(ITransportable piece)
    {
        AssemblyTravelingObject travelingObject = Instantiate(travelingObjectPrefab, grid.GetCellCenter(piece.GetGridCoords()), Quaternion.identity).GetComponent<AssemblyTravelingObject>();
        piece.ReceivedObject(travelingObject);
    }

    public AssemblyPiece CreateAssemblyPiece(AssemblyPieceData assemblyPieceData)
    {
        AssemblyPieceType type = assemblyPieceData.type;
        AssemblyPiece newPiece = null;

        switch (type)
        {
            case AssemblyPieceType.ConveyerBelt:
                newPiece = new ConveyerBelt(assemblyPieceData, this);
                break;
            case AssemblyPieceType.Cannon:
                //newPiece = new Cannon(data);
                break;
            case AssemblyPieceType.Tunnel:
                newPiece = new Tunnel(assemblyPieceData, this);
                break; 
        }
        return newPiece;
    }

    public void PlaceTransportablePiece(ITransportable piece)
    {
        assemblyLineManager.PlaceTransportablePiece(piece);
    }

    public void RemoveTransportablePiece(ITransportable piece)
    {
        assemblyLineManager.RemoveTransportablePiece(piece);
    }

    public void SubscribeToTransportTick(UnityAction action)
    {
        TransportTick.AddListener(action);
    }

    public void UnsubscribeFromTransportTick(UnityAction action)
    {
        TransportTick.RemoveListener(action);
    }
}
