using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace PhaseSystem
{
    [Serializable]
    [InlineEditor(Expanded = true)]
    [CreateAssetMenu(fileName = "New Phase Trigger", menuName = "PhaseSystem/Triggers/PhaseTrigger")]
    public class PhaseTrigger : PhaseTriggerBase, IActive
    {
        public bool IsActive { get => isActive; set => isActive = value; }

        [NonSerialized]
        private bool isActive = false;

        public override void InitializeTriggers()
        {
            if (triggerConditions == null)
                return;

            isActive = true;
            ResetAndSubscribeToTriggers();

            // Debug.Log($"Triggers were initialized for {this.name}.");
        }

        private void ResetAndSubscribeToTriggers()
        {
            foreach (ITriggerCondition condition in triggerConditions)
            {
                condition.Initialize();
                condition.ConditionMet.AddListener(OnTriggered);
            }
        }

        public override void ResetTriggers()
        {
            if (triggerConditions == null)
                return;

            isActive = false;
            ResetAndUnsubscribeToTriggers();

            // Debug.Log($"Triggers were reset for {this.name}.");
        }

        private void ResetAndUnsubscribeToTriggers()
        {
            foreach (ITriggerCondition condition in triggerConditions)
            {
                condition.Reset();
                condition.ConditionMet.RemoveListener(OnTriggered);
            }
        }
    }
}