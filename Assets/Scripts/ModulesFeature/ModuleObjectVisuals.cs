using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleObjectVisuals : MonoBehaviour
{
    [SerializeField] private ModuleInputOutput inputOutput;

    private void OnEnable() {
        inputOutput.receivedObject.AddListener(OnReceivedObject);
        inputOutput.sentObject.AddListener(OnSentObject);
    }

    private void OnDisable() {
        inputOutput.receivedObject.RemoveListener(OnReceivedObject);
        inputOutput.sentObject.RemoveListener(OnSentObject);
    }

    private void OnReceivedObject(AssemblyTravelingObject newObj) {
        newObj.gameObject.SetActive(false);
    }

    private void OnSentObject(AssemblyTravelingObject sentObj, Vector2Int from, Vector2Int to) {
        sentObj.gameObject.SetActive(true);
        sentObj.MoveToPiece(from, to, inputOutput.Grid);
    }

}
