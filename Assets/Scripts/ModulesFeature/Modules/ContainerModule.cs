using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerModule : Module
{
    private void Update() {
        AssemblyTravelingObject inputObj = GetObjectIn();
        if(inputObj != null) {
            Destroy(inputObj.gameObject);
        }
    }
}
