using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "MultiplicativeStrategy", menuName = "CalculationStrategies/Multiplicative")]
    public class MultiplicativeStrategy_Standard : CalculationStrategy
    {
        public override float Calculate(IEnumerable<float> values, float runningTotal) => runningTotal *= 1 + (0.01f * values.Sum());

        public override AttributeCalculationType AttributeCalculationType { get => attributeCalculationType; }

        [SerializeField]
        private AttributeCalculationType attributeCalculationType = AttributeCalculationType.Multiplicative;
    }
}