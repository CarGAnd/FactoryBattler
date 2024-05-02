using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameClientManager : NetworkBehaviour
{
    private List<PlayerInfo> players;
    [SerializeField] private GameObject playerPrefab;

    public void Initialize(Lobby playerLobby) {
        DontDestroyOnLoad(this);
        players = new List<PlayerInfo>();
        FactoryGrid[] grids = FindObjectsOfType<FactoryGrid>();
        List<ClientInfo> clients = playerLobby.GetConnectedClients();
        for(int i = 0; i < clients.Count; i++) {
            PlayerInfo pInfo = new PlayerInfo();
            pInfo.clientInfo = clients[i];
            pInfo.playerPrefab = Instantiate(playerPrefab);
            pInfo.playerPrefab.GetComponent<NetworkObject>().SpawnAsPlayerObject(clients[i].clientId, true);
            pInfo.playerGrid = grids[i].transform.root.gameObject;
            players.Add(pInfo);
            AssignGridToPlayerClientRpc(pInfo.playerGrid.GetComponent<NetworkObject>(), pInfo.playerPrefab.GetComponent<NetworkObject>());
            AssignGridToPlayer(pInfo.playerGrid.GetComponent<NetworkObject>(), pInfo.playerPrefab.GetComponent<NetworkObject>());
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
        Builder builder = gridNetworkObject.GetComponentInChildren<Builder>();
        playerModeManager.UpdateGridReference(grid, builder);
    }
}

public class PlayerInfo {
    
    public ClientInfo clientInfo;
    public GameObject playerPrefab;
    public GameObject playerGrid;

    public PlayerInfo() {

    }
}
