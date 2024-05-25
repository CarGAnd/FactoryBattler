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
        GameEvents.AllPlayersReadyEvent.AddListener(SetToTrue);
    }

    public override void Reset() {
        base.Reset();
        GameEvents.AllPlayersReadyEvent.RemoveListener(SetToTrue);
    }
}
