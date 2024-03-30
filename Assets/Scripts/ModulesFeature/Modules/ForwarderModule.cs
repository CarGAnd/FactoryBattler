using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwarderModule : Module
{
    private void Update() {
        if (!OutputHasRoom()) {
            return;
        }
        AssemblyTravelingObject inputObj = GetObjectIn();
        if(inputObj != null) {
            SendObjectOut(inputObj);
        }
    }
}
