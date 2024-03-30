using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PortLayout 
{
    private static Facing[] directionMap = new Facing[] { Facing.North, Facing.East, Facing.South, Facing.West };

    [SerializeField] private int width;
    [SerializeField] private int height;

    [SerializeField] private int[] inputs;
    [SerializeField] private int[] outputs;

    public List<PortSettings> GetPorts() {
        List<PortSettings> ports = new List<PortSettings>();
        for(int y = 0; y < height; y++) {
            for(int x = 0; x < width; x++) {
                //Flip the y axis so 0 is at the bottom
                int arrayIndex = (height - y - 1) * width + x;
                int inputValue = inputs[arrayIndex];
                int outputValue = outputs[arrayIndex];
                if(inputValue <= 0 && outputValue <= 0) {
                    continue;
                }
                inputValue -= 1;
                outputValue -= 1;
                PortSettings newPort = new PortSettings()
                {
                    gridPosition = new Vector2Int(x, y),
                    inputDirection = inputValue >= 0 ? directionMap[inputValue] : Facing.North,
                    outputDirection = outputValue >= 0 ? directionMap[outputValue] : Facing.North,
                    isInput = inputValue >= 0,
                    isOutput = outputValue >= 0
                };
                ports.Add(newPort);
            }
        }
        return ports;
    }    
}

public class PortSettings {
    public Vector2Int gridPosition;
    public Facing inputDirection;
    public Facing outputDirection;
    public bool isInput;
    public bool isOutput;
}
