using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;

namespace PhaseSystem {
    public class TimedCondition : TriggerCondition
    {
        [SerializeField][HorizontalGroup]
        private int waitTime;
        [NonSerialized][ProgressBar(0, "waitTime")][ShowInInspector][ReadOnly][HorizontalGroup]
        private float counter;

        public override void Reset()
        {
            base.Reset();
            counter = 0;
        }

        public override void Update()
        {
            base.Update();
            if (counter < waitTime) {
                counter += Time.deltaTime;

                #if UNITY_EDITOR
                    GUIHelper.RequestRepaint();
                #endif
            }
            else
                IsTrue = true;
            
        }
    }
}