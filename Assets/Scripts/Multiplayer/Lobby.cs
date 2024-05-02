using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class Lobby
{
    public ulong LobbyOwnerId { get; private set; }
    private List<ClientInfo> connectedClients;
    
    public Lobby() {
        connectedClients = new List<ClientInfo>();
    }

    public void AddPlayer(ClientInfo clientInfo) {
        if(connectedClients.Count == 0) {
            LobbyOwnerId = clientInfo.clientId;
        }
        connectedClients.Add(clientInfo);
        Debug.Log(connectedClients.Count);
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

    public List<ClientInfo> GetConnectedClients() {
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
public class ClientInfo : INetworkSerializable {
    public ulong clientId;
    public FixedString128Bytes clientName;

    public ClientInfo(ulong clientId) {
        this.clientId = clientId;
    }

    public ClientInfo() {

    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref clientId);
        serializer.SerializeValue(ref clientName);
    }

    public override string ToString() {
        return clientId + " : " + clientName;
    }
}
