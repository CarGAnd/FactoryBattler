using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GridSystem {
    [CustomEditor(typeof(GridLayout))]
    public class GridLayoutCustomEditor : Editor {
        private GridLayout targetGridLayout;

        private SerializedProperty gridRotation;
        private SerializedProperty gridOrigin;

        private void OnEnable() {
            targetGridLayout = (GridLayout)target;
            gridRotation = serializedObject.FindProperty("<Rotation>k__BackingField");
            gridOrigin = serializedObject.FindProperty("<Origin>k__BackingField");
        }

        private void OnSceneGUI() {
            Vector3 pivotPosition = targetGridLayout.Origin + targetGridLayout.RotationPivot;
            switch (Tools.current) {
                case Tool.Move:
                    Vector3 movedPos = Handles.PositionHandle(pivotPosition, targetGridLayout.Rotation);
                    gridOrigin.vector3Value = movedPos - targetGridLayout.RotationPivot;
                    break;
                case Tool.Rotate:
                    Quaternion newRotation = Handles.RotationHandle(targetGridLayout.Rotation, pivotPosition);
                    gridRotation.quaternionValue = newRotation;
                    break;
            }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
