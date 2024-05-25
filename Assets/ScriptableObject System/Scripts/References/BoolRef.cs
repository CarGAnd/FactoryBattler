using System;
using UnityEngine;

namespace SOS {
[Serializable]
    public class BoolRef : GenericReference<ScriptableBool, bool> {
        protected override string denotation {
            get { return "BL"; }
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