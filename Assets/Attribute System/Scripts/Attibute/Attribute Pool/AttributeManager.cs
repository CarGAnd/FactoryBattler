/// <summary>
/// The AttributeManager class is responsible for managing attributes in the AttributeSystem.
/// It keeps track of the base attributes, pool attributes, calculated attributes, and dirty attributes.
/// The AttributeManager allows for adding and removing pool attributes, calculating attribute values, and marking attributes as dirty.
/// </summary>
/// <remarks>
/// The AttributeManager class has the following key features:
/// - SetBaseAttributes: Sets the base attributes for the AttributeManager.
/// - AddPoolAttributes: Adds pool attributes to the AttributeManager.
/// - RemovePoolAttributes: Removes pool attributes from the AttributeManager.
/// - GetCalculatedAttributeValue: Gets calculated value of a specific attribute.
/// - MarkAttributeAsDirty: Marks an attribute as dirty, indicating that it needs to be recalculated.
/// </remarks>
/// <example>
/// The following example demonstrates how to use the AttributeManager class:
/// <code>
/// // Create an instance of the AttributeManager
/// AttributeManager attributeManager = new AttributeManager();
///
/// // Set the base attributes
/// attributeManager.SetBaseAttributes(baseAttributes);
///
/// // Add pool attributes
/// attributeManager.AddPoolAttributes(poolAttributes);
///
/// // Get the value of a specific attribute
/// float attributeValue = attributeManager.GetCalculatedAttributeValue("attributeName");
///
/// // Mark an attribute as dirty
/// attributeManager.MarkAttributeAsDirty("attributeName");
/// </code>
/// </example>
/// <seealso cref="AttributeInstance"/>
/// <seealso cref="AttributeCalculationSystem"/>
/// <seealso cref="AttributeSetMaster"/>

using System.Collections.Generic;

namespace AttributeSystem
{
    public class AttributeManager
    {
        private HashSet<AttributeInstance> inherientAttributes;
        private HashSet<AttributeInstance> combinedAttributes = new HashSet<AttributeInstance>();
        private Dictionary<string, AttributeInstance> poolAttributes = new Dictionary<string, AttributeInstance>();
        private Dictionary<string, float> calculatedAttributes = new Dictionary<string, float>();
        private HashSet<string> dirtyAttributes = new HashSet<string>();

        internal void SetBaseAttributes(IReadOnlyList<AttributeInstance> baseAttributes)
        {
            if (baseAttributes == null)
                return;

            inherientAttributes = new HashSet<AttributeInstance>(baseAttributes);
        }

        public void AddPoolAttributes(HashSet<AttributeInstance> updatedPoolAttributes)
        {
            foreach (var updatedAttribute in updatedPoolAttributes)
            {
                poolAttributes[updatedAttribute.Id] = updatedAttribute;
                UnityEngine.Debug.Log($"{updatedAttribute.definition.name} with {updatedAttribute.value} was added to the AttributeManager.");
            }

            MarkAttributesAsDirty(updatedPoolAttributes);
        }

        public void RemovePoolAttributes(HashSet<AttributeInstance> updatedPoolAttributes)
        {
            foreach (var updatedAttribute in updatedPoolAttributes)
            {
                poolAttributes.Remove(updatedAttribute.Id);
                // UnityEngine.Debug.Log($"{updatedAttribute.definition.name} with {updatedAttribute.value} was removed to the AttributeManager.");
            }

            MarkAttributesAsDirty(updatedPoolAttributes);
        }

        private void MarkAttributesAsDirty(HashSet<AttributeInstance> updatedPoolAttributes)
        {
            foreach (string mainAttributeName in AttributeSetMaster.FindMainAttributeNamesForRelevantSets(updatedPoolAttributes))
            {
                MarkAttributeAsDirty(mainAttributeName);
            }
        }

        public float GetCalculatedAttributeValue(string mainAttributeName)
        {
            if (calculatedAttributes.TryGetValue(mainAttributeName, out float cachedValue))
            {
                // UnityEngine.Debug.Log($"Cached value for {mainAttributeName} is {cachedValue}.");
                return cachedValue;
            }

            combinedAttributes = new HashSet<AttributeInstance>(inherientAttributes);

            combinedAttributes.UnionWith(poolAttributes.Values);

            float setValue = AttributeCalculationSystem.CalculateAttribute(mainAttributeName, combinedAttributes);
            calculatedAttributes[mainAttributeName] = setValue;

            if (dirtyAttributes.Contains(mainAttributeName))
            {
                // UnityEngine.Debug.Log($"{mainAttributeName} is no longer marked dirty.");
                dirtyAttributes.Remove(mainAttributeName);
            }

            return setValue;
        }

        public void MarkAttributeAsDirty(string attributeName)
        {
            if (!dirtyAttributes.Contains(attributeName))
            {
                // UnityEngine.Debug.Log($"{attributeName} was marked dirty.");
                dirtyAttributes.Add(attributeName);
                if (calculatedAttributes.ContainsKey(attributeName))
                    calculatedAttributes.Remove(attributeName);
            }
        }
    }
}