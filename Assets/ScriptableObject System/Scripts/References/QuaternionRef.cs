using System;
using UnityEngine;

namespace SOS {
    [Serializable]
    public class QuaternionRef : GenericReference<ScriptableQuaternion, Quaternion> {
        protected override string denotation {
            get { return "QU"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().QuaternionDenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().QuaternionRefPath;
            base.CreateScriptableObject();
        }
    }
}