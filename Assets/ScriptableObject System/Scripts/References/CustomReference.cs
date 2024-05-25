using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;

namespace SOS {
    [Serializable]
    public abstract class CustomReference<A, B> : GenericReference<A, B> where A : GenericScriptableObject<B>
    {
        [HideLabel]
        [HideReferenceObjectPicker]
        [HideDuplicateReferenceBox]
        [SerializeReference]
        [HorizontalGroup("row", Width = 0.45f)]
        [ShowIf("useLocalConstant")]
        private B customLocalConstantValue; 

        [HideReferenceObjectPicker]
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.45f)]
        [ShowIf("@variable && referenceType == ReferenceType.Dynamic && !useLocalConstant")]
        [HideLabel]
        protected override B dynamicValue {
            get {
                if (variable != null && referenceType == ReferenceType.Dynamic) {
                    return variable.DynamicValue;
                }

                return customLocalConstantValue;
            }
        }

        [HideReferenceObjectPicker]
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.45f)]
        [ShowIf("@variable && referenceType != ReferenceType.Dynamic && !useLocalConstant")]
        [HideLabel]
        protected override B value {
            get {
                if (variable != null) {
                    if (referenceType == ReferenceType.Dynamic)
                        return variable.DynamicValue;
                    else if (referenceType == ReferenceType.GlobalConstant)
                        return variable.GlobalConstantValue;
                }

                return customLocalConstantValue;
            }
            set {
                if (variable != null && referenceType == ReferenceType.GlobalConstant){
                    variable.GlobalConstantValue = value;
                    SetDirty();
                }
                variable.ValueChanged?.Invoke(value, referenceType);
            }
        }

        protected override string denotation {
            get { return "CU"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().CustomDenotionColor; }
        }
    }
}
