using System.Collections.Generic;
using UnityEngine;

public enum Facing
{
    North = 0,
    East = 10,
    South = 20,
    West = 30
}

public static class FacingExtentions {

    private static Facing[] facingMap = new Facing[] { Facing.West, Facing.North, Facing.East, Facing.South };

    public static Vector2Int GetIntDirection(this Facing facing) {
        switch (facing) {
            case Facing.North:
                return new Vector2Int(0, 1);
            case Facing.East:
                return new Vector2Int(1, 0);
            case Facing.South:
                return new Vector2Int(0, -1);
            case Facing.West:
                return new Vector2Int(-1, 0);
            default:
                return new Vector2Int(0, 1);
        }
    }

    public static Facing RotatedDirection(this Facing inputDirection, int numRotations) {
        int startNumRotations = inputDirection.GetNumRotations();
        int newNumRotations = (startNumRotations + numRotations) % 4;
        if(newNumRotations < 0) {
            newNumRotations += 4;
        }
        return facingMap[newNumRotations];
    }

    public static int GetNumRotations(this Facing inputFacing) {
        return inputFacing switch
        {
            Facing.West => 0,
            Facing.North => 1,
            Facing.East => 2,
            Facing.South => 3,
            _ => 0,
        };

    }

    public static Facing GetOppositeFacing(this Facing facing)
    {
        return facing switch
        {
            Facing.North => Facing.South,
            Facing.East => Facing.West,
            Facing.South => Facing.North,
            Facing.West => Facing.East,
            _ => Facing.East,
        };
    }

    public static List<Facing> GetPerpendicularFacings(this Facing facing)
    {
        return facing switch
        {
            Facing.North => new List<Facing> { Facing.East, Facing.West },
            Facing.East => new List<Facing> { Facing.North, Facing.South },
            Facing.South => new List<Facing> { Facing.East, Facing.West },
            Facing.West => new List<Facing> { Facing.North, Facing.South },
            _ => new List<Facing> { Facing.North, Facing.South },
        };
    }

    public static Quaternion GetRotationFromFacing(this Facing facing)
    {
        return facing switch
        {
            Facing.West => Quaternion.Euler(0, 0, 0),
            Facing.North => Quaternion.Euler(0, 90, 0),
            Facing.East => Quaternion.Euler(0, 180, 0),
            Facing.South => Quaternion.Euler(0, 270, 0),
            _ => Quaternion.Euler(0, 0, 0),
        };
    }
}
