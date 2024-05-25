using System;
using UnityEngine;

namespace SOS {
    [Serializable]
    public class Vector3Ref : GenericReference<ScriptableVector3, Vector3> {
        protected override string denotation {
            get { return "V3"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().Vector3DenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().Vector3RefPath;
            base.CreateScriptableObject();
        }
    }
}