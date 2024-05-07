using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestEvents : MonoBehaviour
{

    public static UnityEvent testEvent = new UnityEvent();

    [Button]
    public void InvokeEvent() {
        testEvent?.Invoke();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
