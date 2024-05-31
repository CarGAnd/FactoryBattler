using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayerReadyVisuals : MonoBehaviour
{
    [SerializeField] private ReadyTracker readyTracker;
    [SerializeField] private GameObject readyStatusPrefab;
    [SerializeField] private Transform readyStatusParent;

    private void Start() {
        NetworkList<PlayerReadyState> readyStateList = readyTracker.GetReadyStateList();
        readyStateList.OnListChanged += OnReadyStateListChanged;
        RefreshReadyVisuals();
    }

    private void OnDisable() {
        NetworkList<PlayerReadyState> readyStateList = readyTracker.GetReadyStateList();
        readyStateList.OnListChanged -= OnReadyStateListChanged;
    }

    private void OnReadyStateListChanged(NetworkListEvent<PlayerReadyState> changeEvent) {
        RefreshReadyVisuals();    
    }

    private void RefreshReadyVisuals() {
        NetworkList<PlayerReadyState> readyStates = readyTracker.GetReadyStateList();
        int currentPlayerCount = readyStatusParent.childCount;
        int playersToSpawn = readyStates.Count - currentPlayerCount;
        int playersToRemove = -playersToSpawn;

        for(int i = 0; i < playersToSpawn; i++) {
            Instantiate(readyStatusPrefab, readyStatusParent);
        }
        for(int i = 0; i < playersToRemove; i++) {
            Destroy(readyStatusParent.GetChild(readyStatusParent.childCount - 1).gameObject);
        }

        for(int i = 0; i < readyStates.Count; i++) {
            PlayerReadyState playerState = readyStates[i];
            GameObject readyStateObject = readyStatusParent.GetChild(i).gameObject;
            Image readyImage = readyStateObject.GetComponent<Image>();
            readyImage.color = playerState.isReadyForCombat ? Color.green : Color.red;
        }

    }
}
