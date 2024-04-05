using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;


namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "SumStrategy", menuName = "CalculationStrategies/Sum")]
    public class SumStrategy_Standard : CalculationStrategy
    {
        public override float Calculate(IEnumerable<float> values, float runningTotal) => runningTotal += values.Sum();
        
        public override AttributeCalculationType AttributeCalculationType { get => attributeCalculationType; }

        [SerializeField][EnumToggleButtons]
        private AttributeCalculationType attributeCalculationType = AttributeCalculationType.Additive;
    }
}
