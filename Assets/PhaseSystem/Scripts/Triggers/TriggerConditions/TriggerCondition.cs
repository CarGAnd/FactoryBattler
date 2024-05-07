using System;
using Sirenix.OdinInspector;
using UnityEngine.Events;

namespace PhaseSystem {
    public abstract class TriggerCondition : ITriggerCondition
    {
        public UnityEvent ConditionMet => conditionMet;
        public virtual void Initialize() { IsTrue = false; }
        public bool IsTrue { get { return isTrue; } internal set { isTrue = value; if (value) ConditionMet?.Invoke(); } }
        internal bool isTrue = false;
        internal UnityEvent conditionMet = new UnityEvent();
        public virtual void FixedUpdate() {}
        public virtual void Reset() { IsTrue = false; }
        public virtual void Update() {}
    }
}