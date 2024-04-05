using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace AttributeSystem
{
    [Serializable]
    [CreateAssetMenu(fileName = "CreateAttributeWindow", menuName = "Attributes/Windows/CreateAttributeWindow", order = 1)]
    public class CreateAttributesWindow : ScriptableObject
    {
        [SerializeField]
        [PropertyOrder(100)]
        string attributeBasePath = "Assets/Attribute System/ScriptableObjects/Attributes/";

        [SerializeField]
        [PropertyOrder(101)]
        string setBasePath = "Assets/Attribute System/ScriptableObjects/AttributeSets/";

        [SerializeField]
        [LabelText("Name of new Attribute")]
        [OnInspectorInit("OnInspectorInit")] 
        [PropertyOrder(-1)]
        private string attributeName;
        [SerializeField]
        [ShowIf("createAndAddToSet")]
        private CalculationStrategyGroup calculationGroup;
        [SerializeField]
        [HideInInspector]
        private bool createMainAttribute = true;
        [SerializeField]
        [HideInInspector]
        private bool createAdditiveAttribute = false;
        [SerializeField]
        [HideInInspector]
        private bool createMultiplicativeAttribute = false;
        [SerializeField]
        [HideInInspector]
        private bool createMultiplicativeMoreAttribute = false;
        [SerializeField]
        [HideInInspector]
        private bool createAndAddToSet = false;
        [Button("Create Attribute(s)", ButtonSizes.Large)]
        [GUIColor("#26FFB9")]
        [EnableIf("CheckCreationCriteria")]
        [PropertyOrder(0)]
        private void CreateAttributes () {
            List<AttributeDefinition> createdAttributes = new List<AttributeDefinition>();
            AttributeDefinition mainAttributeDefinition = null;
            
            if (createMainAttribute) {
                mainAttributeDefinition = CreateAttribute(attributeName, "", AttributeCalculationType.Base);
            }

            if (createAdditiveAttribute) {
                AttributeDefinition newAttribute = CreateAttribute(attributeName, "Added", AttributeCalculationType.Additive);

                if (newAttribute != null)
                    createdAttributes.Add(newAttribute);
            }
            
            if (createMultiplicativeAttribute) {
                AttributeDefinition newAttribute = CreateAttribute(attributeName, "Multiplied", AttributeCalculationType.Multiplicative);

                if (newAttribute != null)
                    createdAttributes.Add(newAttribute);
            }
            
            if (createMultiplicativeMoreAttribute) {
                AttributeDefinition newAttribute = CreateAttribute(attributeName, "MultipliedMore", AttributeCalculationType.MultiplicativeMore);

                if (newAttribute != null)
                    createdAttributes.Add(newAttribute);
            }

            if (createdAttributes.Any() && createAndAddToSet) {
                CreateNewSet(createdAttributes, mainAttributeDefinition, attributeName);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        [ResponsiveButtonGroup, GUIColor("$mainColorInvoking")]
        private void MainAttribute() {
            createMainAttribute = !createMainAttribute;
            ToggleColor(ref mainColorInvoking, createMainAttribute);
        }

        [ResponsiveButtonGroup, GUIColor("$additiveColorInvoking")]
        private void AdditiveAttribute() {
            createAdditiveAttribute = !createAdditiveAttribute;
            ToggleColor(ref additiveColorInvoking, createAdditiveAttribute);
        }

        [ResponsiveButtonGroup, GUIColor("$multiColorInvoking")]
        private void MultiAttribute() {
            createMultiplicativeAttribute = !createMultiplicativeAttribute;
            ToggleColor(ref multiColorInvoking, createMultiplicativeAttribute);
        }

        [ResponsiveButtonGroup, GUIColor("$multiMoreColorInvoking")]
        private void MultiMoreAttribute() {
            createMultiplicativeMoreAttribute = !createMultiplicativeMoreAttribute;
            ToggleColor(ref multiMoreColorInvoking, createMultiplicativeMoreAttribute);
        }

        [ResponsiveButtonGroup("Set"), GUIColor("$setColorInvoking")]
        private void Set() {
            createAndAddToSet = !createAndAddToSet;
            ToggleColor(ref setColorInvoking, createAndAddToSet);
        }

        private void CreateNewSet(List<AttributeDefinition> createdAttributes, AttributeDefinition mainAttribute, string baseName)
        {
            string path = Path.Combine(setBasePath + baseName + "Set.asset");

            if (File.Exists(path)) {
                Debug.LogWarning($"Set '{path}' already exists.");
                return;
            }

            AttributeSet newSet = CreateInstance<AttributeSet>();

            if (calculationGroup != null)
                newSet.calculationGroup = calculationGroup;

            if (mainAttribute != null)
                newSet.mainAttribute = mainAttribute as MainAttributeDefinition;
            
            newSet.attributes = createdAttributes.ToList();

            AssetDatabase.CreateAsset(newSet, path);
        }

        private AttributeDefinition CreateAttribute(string baseName, string addedName, AttributeCalculationType type) {
            AttributeDefinition attribute;
            string path = attributeBasePath;

            if (type == AttributeCalculationType.Base) {
                path = Path.Combine(path, "Main/");
                attribute = CreateInstance<MainAttributeDefinition>();
            } else {
                path = Path.Combine(path, "Enhance/");
                attribute = CreateInstance<EnhanceAttributeDefinition>();
            }

            path = Path.Combine(path, baseName + addedName + ".asset");

            if (File.Exists(path)) {
                Debug.LogWarning($"Attribute '{path}' already exists.");
                return null;
            }
                

            attribute.calculationType = type;

            AssetDatabase.CreateAsset(attribute, path);

            return attribute;
        }

        private bool CheckCreationCriteria() {
            if (string.IsNullOrEmpty(attributeName))
                return false;

            if (attributeName.Any(char.IsWhiteSpace))
                return false;

            if (createMainAttribute || createAdditiveAttribute || createMultiplicativeAttribute || createMultiplicativeMoreAttribute)
                return true;

            return false;
        }

        private void ToggleColor(ref Color colorToToggle, bool state) {
            if (state)
                colorToToggle = new Color(0, 0.8f, 0);
            else
                colorToToggle = new Color32(243, 109, 134, 255);
        }

        private void OnInspectorInit() {
            ToggleColor(ref mainColorInvoking, createMainAttribute);
            ToggleColor(ref additiveColorInvoking, createAdditiveAttribute);
            ToggleColor(ref multiColorInvoking, createMultiplicativeAttribute);
            ToggleColor(ref multiMoreColorInvoking, createMultiplicativeMoreAttribute);
            ToggleColor(ref setColorInvoking, createAndAddToSet);
        }

        #region Private Fields

        #pragma warning disable 0414
        private Color mainColorInvoking, additiveColorInvoking, multiColorInvoking, multiMoreColorInvoking, setColorInvoking = new Color(0, 0.8f, 0);
        #pragma warning restore 0414

        #endregion
    }
}