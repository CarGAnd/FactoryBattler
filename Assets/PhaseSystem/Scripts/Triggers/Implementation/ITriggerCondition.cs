using UnityEngine.Events;

namespace PhaseSystem {
    public interface ITriggerCondition
    {
        UnityEvent ConditionMet { get; }
        bool IsTrue { get; }
        void FixedUpdate();
        void Update();
        void Reset();
    }
}