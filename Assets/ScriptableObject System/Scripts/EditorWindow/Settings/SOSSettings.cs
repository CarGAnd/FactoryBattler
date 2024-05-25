using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SOS {
    [CreateAssetMenu(fileName = "SOSSettings", menuName = "SOS/Settings", order = 1)]
    [InlineEditor]
    public class SOSSettings : ScriptableObject
    {
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true)]
        private string sosDynamicParentPath = "Assets/ScriptableObject System";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(ParentFolder = "$sosDynamicParentPath", RequireExistingPath = true)]
        private string boolRefPath = "ScriptableObjects/Bools";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string floatRefPath = "ScriptableObjects/Floats";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string intRefPath = "ScriptableObjects/Ints";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string vector2RefPath = "ScriptableObjects/Vector2s";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string vector3RefPath = "ScriptableObjects/Vector3s";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string quaternionRefPath = "ScriptableObjects/Quaternions";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string gameEventRefPath = "ScriptableObjects/GameEvents";
        [SerializeField]
        [FoldoutGroup("Paths", expanded: true)]
        [FolderPath(RequireExistingPath = true, ParentFolder = "$sosDynamicParentPath")]
        private string custemRefPath = "ScriptableObjects/Custom";

        [SerializeField]
        [FoldoutGroup("Colors", expanded: true)]
        [HorizontalGroup("Colors/Row1", LabelWidth = 200)]
        [VerticalGroup("Colors/Row1/Left")]
        private Color32 boolDenotionColor = new Color32(252, 218, 224, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Right")]
        private Color32 floatDenotionColor = new Color32(255, 231, 1, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Left")]
        private Color32 intDenotionColor = new Color32(61, 128, 255, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Right")]
        private Color32 vector2DenotionColor = new Color32(40, 255, 185, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Left")]
        private Color32 vector3DenotionColor = new Color32(165, 74, 91, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Right")]
        private Color32 quaternionDenotionColor = new Color32(255, 85, 242, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Left")]
        private Color32 gameEventDenotionColor = new Color32(255, 85, 242, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Right")]
        private Color32 customDenotionColor = new Color32(255, 85, 242, 255);
        [SerializeField]
        [VerticalGroup("Colors/Row1/Left")]
        private Color32 gameEventUnsubscribeColor = new Color32(243, 109, 134, 255);

        [SerializeField]
        [FoldoutGroup("General", expanded: true)]
        [HorizontalGroup("General/Row1", LabelWidth = 200)]
        [VerticalGroup("General/Row1/Left")]
        private bool setDirtyInEditor = true;
        [SerializeField]
        [VerticalGroup("General/Row1/Right")]
        [EnumPaging]
        private ReferenceType startReferenceIn = ReferenceType.Dynamic;

        public string BoolRefPath { get => boolRefPath; private set => boolRefPath = value; }
        public string FloatRefPath { get => floatRefPath; private set => floatRefPath = value; }
        public string IntRefPath { get => intRefPath; private set => intRefPath = value; }
        public string Vector2RefPath { get => vector2RefPath; private set => vector2RefPath = value; }
        public string Vector3RefPath { get => vector3RefPath; private set => vector3RefPath = value; }
        public string QuaternionRefPath { get => quaternionRefPath; private set => quaternionRefPath = value; }
        public Color32 BoolDenotionColor { get => boolDenotionColor; private set => boolDenotionColor = value; }
        public Color32 FloatDenotionColor { get => floatDenotionColor; private set => floatDenotionColor = value; }
        public Color32 IntDenotionColor { get => intDenotionColor; private set => intDenotionColor = value; }
        public Color32 Vector2DenotionColor { get => vector2DenotionColor; private set => vector2DenotionColor = value; }
        public Color32 Vector3DenotionColor { get => vector3DenotionColor; private set => vector3DenotionColor = value; }
        public Color32 QuaternionDenotionColor { get => quaternionDenotionColor; private set => quaternionDenotionColor = value; }
        public string SosDynamicParentPath { get => sosDynamicParentPath; private set => sosDynamicParentPath = value; }
        public Color32 GameEventDenotionColor { get => gameEventDenotionColor; private set => gameEventDenotionColor = value; }
        public string GameEventRefPath { get => gameEventRefPath; private set => gameEventRefPath = value; }
        public bool SetDirtyInEditor { get => setDirtyInEditor; private set => setDirtyInEditor = value; }
        public Color32 GameEventUnsubscribeColor { get => gameEventUnsubscribeColor; private set => gameEventUnsubscribeColor = value; }
        public ReferenceType StartReferenceIn { get => startReferenceIn; private set => startReferenceIn = value; }
        public string CustemRefPath { get => custemRefPath; private set => custemRefPath = value; }
        public Color32 CustomDenotionColor { get => customDenotionColor; private set => customDenotionColor = value; }
    }
}