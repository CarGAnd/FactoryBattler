using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using System;

public class Lobby : NetworkBehaviour
{
    public ulong LobbyOwnerId { get; private set; }
    private NetworkList<ClientInfo> connectedClients;

    private void Awake() {
        connectedClients = new NetworkList<ClientInfo>();
    }

    public void AddPlayer(ClientInfo clientInfo) {
        if(connectedClients.Count == 0) {
            LobbyOwnerId = clientInfo.clientId;
        }
        connectedClients.Add(clientInfo);
    }

    public void RemovePlayer(ulong clientId) {
        if(clientId == LobbyOwnerId) {
            if(connectedClients.Count > 0) {
                LobbyOwnerId = connectedClients[0].clientId;
            }
        }
        int playerIndex = GetPlayerIndex(clientId);
        connectedClients.RemoveAt(playerIndex);
    }

    public NetworkList<ClientInfo> GetConnectedClients() {
        return connectedClients;
    }

    public void SetClientInfo(ulong playerId, ClientInfo newClientInfo) {
        int playerIndex = GetPlayerIndex(playerId);
        connectedClients[playerIndex] = newClientInfo;
    }

    private int GetPlayerIndex(ulong playerId) {
        for(int i = 0; i < connectedClients.Count; i++) {
            if(connectedClients[i].clientId == playerId) {
                return i;
            }
        }
        return -1;
    }
}

[System.Serializable]
public struct ClientInfo : INetworkSerializable, IEquatable<ClientInfo> {
    public ulong clientId;
    public FixedString128Bytes clientName; //Normal strings are not supported by unity netcode so we use this instead

    public bool Equals(ClientInfo other) {
        return clientId == other.clientId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref clientName);
    }

    public override string ToString() {
        return clientId + " : " + clientName;
    }
}
