using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LobbyPlayerVisuals : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image readyImage;
    [SerializeField] private Image spectatorImage;

    public void SetName(string newName) {
        nameText.text = newName;
    }

    public void SetReadyState(bool newReadyState) {
        readyImage.color = newReadyState ? Color.green : Color.red;
    }

    public void SetSpectatorStatus(bool newIsSpectator) {
        spectatorImage.gameObject.SetActive(newIsSpectator);
    }

}
