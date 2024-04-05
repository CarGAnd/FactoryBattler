using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem
{
    public abstract class CalculationStrategy : ScriptableObject, ICalculationStrategy
    {
        public abstract float Calculate(IEnumerable<float> values, float runningTotal);
        
        public abstract AttributeCalculationType AttributeCalculationType {get;}
    }
}