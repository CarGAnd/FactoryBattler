using PlayerSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class NetworkPlayerGrid : NetworkBehaviour, IPlayerGrid
{
    [SerializeField] private PlayerGrid playerGrid;
    [SerializeField] private BuildingDatabase buildingDatabase;

    public IPlayer Owner { get => playerGrid.Owner; set => playerGrid.Owner = value; }
    public FactoryGrid FactoryGrid { get => playerGrid.FactoryGrid; }

    [Rpc(SendTo.Server)]
    private void TryPlaceBuildingServerRpc(FixedString128Bytes buildingId, Vector2Int pos, Facing rotation, RpcParams rpcParams = default) {
        if(!SenderIsOwner(rpcParams)) {
            Debug.LogError("Only the owner of a grid can place objects on it");
            return;
        }
        GridObjectSO building = buildingDatabase.GetBuildingByID(buildingId.ToString());
        IGridObject placedBuilding = playerGrid.TryPlaceBuilding(building, pos, rotation);
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
            playerGrid.ConnectExistingBuildingToGrid(buildingData, gridObject, pos, rotation);
        }
    }

    public void RevealBoard(List<ulong> playerIds) {
        List<IGridObject> placedObjects = playerGrid.GetAllPlacedBuildings();
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

    public void HideBoard(List<ulong> playerIds) {
        List<IGridObject> placedObjects = playerGrid.GetAllPlacedBuildings();
        foreach(IGridObject gridObject in placedObjects) {
            GameObject gObject = gridObject.GetGameObject();
            NetworkObject nObject = gObject.GetComponent<NetworkObject>();
            foreach(ulong playerId in playerIds) {
                if (nObject.IsNetworkVisibleTo(playerId)) {
                    nObject.NetworkHide(playerId);
                }
            }
        }
        Debug.Log("Hid board");
    }
    
    [Rpc(SendTo.Server)]
    private void RemoveBuildingServerRpc(Vector2Int position, RpcParams rpcParams = default) {
        if (!SenderIsOwner(rpcParams)) {
            return;
        }
        IGridObject gridObject = playerGrid.GetBuildingAtPosition(position);
        if(gridObject != null) {
            GameObject g = gridObject.GetGameObject();
            g.GetComponent<NetworkObject>().Despawn();
            gridObject.DestroyObject();
        }
    }

    private bool SenderIsOwner(RpcParams rpcParams) {
        return rpcParams.Receive.SenderClientId.ToString() == Owner.Id;
    }

    public void RevealBoardToAll() {
        if (!IsServer) {
            return;
        }
        List<ulong> clientList = new List<ulong>();
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds) {
            clientList.Add(clientId);
        }
        RevealBoard(clientList);
    }

    public void HideBoardFromAllExceptOwner() {
        if (!IsServer || Owner == null) {
            return;
        }
        List<ulong> clientList = new List<ulong>();
        foreach(ulong clientId in NetworkManager.ConnectedClientsIds) {
            //Dont hide the board from the owner
            if(Owner.Id == clientId.ToString()) {
                continue;
            }
            clientList.Add(clientId);
        }
        HideBoard(clientList);
    }

    public void TryPlaceBuilding(GridObjectSO buildingData, Vector2Int coord, Facing rotation) {
        TryPlaceBuildingServerRpc(buildingData.ID, coord, rotation);
    }

    public void RemoveBuilding(Vector2Int position) {
        RemoveBuildingServerRpc(position);
    }
}
