using System;

namespace PhaseSystem {
    [Serializable]
    public abstract class StateMachine : IStateMachine
    {
        internal int stateIndex = 0;
        public virtual IState CurrentState { get; set; }
        public virtual IState[] States { get; protected set; }

        public virtual void ChangeState(IState newState)
        {
            if (CurrentState != null) {
                CurrentState.ExitedState.RemoveListener(ForwardState);
            }

            CurrentState = newState;
            CurrentState.Enter();
            CurrentState.ExitedState.AddListener(ForwardState);
        }

        public virtual void ForwardState()
        {
            if ((stateIndex + 1) >= States.Length)
                stateIndex = 0;
            else
                stateIndex++;

            ChangeState(States[stateIndex]);
        }

        public virtual void FixedUpdate()
        {
            CurrentState?.FixedUpdate();
        }

        public virtual void Update()
        {
            CurrentState?.Update();
        }
    }
}
        