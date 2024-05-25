using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SOS {
[Serializable]
    public class FloatRef : GenericReference<ScriptableFloat, float> {
        protected override string denotation {
            get { return "FL"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().FloatDenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().FloatRefPath;
            base.CreateScriptableObject();
        }
    }
}