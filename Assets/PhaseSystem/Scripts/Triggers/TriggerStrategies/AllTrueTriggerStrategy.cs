using System;
using System.Collections.Generic;
using System.Linq;

namespace PhaseSystem {
    [Serializable]
    public class AllTrueTriggerStrategy : ITriggerStrategy
    {
        public bool ShouldTrigger(IEnumerable<ITriggerCondition> conditions) =>
            conditions.All(condition => condition.IsTrue);
    }
}