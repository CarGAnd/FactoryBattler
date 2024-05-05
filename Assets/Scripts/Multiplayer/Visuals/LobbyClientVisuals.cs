using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class LobbyClientVisuals : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject lobbyPlayerPrefab;
    [SerializeField] private GameObject lobbyPlayerList;
    
    [SerializeField] private Lobby lobby;

    private void Start() {
        startPanel.SetActive(true);
        lobbyPanel.SetActive(false);
        NetworkManager.Singleton.OnConnectionEvent += OnConnectionEvent;
        lobby.playerListChanged.AddListener(OnPlayerListChanged);
    }

    private void OnDestroy() {
        lobby.playerListChanged.RemoveListener(OnPlayerListChanged);
    }

    private void OnConnectionEvent(NetworkManager nManager, ConnectionEventData connectionData) {
        if(connectionData.EventType == ConnectionEvent.ClientConnected) {
            startPanel.SetActive(false);
            lobbyPanel.SetActive(true);
        }
    }

    private void OnPlayerListChanged(NetworkListEvent<LobbyPlayerInfo> changeEventInfo) {
        switch (changeEventInfo.Type) {
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
        }
        //TODO: Right now we just set all values again when something changes. Ideally we should make more focused changes to the UI depending on the change.
        int currentPlayerCount = lobbyPlayerList.transform.childCount;
        while(currentPlayerCount < lobby.GetNumClientsConnected()) {
            Instantiate(lobbyPlayerPrefab, lobbyPlayerList.transform);
            currentPlayerCount = lobbyPlayerList.transform.childCount;
        }
        while(currentPlayerCount > lobbyPlayerList.transform.childCount) {
            Destroy(lobbyPlayerList.transform.GetChild(lobbyPlayerList.transform.childCount - 1));
            currentPlayerCount = lobbyPlayerList.transform.childCount;
        }
        for(int i = 0; i < currentPlayerCount; i++) {
            LobbyPlayerInfo playerInfo = lobby.GetPlayerInfo(i);
            GameObject lobbyPlayerObject = lobbyPlayerList.transform.GetChild(i).gameObject;
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
