using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem {
    public abstract class PhaseState : IState
    {
        [Title("@GetType().Name", Bold = true, HorizontalLine = false)][SerializeField][GUIColor("$testColor")][HideLabel]
        private PhaseTriggerBase endTrigger;

        private bool active = false;
        private bool firstRun = true;

        private UnityEvent exitedState = new UnityEvent();
        public PhaseTriggerBase EndTrigger => endTrigger;
        public UnityEvent ExitedState => exitedState;
        public bool IsActive { get => active; set { active = value; ChangeColor(active); }}

        public virtual void Enter()
        {
            IsActive = true;
            CheckIfFirstRun();
            InitializeEndTrigger();

            // Debug.Log($"Entered {this.GetTypeName()}.");
        }

        private void InitializeEndTrigger()
        {
            if (EndTrigger != null)
            {
                EndTrigger?.InitializeTriggers();
                EndTrigger?.Triggered.AddListener(Exit);
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

        public virtual void Update()
        {
            if (EndTrigger != null) {
                EndTrigger.Update();
            }
        }

        public virtual void FixedUpdate()
        {
            if (EndTrigger != null) {
                EndTrigger.FixedUpdate();
            }
        }

        public virtual void Exit()
        {
            IsActive = false;
            ResetEndTrigger();
            ExitedState?.Invoke();

            // Debug.Log($"Exiting {this.GetTypeName()}.");
        }

        private void ResetEndTrigger()
        {
            if (EndTrigger != null)
            {
                EndTrigger.ResetTriggers();
                EndTrigger.Triggered.RemoveListener(Exit);
            }
        }

        private void ChangeColor(bool isActive) {
            if (isActive)
                testColor = Color.green;
            else
                testColor = new Color(1, 1, 1);
        }

        #region Private Fields
        #pragma warning disable 0414
        private Color testColor = new Color(1, 1, 1);
        #pragma warning restore 0414
        #endregion
    }
}