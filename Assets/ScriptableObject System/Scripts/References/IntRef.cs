using System;
using UnityEngine;

namespace SOS {
    [Serializable]
    public class IntRef : GenericReference<ScriptableInt, int> {
        protected override string denotation {
            get { return "IN"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().IntDenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().IntRefPath;
            base.CreateScriptableObject();
        }
    }
}