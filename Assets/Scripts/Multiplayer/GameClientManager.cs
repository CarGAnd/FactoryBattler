using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameClientManager : NetworkBehaviour
{
    private List<PlayerGameInfo> players;
    [SerializeField] private GameObject playerPrefab;

    public void Initialize(Lobby playerLobby) {
        DontDestroyOnLoad(this);
        players = new List<PlayerGameInfo>();
        FactoryGrid[] grids = FindObjectsOfType<FactoryGrid>();
        NetworkList<LobbyPlayerInfo> clients = playerLobby.GetConnectedClients();
        for(int i = 0; i < clients.Count; i++) {
            if (clients[i].isSpectator) {
                continue;
            }
            PlayerGameInfo pInfo = NetworkManager.ConnectedClients[clients[i].clientId].PlayerObject.gameObject.AddComponent<PlayerGameInfo>();
            pInfo.clientInfo = clients[i];
            pInfo.playerObject = Instantiate(playerPrefab);
            pInfo.playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(clients[i].clientId);
            pInfo.playerGrid = grids[i].transform.root.gameObject;
            players.Add(pInfo);
            AssignGridToPlayerClientRpc(pInfo.playerGrid.GetComponent<NetworkObject>(), pInfo.playerObject.GetComponent<NetworkObject>());
            AssignGridToPlayer(pInfo.playerGrid.GetComponent<NetworkObject>(), pInfo.playerObject.GetComponent<NetworkObject>());
        }
    }

    [Rpc(SendTo.NotServer)]
    private void AssignGridToPlayerClientRpc(NetworkObjectReference gridObject, NetworkObjectReference playerObject) {
        AssignGridToPlayer(gridObject, playerObject);    
    }
    
    private void AssignGridToPlayer(NetworkObjectReference gridObject, NetworkObjectReference playerObject) {
        gridObject.TryGet(out NetworkObject gridNetworkObject);
        playerObject.TryGet(out NetworkObject playerNetworkObject);
        PlayerModeManager playerModeManager = playerNetworkObject.GetComponent<PlayerModeManager>();
        FactoryGrid grid = gridNetworkObject.GetComponentInChildren<FactoryGrid>();
        NetworkBuilder builder = gridNetworkObject.GetComponentInChildren<NetworkBuilder>();
        playerModeManager.UpdateGridReference(grid, builder);
    }
}
