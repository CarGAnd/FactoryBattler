using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyClientVisuals : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private GameObject lobbyPlayerParent;
    
    [SerializeField] private Lobby lobby;

    private void Start() {
        startPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        NetworkManager.Singleton.OnClientStopped += OnClientStopped;
        lobby.playerListChanged.AddListener(OnPlayerListChanged);
    }

    private void OnDestroy() {
        if(NetworkManager.Singleton != null) {
            NetworkManager.Singleton.OnConnectionEvent -= OnConnectionEvent;
            NetworkManager.Singleton.OnClientStopped -= OnClientStopped;
        }
        lobby.playerListChanged.RemoveListener(OnPlayerListChanged);
    }

    private void OnClientStopped(bool wasHost) {
        GoToMainMenu();
    }

    private void OnConnectionEvent(NetworkManager nManager, ConnectionEventData connectionData) {
        //UI code should only run on clients
        if (!nManager.IsClient) {
            return;
        }

        //Since the host is both a client and a server, it will get ClientConnected/Disconnected events for every client that connects.
        //We only want to run this code when we are the ones that are connecting/disconnecting, so we check the clientId.
        //We dont want to disable the lobby UI on the host if we are not the ones that disconnected.
        if(connectionData.ClientId != nManager.LocalClientId) {
            return;
        }

        switch (connectionData.EventType) {
            case ConnectionEvent.ClientConnected:
                GoToLobbyView();
                break;
            case ConnectionEvent.ClientDisconnected:
                GoToMainMenu();
                break;
        }
    }

    private void GoToLobbyView() {
        startPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }

    private void GoToMainMenu() {
        startPanel.SetActive(true);
        lobbyPanel.SetActive(false);
    }

    private void OnPlayerListChanged(NetworkListEvent<LobbyPlayerInfo> changeEventInfo) {
        /*switch (changeEventInfo.Type) {
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Add:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Insert:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.RemoveAt:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Remove:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Value:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Clear:
                break;
            case NetworkListEvent<LobbyPlayerInfo>.EventType.Full:
                break;
            default:
                break;
        }*/
        //TODO: Right now we just set all values again when something changes. Ideally we should make more focused changes to the UI depending on the change.
        int currentPlayerCount = lobbyPlayerParent.transform.childCount;
        int playersToSpawn = lobby.GetNumClientsConnected() - currentPlayerCount;
        int playersToRemove = -playersToSpawn;
        for(int i = 0; i < playersToSpawn; i++) {
            Instantiate(lobbyPlayerPrefab, lobbyPlayerParent.transform);
        }
        for(int i = 0; i < playersToRemove; i++) {
            Destroy(lobbyPlayerParent.transform.GetChild(lobbyPlayerParent.transform.childCount - 1).gameObject);
        }
        for(int i = 0; i < lobby.GetNumClientsConnected(); i++) {
            LobbyPlayerInfo playerInfo = lobby.GetPlayerInfo(i);
            GameObject lobbyPlayerObject = lobbyPlayerParent.transform.GetChild(i).gameObject;
            LobbyPlayerVisuals playerVisuals = lobbyPlayerObject.GetComponent<LobbyPlayerVisuals>();
            SetPlayerVisuals(playerVisuals, playerInfo);
        }
    }

    private void SetPlayerVisuals(LobbyPlayerVisuals playerVisuals, LobbyPlayerInfo playerInfo) {
        playerVisuals.SetName(playerInfo.playerName.ToString());
        playerVisuals.SetReadyState(playerInfo.isReady);
        playerVisuals.SetSpectatorStatus(playerInfo.isSpectator);
    }

}
