using PhaseSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AllPlayersReadyCondition : TriggerCondition
{
    [SerializeField, ReadOnly]
    private string name = "AllPlayersReady";

    public void SetToTrue() {
        IsTrue = true;
    }

    public override void Initialize() {
        base.Initialize();
        TestEvents.testEvent.AddListener(SetToTrue);
    }

    public override void Reset() {
        base.Reset();
        TestEvents.testEvent.RemoveListener(SetToTrue);
    }
}
