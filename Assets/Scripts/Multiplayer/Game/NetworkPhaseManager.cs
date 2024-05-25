using PhaseSystem;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class NetworkPhaseManager : NetworkBehaviour
{
    [SerializeField] private PhaseController phaseController;

    public override void OnNetworkSpawn() {
        if (!IsServer) {
            phaseController.enabled = false;
        }
    }

    private void OnEnable() {
        PhaseStateHelper.PhaseStateChangedTo.AddListener(OnPhaseChanged);
    }

    private void OnDisable() {
        PhaseStateHelper.PhaseStateChangedTo.RemoveListener(OnPhaseChanged);
    }

    private void OnPhaseChanged(string newStateName) {
        if (IsServer) {
            SetNewStateClientRpc(newStateName);
        }
    }

    [Rpc(SendTo.NotServer)]
    private void SetNewStateClientRpc(string stateName) {
        PhaseStateHelper.SetCurrentStateName(stateName);
    }

}
