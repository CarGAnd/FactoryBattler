using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem
{
    public class MultiPhaseState : StateMachine, IState, IActive
    {
        [Title("@GetType().Name", Bold = true, HorizontalLine = false)]
        [SerializeField]
        private bool repeatStates = false;

        [SerializeReference]
        [ListDrawerSettings(ShowFoldout = false, DefaultExpandedState = true)]
        [HideReferenceObjectPicker]
        [GUIColor("$testColor")]
        [HideLabel]
        [ShowIf("@(this as IActive).CheckIfActive()")]
        internal IState[] states = new IState[0];

        [SerializeField]
        private PhaseTriggerBase endTrigger; 

        private IState currentState;
        private bool active = false;
        private bool firstRun = true;

        public PhaseTriggerBase EndTrigger => endTrigger;
        private UnityEvent exitedState = new UnityEvent();
        public UnityEvent ExitedState => exitedState;
        public bool IsActive { get => active; set { active = value; ChangeColor(active); } }
        public override IState CurrentState { get => currentState; set => currentState = value; }
        public override IState[] States { get => states; protected set => states = value; }

        public void Enter()
        {
            IsActive = true;
            CheckIfFirstRun();
            InitializeEndTrigger();
            ChangeToFirstState();
        }

        public override void Update()
        {
            base.Update();
            if (EndTrigger != null)
                EndTrigger.Update();
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();
            if (EndTrigger != null)
                EndTrigger.FixedUpdate();
        }

        public void Exit()
        {
            IsActive = false;
            ResetStates();
            ResetEndTrigger();

            ExitedState?.Invoke();
        }

        private void ChangeToFirstState()
        {
            if (States != null && States.Length > 0)
            {
                ChangeState(States[stateIndex]);
            }
        }

        private void InitializeEndTrigger()
        {
            if (EndTrigger != null)
            {
                EndTrigger?.InitializeTriggers();
                EndTrigger?.Triggered.AddListener(Exit);
            }
        }

        private void ResetEndTrigger()
        {
            if (EndTrigger != null)
            {
                EndTrigger.ResetTriggers();
                EndTrigger.Triggered.RemoveListener(Exit);
            }
        }

        private void CheckIfFirstRun()
        {
            if (firstRun)
            {
                firstRun = false;
                endTrigger = endTrigger.Clone();
            }
        }

        private void ResetStates()
        {
            if (CurrentState != null)
            {
                CurrentState.ExitedState.RemoveListener(ForwardState);

                if (CurrentState.IsActive)
                    CurrentState.Exit();

                CurrentState = null;
            }

            stateIndex = 0;
        }

        private void ChangeColor(bool isActive)
        {
            if (isActive)
                testColor = Color.green;
            else
                testColor = new Color(1, 1, 1);
        }

        public override void ForwardState()
        {
            if ((stateIndex + 1) >= States.Length)
            {
                if (!repeatStates)
                {
                    Exit();
                    return;
                }

                stateIndex = 0;
            }
            else
                stateIndex++;

            ChangeState(States[stateIndex]);
        }

        #region Private Fields
        #pragma warning disable 0414
        private Color testColor = new Color(1, 1, 1);
        #pragma warning restore 0414
        #endregion
    }
}