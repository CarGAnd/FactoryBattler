using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class GridConnector : NetworkBehaviour
{
    private NetworkPlayerGrid networkBuilder;
    private Vector2Int position;
    private FixedString128Bytes buildingId;
    private Facing rotation;

    public void Init(NetworkPlayerGrid nBuilder, Vector2Int position, FixedString128Bytes buildingId, Facing rotation) {
        this.networkBuilder = nBuilder;
        this.position = position;
        this.buildingId = buildingId;
        this.rotation = rotation;
    }

    public override void OnNetworkSpawn() {
        if (!IsServer) {
            RequestBuildingInfoServerRpc(GetComponent<NetworkObject>());
        }
    }

    [Rpc(SendTo.Server)]
    private void RequestBuildingInfoServerRpc(NetworkObjectReference nObject, RpcParams rpcParams = default) {
        RpcParams sendParams = new RpcParams
        {
            Send = new RpcSendParams
            {
                Target = RpcTarget.Single(rpcParams.Receive.SenderClientId, RpcTargetUse.Temp)
            }
        };
        networkBuilder.ConnectExistingBuildingClientRpc(nObject, buildingId, position, rotation, sendParams);
    }
}
