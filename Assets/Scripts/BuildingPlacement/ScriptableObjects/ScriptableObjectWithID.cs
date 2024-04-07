using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif

public abstract class ScriptableObjectWithID : ScriptableObject {

    [SerializeField, ReadOnly] private string id;

    public string ID => id;

    private static Dictionary<string, ScriptableObjectWithID> database = new Dictionary<string, ScriptableObjectWithID>();
    private static Dictionary<ScriptableObjectWithID, string> inverseDatabase = new Dictionary<ScriptableObjectWithID, string>();

    private static void RegisterObject(ScriptableObjectWithID obj) {
        if (obj == null || string.IsNullOrEmpty(obj.ID)) {
            return;
        }

        if (database.ContainsKey(obj.ID)) {
            ScriptableObjectWithID dbObj;
            database.TryGetValue(obj.ID, out dbObj);
            if(dbObj != obj) {
                Debug.LogWarning("Two objects exist with the same id, please give either object a new id", obj);
                Debug.LogWarning("Two objects exist with the same id, please give either object a new id", dbObj);
                return;
            }
            else {
                return;
            }
        }

        database.Add(obj.ID, obj);
        inverseDatabase.Add(obj, obj.ID);
    }

    private static void UnRegisterObject(ScriptableObjectWithID obj) {
        if (obj == null || string.IsNullOrEmpty(obj.ID)) {
            return;
        }
        database.Remove(obj.ID);
        inverseDatabase.Remove(obj);
    }

    public static ScriptableObjectWithID GetObjectByID(string id) {
        ScriptableObjectWithID obj;
        database.TryGetValue(id, out obj);
        return obj;
    }

    private void OnEnable() {
        if (string.IsNullOrEmpty(id)) {
            RecalcID();
        }
        RegisterObject(this);
    }

    private void Reset() {
        if (!inverseDatabase.ContainsKey(this)) {
            RecalcID();
        }
        else {
            inverseDatabase.TryGetValue(this, out id);
        }
    }

    private void OnDisable() {
        UnRegisterObject(this);
    }

    [ContextMenu("New id")]
    private void RecalcID() {
        UnRegisterObject(this);

        id = GenerateID();
#if UNITY_EDITOR
        EditorUtility.SetDirty(this);
#endif

        RegisterObject(this);
    }

    private string GenerateID() {
        return Guid.NewGuid().ToString();
    }
}

public class ReadOnlyAttribute : PropertyAttribute {

}

