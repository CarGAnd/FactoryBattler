using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace PhaseSystem {
    public abstract class PhaseTriggerBase : ScriptableObject
    {
        [SerializeReference]
        [HideReferenceObjectPicker]
        [ListDrawerSettings(ShowFoldout = false)]
        [ShowIf("@(this as IActive).CheckIfActive()")]
        protected List<ITriggerCondition> triggerConditions;

        [SerializeReference]
        private ITriggerStrategy triggerStrategy = new AllTrueTriggerStrategy();

        internal UnityEvent Triggered;
        public virtual void OnTriggered() 
        {
            if (triggerStrategy.ShouldTrigger(triggerConditions))
            {
                Triggered?.Invoke();
                // Debug.Log($"All conditions for {this.name} were triggered.");
            }
        }

        public virtual void Update() 
        {
            if (triggerConditions == null)
                return;

            foreach (ITriggerCondition condition in triggerConditions)
            {
                condition.Update();
            }
        }

        public virtual void FixedUpdate()
        {
            if (triggerConditions == null)
                return;

            foreach (ITriggerCondition condition in triggerConditions)
            {
                condition.FixedUpdate();
            }
        }

        public virtual PhaseTriggerBase Clone() {
            PhaseTriggerBase clonedTrigger = Instantiate(this);
            clonedTrigger.Triggered = new UnityEvent();
            return clonedTrigger;
        }

        public abstract void InitializeTriggers();
        public abstract void ResetTriggers();
    }
}