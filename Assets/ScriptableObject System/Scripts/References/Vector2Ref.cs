using System;
using UnityEngine;

namespace SOS {
    [Serializable]
    public class Vector2Ref : GenericReference<ScriptableVector2, Vector2> {
        protected override string denotation {
            get { return "V2"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().Vector2DenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().Vector2RefPath;
            base.CreateScriptableObject();
        }
    }
}