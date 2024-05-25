using System;
using AttributeSystem;
using Sirenix.OdinInspector;
using SOS;
using UnityEngine;

public class TestAttributeKeyList : MonoBehaviour
{
    public AttributeName attributeName;

    public IntRef intRef;
    public BoolRef boolRef;
    public Vector2Ref vector2Ref;
    public Vector3Ref vector3Ref;
    public QuaternionRef quaternionRef;
    public GameEvent gameEvent;

    AttributeManager AttributeManager { get; }

    private void AddToAttribute(int value) {
        attributeName = AttributeKeys.CardDrawAdditive;
        AttributeManager.AddAttributeByName(attributeName, value);
    }

    [Button]
    public void TestButton () {
        TestRefUse(intRef, boolRef, vector2Ref, vector3Ref, quaternionRef);
    }

    public void TestRefUse(int intvalue, bool boolvalue, Vector2 vector2, Vector3 vector3, Quaternion quaternion) {
        Debug.Log($"Values are: {intvalue}, {boolvalue}, {vector2}, {vector3} and {quaternion}.");
    }
}