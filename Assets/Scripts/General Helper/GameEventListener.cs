using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/*using SOS;

public class GameEventListener : MonoBehaviour
{
    [SerializeField] private GameEventAction[] gameEvents;

    private void OnEnable() {
        foreach(GameEventAction g in gameEvents) {
            g.SubEvent();
        }
    }

    private void OnDisable() {
        foreach (GameEventAction g in gameEvents) {
            g.UnsubEvent();
        }
    }

    [System.Serializable]
    private class GameEventAction {
        public GameEvent gameEvent;
        public UnityEvent onEventTriggered;

        private void TriggerListenEvent() {
            onEventTriggered.Invoke();
        }

        public void SubEvent() {
            gameEvent.OnInvoked += TriggerListenEvent;
        }

        public void UnsubEvent() {
            gameEvent.OnInvoked -= TriggerListenEvent;
        }
    }
}*/

