using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerController : MonoBehaviour
{
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    private int activePlayer = 1;

    private void SetActivePlayer(int playerNumber) {
        player1.SetActive(playerNumber == 1);
        player2.SetActive(playerNumber == 2);
        activePlayer = playerNumber;
    }

    private void Start() {
        SetActivePlayer(1);
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.F1)) {
            SetActivePlayer(1);
        }
        else if (Input.GetKeyDown(KeyCode.F2)) {
            SetActivePlayer(2);
        }
    }

}
