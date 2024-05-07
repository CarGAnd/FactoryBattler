using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PhaseSystem {
    [Serializable]
    public class CountTrueTriggerStrategy : ITriggerStrategy
    {
        [SerializeField]
        private int requiredTrueCount;

        public CountTrueTriggerStrategy()
        {
            requiredTrueCount = 1;
        }

        public CountTrueTriggerStrategy(int count)
        {
            requiredTrueCount = count;
        }

        public bool ShouldTrigger(IEnumerable<ITriggerCondition> conditions) =>
            conditions.Count(condition => condition.IsTrue) >= requiredTrueCount;
    }
}