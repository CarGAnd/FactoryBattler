using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ReadyTracker : NetworkBehaviour 
{
    private NetworkList<PlayerReadyState> playerReadyStates;

    private void Awake() {
        playerReadyStates = new NetworkList<PlayerReadyState>();
    }

    public NetworkList<PlayerReadyState> GetReadyStateList() {
        return playerReadyStates;
    }

    public override void OnNetworkSpawn() {
        if (!IsServer) {
            return;
        }
        ResetReadyStates();
    }

    [Rpc(SendTo.Server)]
    private void TogglePlayerReadyServerRpc(RpcParams rpcParams = default) {
        ulong playerId = rpcParams.Receive.SenderClientId;
        int playerIndex = GetPlayerIndex(playerId);
        PlayerReadyState newPlayerReadyState = new PlayerReadyState
        {
            playerId = playerId,
            isReadyForCombat = !playerReadyStates[playerIndex].isReadyForCombat
        };
        playerReadyStates[playerIndex] = newPlayerReadyState;
        Debug.Log(String.Format("Player {0} is {1}", playerId, newPlayerReadyState.isReadyForCombat ? "Ready" : "Not ready"));
        if (IsAllPlayersReady()) {
            Debug.Log("All players are ready. Moving to combat phase");
            GoToCombatPhase();
        }
    }

    private void GoToCombatPhase() {
        foreach(PlayerReadyState pReadyState in playerReadyStates) {
            NetworkClient nClient = NetworkManager.ConnectedClients[pReadyState.playerId];
            //TODO: Set this up with events instead of digging for the reference
            NetworkBuilder networkBuilder = nClient.PlayerObject.GetComponent<PlayerGameInfo>().playerGrid.GetComponentInChildren<NetworkBuilder>();
            networkBuilder.RevealBoardToAll();
        }
    }

    private void OnEnterBuildingPhase() {
        ResetReadyStates();
    }

    private bool IsAllPlayersReady() {
        for(int i = 0; i < playerReadyStates.Count; i++) {
            if(!playerReadyStates[i].isReadyForCombat) {
                return false;
            }
        }
        return true;
    }

    private int GetPlayerIndex(ulong playerId) {
        for(int i = 0; i < playerReadyStates.Count; i++) {
            if(playerReadyStates[i].playerId == playerId) {
                return i;
            }
        }
        return -1;
    }

    public void ToggleLocalClientReady() {
        TogglePlayerReadyServerRpc();
    }

    public void ResetReadyStates() {
        foreach(NetworkClient nClient in NetworkManager.ConnectedClientsList) {
            PlayerGameInfo playerGameInfo = nClient.PlayerObject.GetComponent<PlayerGameInfo>();
            if (!playerGameInfo.clientInfo.isSpectator) {
                playerReadyStates.Add(new PlayerReadyState
                {
                    playerId = nClient.ClientId,
                    isReadyForCombat = false
                });
            }
        }
    }
}

public struct PlayerReadyState : INetworkSerializable, IEquatable<PlayerReadyState> {
    public bool isReadyForCombat;
    public ulong playerId;

    public bool Equals(PlayerReadyState other) {
        return isReadyForCombat = other.isReadyForCombat && playerId == other.playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref isReadyForCombat);
        serializer.SerializeValue(ref playerId);
    }
}
