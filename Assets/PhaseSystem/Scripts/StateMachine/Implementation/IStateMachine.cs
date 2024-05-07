using System.Collections.Generic;

namespace PhaseSystem {
    public interface IStateMachine
    {
        IState CurrentState { get; set; }
        IState[] States { get; }
        void ChangeState(IState newState);
        void Update();
        void FixedUpdate();
        void ForwardState();
    }
}