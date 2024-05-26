using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkLoadingManager : NetworkBehaviour
{
    private Dictionary<ulong, NetworkLoadingTracker> loadingTrackers;

    public override void OnNetworkSpawn() {
        if (IsServer) {
            loadingTrackers = new Dictionary<ulong, NetworkLoadingTracker>();
            NetworkManager.OnClientConnectedCallback += AddTracker;
            NetworkManager.OnClientDisconnectCallback += RemoveTracker;
        }
    }

    public override void OnNetworkDespawn() {
        if (IsServer) {
            NetworkManager.OnClientConnectedCallback -= AddTracker;
            NetworkManager.OnClientDisconnectCallback -= RemoveTracker;
        }
    }

    private void AddTracker(ulong clientId) {
        NetworkLoadingTracker loadingTracker = NetworkManager.ConnectedClients[clientId].PlayerObject.GetComponent<NetworkLoadingTracker>();
        loadingTrackers.Add(clientId, loadingTracker);
    }

    private void RemoveTracker(ulong clientId) {
        loadingTrackers.Remove(clientId);
    }


}
