using System;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SOS {
    [InlineProperty]
    [LabelWidth(100)]
    [Serializable]
    public abstract class SOSReference<A> where A : ScriptableObject
    {
        // Name of scriptable object.
        [SerializeField]
        [LabelWidth(40)]
        [HorizontalGroup("row", Width = 0.45f)]
        [HideIf("variable")]
        [Tooltip("Please provide a unique name.")]
        protected string name;

        // Small denotion of type, only for inspector. 
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.06f)]
        [PropertyOrder(-1)]
        [GUIColor("$denotionColor")]
        [HideLabel]
        [DisplayAsString]
        [Sirenix.OdinInspector.ReadOnly]
        protected abstract string denotation { get; }

        [SerializeField]
        [HideInInspector]
        protected A scriptableObject;

        // Scriptable Object of specific type.
        [Required]
        protected virtual A variable {
            get {return scriptableObject;}
            set {scriptableObject = value;}
        }

        // When true, the reference only returns the local constant, whereas all other references of the same object are unchanged.
        [SerializeField]
        [HideInInspector]
        protected bool useLocalConstant = false;
        protected string sOSaveLocation = "Assets/ScriptableObjects/";
        protected abstract Color32 denotionColor { get; }

        // Creation of Object.
        [HorizontalGroup("row", Width = 0.35f)]
        [LabelWidth(150)]
        [Button("Create"), GUIColor(0, 0.9f, 0)]
        [HideIf("@variable || string.IsNullOrEmpty(name)")]
        protected virtual void CreateScriptableObject () {
            variable = ScriptableObject.CreateInstance<A>();
            AssetDatabase.CreateAsset(variable, Path.Combine(SettingsHelperSOS.GetSettings().SosDynamicParentPath, sOSaveLocation, name + ".asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Sets the variable dirty, saving the data between engine restarts, only during editing.
        protected virtual void SetDirty () {
            if (!EditorApplication.isPlaying && SettingsHelperSOS.GetSettings().SetDirtyInEditor) {
                EditorUtility.SetDirty(variable);
            }
        }
    }
}
