using PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkBuilder : NetworkBehaviour, IBuilder
{
    [SerializeField] private Builder builder;
    [SerializeField] private BuildingDatabase buildingDatabase;
 
    public override void OnNetworkSpawn() {
        if (IsServer) {
            Debug.Log("reveal in 10");
            StartCoroutine(StartReveal());
        }
    }

    public IEnumerator StartReveal() {
        yield return new WaitForSeconds(10f);
        RevealBoardToAll();
    }

    [Rpc(SendTo.Server)]
    public void TryPlaceBuildingServerRpc(FixedString128Bytes buildingId, Vector2Int pos, Facing rotation, RpcParams rpcParams = default) {
        GridObjectSO building = buildingDatabase.GetBuildingByID(buildingId.ToString());
        IGridObject placedBuilding = builder.TryPlaceBuilding(building, pos, rotation);
        if(placedBuilding != null) {
            GameObject placedObject = placedBuilding.GetGameObject();
            NetworkObject nObject = placedObject.GetComponent<NetworkObject>();
            nObject.SpawnWithObservers = false;
            nObject.Spawn();
            nObject.NetworkShow(rpcParams.Receive.SenderClientId);
            placedObject.GetComponent<GridConnector>().Init(this, pos, buildingId, rotation);
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void ConnectExistingBuildingClientRpc(NetworkObjectReference networkReference, FixedString128Bytes buildingId, Vector2Int pos, Facing rotation, RpcParams rpcParams = default) {
        networkReference.TryGet(out NetworkObject nObject);
        GameObject gObject = nObject.gameObject;
        GridObjectSO buildingData = buildingDatabase.GetBuildingByID(buildingId.ToString());
        if(gObject.TryGetComponent<IGridObject>(out IGridObject gridObject)) {
            builder.ConnectExistingBuildingToGrid(buildingData, gridObject, pos, rotation);
        }
    }

    public void RevealBoard(List<ulong> playerIds) {
        List<IGridObject> placedObjects = builder.GetAllPlacedBuildings();
        foreach(IGridObject gridObject in placedObjects) {
            GameObject gObject = gridObject.GetGameObject();
            NetworkObject nObject = gObject.GetComponent<NetworkObject>();
            foreach(ulong playerId in playerIds) {
                if (!nObject.IsNetworkVisibleTo(playerId)) {
                    nObject.NetworkShow(playerId);
                }
            }
        }
        Debug.Log("Revealed board");
    }

    public void RevealBoardToAll() {
        List<ulong> clientList = new List<ulong>();
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds) {
            clientList.Add(clientId);
        }
        RevealBoard(clientList);
    }

    public void TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation) {
        TryPlaceBuildingServerRpc(buildingData.ID, coord, rotation);
    }

    public Vector2Int GetBuildingPlacementPosition(GridObjectSO buildingData, Vector3 worldPos, Facing rotation) {
        return builder.GetBuildingPlacementPosition(buildingData, worldPos, rotation);
    }
}
