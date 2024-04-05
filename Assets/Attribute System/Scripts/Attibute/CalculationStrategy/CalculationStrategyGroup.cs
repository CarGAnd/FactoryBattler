using Sirenix.OdinInspector;
using UnityEngine;
using System.Linq;
using System;

namespace AttributeSystem
{
    [CreateAssetMenu(fileName = "CalculationStrategyGroup", menuName = "CalculationStrategies/StrategyGroup")]
    public class CalculationStrategyGroup : ScriptableObject
    {
        [ListDrawerSettings(ShowFoldout = true)]
        [InlineEditor(InlineEditorModes.FullEditor)]
        [InfoBox("$warningText", InfoMessageType.Error, "showWarning")]
        public CalculationStrategy[] strategies;

        private string warningText = "";
        private bool showWarning = true;

        private void OnValidate()
        {
            warningText = "";
            showWarning = false;
            // Assuming CalculationStrategy has a public CalculationType property or method to get its type
            var allTypes = System.Enum.GetValues(typeof(AttributeCalculationType)).Cast<AttributeCalculationType>();
            foreach (var type in allTypes)
            {
                int count = strategies.Count(s => s.AttributeCalculationType == type);

                if (count != 1)
                {
                    showWarning = true;
                    warningText += ($"This group does not have exactly one strategy of type '{type}'. Found: {count} {System.Environment.NewLine}");
                    // You might handle this situation more robustly, e.g., auto-correcting the array or marking data as invalid
                }
            }
        }
    }
}