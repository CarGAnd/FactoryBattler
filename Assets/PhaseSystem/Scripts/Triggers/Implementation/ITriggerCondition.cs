using UnityEngine.Events;

namespace PhaseSystem {
    public interface ITriggerCondition
    {
        UnityEvent ConditionMet { get; }
        bool IsTrue { get; }
        void Initialize();
        void FixedUpdate();
        void Update();
        void Reset();
    }
}