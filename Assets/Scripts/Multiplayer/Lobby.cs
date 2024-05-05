using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;
using UnityEngine.Events;

public class Lobby : NetworkBehaviour
{
    public ulong LobbyOwnerId { get; private set; }
    private int maxPlayerCount = 2;
    private NetworkList<LobbyPlayerInfo> playerSlots;
    
    [HideInInspector] public UnityEvent<NetworkListEvent<LobbyPlayerInfo>> playerListChanged;    

    private void Awake() {
        ResetLobby();
    }

    private void OnPlayerSlotsChanged(NetworkListEvent<LobbyPlayerInfo> changeEvent) {
        playerListChanged.Invoke(changeEvent);
    }

    public void ResetLobby() {
        playerSlots = new NetworkList<LobbyPlayerInfo>();
    }

    public LobbyPlayerInfo GetPlayerInfo(int playerIndex) {
        return playerSlots[playerIndex];
    }

    private void OnEnable() {
        playerSlots.OnListChanged += OnPlayerSlotsChanged;
    }

    private void OnDisable() {
        playerSlots.OnListChanged -= OnPlayerSlotsChanged;
    }

    public void AddPlayer(LobbyPlayerInfo clientInfo) {
        if(playerSlots.Count == 0) {
            LobbyOwnerId = clientInfo.clientId;
        }
        if(playerSlots.Count >= maxPlayerCount) {
            clientInfo.isSpectator = true;
        }
        Debug.Log(clientInfo);
        playerSlots.Add(clientInfo);
    }

    public void RemovePlayer(ulong clientId) {
        int playerIndex = GetPlayerIndex(clientId);
        playerSlots.RemoveAt(playerIndex);

        if(clientId == LobbyOwnerId) {
            if(playerSlots.Count > 0) {
                LobbyOwnerId = playerSlots[0].clientId;
            }
        }
    }

    public NetworkList<LobbyPlayerInfo> GetConnectedClients() {
        return playerSlots;
    }

    public void SetClientInfo(ulong playerId, LobbyPlayerInfo newClientInfo) {
        int playerIndex = GetPlayerIndex(playerId);
        LobbyPlayerInfo playerInfo = playerSlots[playerIndex];
        playerInfo.playerName = newClientInfo.playerName;
        playerInfo.isReady = newClientInfo.isReady;

        //Client is a spectator and wants to be a player
        if(playerInfo.isSpectator && !newClientInfo.isSpectator) {
            if(GetNumPlayers() < maxPlayerCount) {
                playerInfo.isSpectator = newClientInfo.isSpectator;
            }
        }
        //Client is a player and wants to be a spectator
        else if(!playerInfo.isSpectator && newClientInfo.isSpectator) {
            playerInfo.isSpectator = newClientInfo.isSpectator;
        }

        //Trigger a network sync of the new value
        playerSlots[playerIndex] = playerInfo;
    }

    private int GetPlayerIndex(ulong playerId) {
        for(int i = 0; i < playerSlots.Count; i++) {
            if(playerSlots[i].clientId == playerId) {
                return i;
            }
        }
        return -1;
    }

    public LobbyPlayerInfo GetPlayerInfo(ulong playerId) {
        int playerIndex = GetPlayerIndex(playerId);
        return playerSlots[playerIndex];
    }

    public int GetNumPlayers() {
        int playerCount = 0;
        foreach(LobbyPlayerInfo playerInfo in playerSlots) {
            if (!playerInfo.isSpectator) {
                playerCount += 1;
            }
        }
        return playerCount;
    }

    public int GetNumSpectators() {
        int spectatorCount = 0;
        foreach(LobbyPlayerInfo playerInfo in playerSlots) {
            if (playerInfo.isSpectator) {
                spectatorCount += 1;
            }
        }
        return spectatorCount;
    }

    public int GetNumClientsConnected() {
        return GetNumPlayers() + GetNumSpectators();
    }

    public bool AllPlayersAreReady() {
        foreach(LobbyPlayerInfo playerInfo in playerSlots) {
            //We only care about the ready state of players, not spectators
            if(!playerInfo.isSpectator && !playerInfo.isReady) {
                return false;
            }
        }
        return true;
    }
}

[System.Serializable]
public struct LobbyPlayerInfo : INetworkSerializable, IEquatable<LobbyPlayerInfo> {
    public ulong clientId;
    public FixedString128Bytes playerName; //Normal strings are not supported by unity netcode so we use this instead
    public bool isReady;
    public bool isSpectator;

    public bool Equals(LobbyPlayerInfo other) {
        return clientId == other.clientId &&
               playerName == other.playerName &&
               isReady == other.isReady &&
               isSpectator == other.isSpectator;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref playerName);
        serializer.SerializeValue(ref isReady);
        serializer.SerializeValue(ref isSpectator);
    }

    public override string ToString() {
        return String.Format("Id : {0} \n" +
            "name : {1} \n" +
            "isReady : {2} \n" +
            "isSpectator : {3}", clientId, playerName, isReady, isSpectator);
    }
}
