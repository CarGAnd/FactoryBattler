using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyCreation : MonoBehaviour
{
    [SerializeField] private LobbyManager serverLobby;

    public void StartHost() {
        serverLobby.StartServer();
        NetworkManager.Singleton.StartHost();
    }

    public void StartClient() {
        NetworkManager.Singleton.StartClient();
    }

    public void StartServer() {
        serverLobby.StartServer();
        NetworkManager.Singleton.StartServer();
    }
}
