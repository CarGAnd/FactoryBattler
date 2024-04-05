using System.Collections.Generic;
using UnityEngine;

namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "AttributeSet", menuName = "Attributes/AttributeSet")]
    public class AttributeSet : ScriptableObject
    {
        public MainAttributeDefinition mainAttribute;
        public List<AttributeDefinition> attributes;
        public CalculationStrategyGroup calculationGroup;
    }
}