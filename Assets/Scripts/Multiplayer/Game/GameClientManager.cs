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
            players.Add(playerGameInfo);

            RpcParams sendParams = new RpcParams
            {
                Send = new RpcSendParams
                {
                    Target = RpcTarget.Single(nClient.ClientId, RpcTargetUse.Temp)
                }
            };

            AssignGridToPlayerClientRpc(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>(), sendParams);
            AssignGridToPlayer(playerGameInfo.playerGrid.GetComponent<NetworkObject>(), playerGameInfo.playerObject.GetComponent<NetworkObject>(), nClient.ClientId);
            gridCounter += 1;
        }
    }

    [Rpc(SendTo.SpecifiedInParams)]
    private void AssignGridToPlayerClientRpc(NetworkObjectReference gridObject, NetworkObjectReference playerObject, RpcParams rpcParams = default) {
        AssignGridToPlayer(gridObject, playerObject, NetworkManager.LocalClientId);    
    }
    
    private void AssignGridToPlayer(NetworkObjectReference gridObject, NetworkObjectReference playerObject, ulong playerId) {
        gridObject.TryGet(out NetworkObject gridNetworkObject);
        playerObject.TryGet(out NetworkObject playerNetworkObject);
        PlayerGameInfo playerGameInfo = null;
        if (IsServer) {
            playerGameInfo = NetworkManager.ConnectedClients[playerId].PlayerObject.GetComponent<PlayerGameInfo>();
        }
        else {
            playerGameInfo = NetworkManager.LocalClient.PlayerObject.GetComponent<PlayerGameInfo>();
        }
        BasePlayer basePlayer = new BasePlayer(playerGameInfo.clientInfo.playerName.ToString(), new WinLossScoreHolderStrategy(), playerId);
        playerGameInfo.player = basePlayer;
        PlayerController playerController = playerNetworkObject.GetComponent<PlayerController>();
        NetworkPlayerGrid builder = gridNetworkObject.GetComponent<NetworkPlayerGrid>();
        playerController.Owner = playerGameInfo.player;
        playerController.AssignGrid(builder);
    }
}
