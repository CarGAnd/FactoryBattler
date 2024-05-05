using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GridConnector : NetworkBehaviour
{
    private NetworkBuilder networkBuilder;
    private ulong clientId;
    private Vector2Int position;
    private FixedString128Bytes buildingId;
    private Facing rotation;

    public void Init(NetworkBuilder nBuilder, ulong clientId, Vector2Int position, FixedString128Bytes buildingId, Facing rotation) {
        this.networkBuilder = nBuilder;
        this.clientId = clientId;
        this.position = position;
        this.buildingId = buildingId;
        this.rotation = rotation;
    }

    public override void OnNetworkSpawn() {
        if (IsLocalPlayer) {
            RpcParams sendParams = new RpcParams
            {
                Send = new RpcSendParams
                {
                    Target = RpcTarget.Single(clientId, RpcTargetUse.Temp)
                }
            };
            networkBuilder.ConnectExistingBuildingClientRpc(GetComponent<NetworkObject>(), buildingId, position, rotation, sendParams);
            Debug.Log("Connected");
        }
    }
}
