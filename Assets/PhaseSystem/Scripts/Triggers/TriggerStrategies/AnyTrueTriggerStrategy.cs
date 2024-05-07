using System;
using System.Collections.Generic;
using System.Linq;

namespace PhaseSystem {
    [Serializable]
    public class AnyTrueStrategy : ITriggerStrategy
    {
        public bool ShouldTrigger(IEnumerable<ITriggerCondition> conditions) =>
            conditions.Any(condition => condition.IsTrue);
    }
}