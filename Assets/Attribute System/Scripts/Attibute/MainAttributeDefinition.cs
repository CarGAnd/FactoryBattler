using Sirenix.OdinInspector;
using UnityEngine;

namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "MainAttributeDefinition", menuName = "Attributes/MainAttributeDefinition")]
    public class MainAttributeDefinition : AttributeDefinition
    {
        [SerializeReference]
        public IRoundingStrategy roundingStrategy = new RoundUpStrategy();

        public void SetRoundingStrategy(IRoundingStrategy newStrategy)
        {
            roundingStrategy = newStrategy; 
        }

        public float ApplyRounding(float value)
        {
            if (roundingStrategy != null)
            {
                return roundingStrategy.Round(value);
            }
            return value; 
        }
    }
}