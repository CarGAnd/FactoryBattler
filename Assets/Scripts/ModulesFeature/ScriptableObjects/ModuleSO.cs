using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : GridObjectSO
{
    public PortLayout portLayout;

    public List<PortSettings> GetPorts(Facing facing, Vector2Int modulePosition) {
        List<PortSettings> portSettings = portLayout.GetPorts();
        foreach(PortSettings ps in portSettings) {
            ps.gridPosition = GetRotatedPosition(ps.gridPosition, facing.GetNumRotations()) + modulePosition;
            ps.inputDirection = ps.inputDirection.RotatedDirection(facing.GetNumRotations());
            ps.outputDirection = ps.outputDirection.RotatedDirection(facing.GetNumRotations());
        }
        return portSettings;
    }

    public override IGridObject CreateInstance(Vector3 position, Quaternion rotation, Facing facing, AssemblyLineSystem assemblyLineSystem) {
        GameObject g = Instantiate(ModulePrefab, position, rotation);
        ModuleInputOutput inputOutput = g.GetComponent<ModuleInputOutput>();
        inputOutput.Initialize(this, facing, assemblyLineSystem);
        return inputOutput;
    }
}
