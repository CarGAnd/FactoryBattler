using PhaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PhaseListener : MonoBehaviour
{
    [SerializeField] private PhaseEvent[] phaseEvents;

    private void Start() {
        OnPhaseChanged(PhaseStateHelper.GetCurrentStateName());
    }

    private void OnEnable() {
        PhaseStateHelper.PhaseStateChangedTo.AddListener(OnPhaseChanged);    
    }

    private void OnDisable() {
        PhaseStateHelper.PhaseStateChangedTo.RemoveListener(OnPhaseChanged);
    }

    private void OnPhaseChanged(string newPhaseName) {
        foreach(PhaseEvent pe in phaseEvents) {
            if(pe.phaseName == newPhaseName) {
                pe.TriggerListenEvent();
            }
        }
    }

    [System.Serializable]
    private class PhaseEvent {

        public string phaseName;
        public UnityEvent onEventTriggered;

        public void TriggerListenEvent() {
            onEventTriggered.Invoke();
        }
    }
}

