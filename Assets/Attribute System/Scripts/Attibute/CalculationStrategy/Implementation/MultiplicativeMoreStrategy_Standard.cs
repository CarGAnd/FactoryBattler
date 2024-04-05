using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "MultiplicativeMoreStrategy", menuName = "CalculationStrategies/MultiplicativeMore")]
    public class MultiplicativeMoreStrategy_Standard : CalculationStrategy
    {
        public override float Calculate(IEnumerable<float> values, float runningTotal) => runningTotal *= values.Aggregate(1f, (acc, val) => acc * (1 + val / 100f));

        public override AttributeCalculationType AttributeCalculationType { get => attributeCalculationType; }

        [SerializeField]
        private AttributeCalculationType attributeCalculationType = AttributeCalculationType.MultiplicativeMore;
    }
}