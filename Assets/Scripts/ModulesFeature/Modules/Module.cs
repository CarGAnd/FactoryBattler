using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleInputOutput))]
public abstract class Module : MonoBehaviour
{
    private ModuleInputOutput inputOutput;

    private void Awake() {
        inputOutput = GetComponent<ModuleInputOutput>();
    }

    protected void SendObjectOut(AssemblyTravelingObject obj) {
        inputOutput.SendToOutput(obj);
    }

    protected bool OutputHasRoom() {
        return inputOutput.OutputHasRoom();
    }

    protected AssemblyTravelingObject GetObjectIn() {
        return inputOutput.ReceiveFromInput();    
    }
}
