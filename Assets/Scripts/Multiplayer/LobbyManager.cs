using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : NetworkBehaviour
{
    [SerializeField] private Lobby lobby;

    private void Start() {
        DontDestroyOnLoad(this);
    }

    #region Server
    public void StartServer() {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
    }

    private void OnClientConnected(ulong clientId) {
        
        RpcParams clientParams = new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Single(clientId, RpcTargetUse.Temp)
            }
        };

        ClientInfo newClientInfo = new ClientInfo();
        newClientInfo.clientId = clientId;

        //Add new player on the server
        lobby.AddPlayer(newClientInfo);

        //Get and update the information (name etc.) for the new player
        GetPlayerInfoClientRpc(clientParams);

        Debug.Log("Added player with id " + clientId);
    }

    private void OnClientDisconnected(ulong clientId) {
        lobby.RemovePlayer(clientId);
    }

    [Rpc(SendTo.Server)]
    public void SetClientInfoServerRpc(ClientInfo newClientInfo, RpcParams rpcParams = default) {
        ulong sender = rpcParams.Receive.SenderClientId;
        newClientInfo.clientId = sender;
        lobby.SetClientInfo(sender, newClientInfo);
        Debug.Log("changed info for player " + sender);
        Debug.Log(newClientInfo);
    }

    private void OnServerStarted() {
        Debug.Log("Started server on port " + NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Port);
    }

    [Rpc(SendTo.Server)]
    public void StartGameServerRpc(RpcParams serverParams = default) {
        if(serverParams.Receive.SenderClientId != lobby.LobbyOwnerId) {
            Debug.Log("Only the owner can start the game");
            return;
        }

        NetworkManager.SceneManager.OnLoadEventCompleted += OnGameSceneLoadComplete;
        NetworkManager.SceneManager.LoadScene("Combat", LoadSceneMode.Single);
    }

    public void StartGame() {
        StartGameServerRpc();
    }

    private void OnGameSceneLoadComplete(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut) {
        GameClientManager gameClientManager = GetComponent<GameClientManager>();
        gameClientManager.Initialize(lobby);
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnGameSceneLoadComplete;
    }

    #endregion


    #region Client
    [Rpc(SendTo.NotServer, AllowTargetOverride = true)]
    public void GetPlayerInfoClientRpc(RpcParams clientParams = default) {
        ClientInfo clientInfo = new ClientInfo()
        {
            clientName = "Player" + NetworkManager.Singleton.LocalClientId
        };
        SetClientInfoServerRpc(clientInfo);
    }

    [Rpc(SendTo.NotServer)]
    public void SetClientInfoClientRpc(ulong playerId, ClientInfo clientInfo) {
        clientInfo.clientId = playerId;
        lobby.SetClientInfo(playerId, clientInfo);
    }
    #endregion

}
