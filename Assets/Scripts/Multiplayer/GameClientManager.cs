using PlayerSystem;
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
            playerGameInfo.player = new BasePlayer(playerGameInfo.clientInfo.playerName.ToString(), new WinLossScoreHolderStrategy(), nClient.ClientId);
            players.Add(playerGameInfo);
            AssignGridToPlayerClientRpc(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>(), nClient.ClientId);
            AssignGridToPlayer(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>(), nClient.ClientId);
            gridCounter += 1;
        }
    }

    [Rpc(SendTo.NotServer)]
    private void AssignGridToPlayerClientRpc(NetworkObjectReference gridObject, NetworkObjectReference playerObject, ulong playerId) {
        AssignGridToPlayer(gridObject, playerObject, playerId);    
    }
    
    private void AssignGridToPlayer(NetworkObjectReference gridObject, NetworkObjectReference playerObject, ulong playerId) {
        gridObject.TryGet(out NetworkObject gridNetworkObject);
        playerObject.TryGet(out NetworkObject playerNetworkObject);
        PlayerModeManager playerModeManager = playerNetworkObject.GetComponent<PlayerModeManager>();
        FactoryGrid grid = gridNetworkObject.GetComponentInChildren<FactoryGrid>();
        NetworkBuilder builder = gridNetworkObject.GetComponentInChildren<NetworkBuilder>();
        IPlayer player = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<PlayerGameInfo>().player;
        gridNetworkObject.GetComponentInChildren<Builder>().Owner = player;
        playerModeManager.Owner = player;
        playerModeManager.UpdateGridReference(grid, builder);
    }
}
