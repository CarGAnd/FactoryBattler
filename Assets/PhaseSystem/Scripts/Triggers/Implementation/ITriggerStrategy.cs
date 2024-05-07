using System.Collections.Generic;

namespace PhaseSystem {
    public interface ITriggerStrategy
    {
        bool ShouldTrigger(IEnumerable<ITriggerCondition> conditions);
    }
}