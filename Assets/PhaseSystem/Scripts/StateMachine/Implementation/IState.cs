using UnityEngine.Events;

namespace PhaseSystem {
    public interface IState
    {
        UnityEvent ExitedState { get; }
        public PhaseTriggerBase EndTrigger { get; }
        bool IsActive { get; }
        void Enter();
        void Update();
        void FixedUpdate();
        void Exit();
    }
}