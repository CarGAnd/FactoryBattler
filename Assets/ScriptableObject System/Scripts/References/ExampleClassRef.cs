using System;
using UnityEngine;

namespace SOS.Example {
[Serializable]
    public class ExampleClassRef : CustomReference<ScriptableExampleClass, ExampleClass> {
        protected override string denotation {
            get { return "CU"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().BoolDenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().BoolRefPath;
            base.CreateScriptableObject();
        }
     }
}