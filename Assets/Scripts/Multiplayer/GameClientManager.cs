using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameClientManager : NetworkBehaviour
{
    private List<PlayerGameInfo> players;
    [SerializeField] private GameObject playerPrefab;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            NetworkManager.SceneManager.OnLoadEventCompleted += InitOnLoadComplete;
        }
    }

    public override void OnNetworkDespawn() {
        if (IsServer) {
            NetworkManager.SceneManager.OnLoadEventCompleted -= InitOnLoadComplete;
        }
    }

    private void InitOnLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        Initialize();
    }

    public void Initialize() {
        players = new List<PlayerGameInfo>();
        FactoryGrid[] grids = FindObjectsOfType<FactoryGrid>();
        int gridCounter = 0;
        foreach(NetworkClient nClient in NetworkManager.ConnectedClientsList) {
            PlayerGameInfo playerGameInfo = nClient.PlayerObject.GetComponent<PlayerGameInfo>();
            //Spectators do not get a grid assigned
            if (playerGameInfo.clientInfo.isSpectator) {
                continue;
            }
            playerGameInfo.playerObject = Instantiate(playerPrefab);
            playerGameInfo.playerObject.GetComponent<NetworkObject>().SpawnWithOwnership(nClient.ClientId);
            playerGameInfo.playerGrid = grids[gridCounter].transform.root.gameObject;
            players.Add(playerGameInfo);
            AssignGridToPlayerClientRpc(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>());
            AssignGridToPlayer(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>());
            gridCounter += 1;
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
