/// <summary>
/// Represents an instance of an attribute.
/// </summary>
/// <remarks>
/// An attribute instance contains the unique identifier, the attribute definition, and the attribute value.
/// </remarks>
/// <example>
/// <code>
/// AttributeInstance instance = new AttributeInstance();
/// instance.definition = new AttributeDefinition("Health", AttributeType.Float);
/// instance.value = 100f;
/// </code>
/// </example>
/// <seealso cref="AttributeDefinition"/>
/// <seealso cref="AttributeType"/>
/// <seealso cref="AttributePool"/>
/// <seealso cref="AttributeManager"/>

using System;
using Sirenix.OdinInspector;

namespace AttributeSystem
{
    [Serializable]
    public class AttributeInstance
    {
        internal string Id;

        [HideLabel][PropertyOrder(1)]
        public AttributeDefinition definition;

        [HideLabel][PropertyOrder(0)][TableColumnWidth(-300)]
        public float value;

        public AttributeInstance() {
            CalculateNewID();
        }

        public AttributeInstance(AttributeDefinition definition, float value)
        {
            CalculateNewID();
            this.definition = definition;
            this.value = value;
        }

        public void CalculateNewID() {
            Id = Guid.NewGuid().ToString();
        }
    }
}