using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLineVisualizer : MonoBehaviour
{
    public FactoryGrid grid;
    public AssemblyLineSystem assemblyLineSystem;
    private void OnDrawGizmos()
    {
        if(assemblyLineSystem != null)
        DrawAssemblyLines();
    }

    private void DrawAssemblyLines()
    {
        if(UnityEngine.Application.isPlaying)
        {
            foreach (var line in assemblyLineSystem.AssemblyLines)
            {
                DrawLine(line);
            }
        }
    }

    private void DrawLine(AssemblyLine line)
    {
        
        Vector3 start = grid.GetCellWorldPosition(line.GetStartPiece().GetGridCoords());
        Vector3 end = grid.GetCellWorldPosition(line.GetEndPiece().GetGridCoords());

        float heightOffset = 1f;
        AdjustLinePosition(ref start, ref end, grid.CellSize, heightOffset, line.GetStartPiece().Facing);
        Color newColor = GetUniqueColor(line);
        Gizmos.color = newColor;
        Gizmos.DrawLine(start, end);

        // Draw circle at start
        Gizmos.DrawSphere(start, 0.75f);

        // Draw triangle at end
        //We need to split this up into the case where end and start are the same location to avoid "Look rotation viewing vector is zero"
        //We're fixing it in AdjustLinePosition
        DrawTriangle(end, Quaternion.LookRotation(end - start), 1.25f);

        foreach(AssemblyLine connectingLine in line.GetAllConnections())
        {
            DrawConnectingLine(connectingLine, newColor);
        }
    }

    private void DrawConnectingLine(AssemblyLine line, Color color)
    {
        HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
        RecursiveDrawConnectingLine(line, color, visitedLines);
    }

    private void RecursiveDrawConnectingLine(AssemblyLine line, Color color, HashSet<AssemblyLine> visitedLines)
    {
        if (visitedLines.Contains(line))
            return;

        visitedLines.Add(line);

        Vector3 start = grid.GetCellWorldPosition(line.GetStartPiece().GetGridCoords());
        Vector3 end = grid.GetCellWorldPosition(line.GetEndPiece().GetGridCoords());

        float heightOffset = 1f;
        AdjustLinePosition(ref start, ref end, grid.CellSize, heightOffset, line.GetStartPiece().Facing);
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        // Draw circle at start
        Gizmos.DrawSphere(start, 0.6f);

        // Draw triangle at end
        DrawTriangle(end, Quaternion.LookRotation(end - start), 1.25f);

        foreach (AssemblyLine connectingLine in line.GetAllConnections())
        {
            RecursiveDrawConnectingLine(connectingLine, color, visitedLines);
        }
    }

    private Color GetUniqueColor(AssemblyLine line)
    {
        int index = assemblyLineSystem.AssemblyLines.IndexOf(line);
        float hue = index * 0.618033988749895f % 1; // The golden ratio conjugate is used for better distribution
        return Color.HSVToRGB(hue, 0.7f, 0.9f); // Adjust saturation and value as needed
    }

    private void DrawTriangle(Vector3 position, Quaternion rotation, float size)
    {
        // Draw a triangle using Gizmos
        Vector3 direction = rotation * Vector3.forward;
        Vector3 right = rotation * Vector3.right;

        Vector3 vertex1 = position + direction * size;
        Vector3 vertex2 = position + right * size / 2;
        Vector3 vertex3 = position - right * size / 2;

        Gizmos.DrawLine(vertex1, vertex2);
        Gizmos.DrawLine(vertex2, vertex3);
        Gizmos.DrawLine(vertex3, vertex1);
    }
    private void AdjustLinePosition(ref Vector3 start, ref Vector3 end, Vector2 cellSize, float heightOffset, Facing facing)
    {
        if(start == end)
        {
            switch(facing)
            {
                case Facing.North:
                {
                    start.x += cellSize.x / 2;
                    end.x += cellSize.x / 2;
                    end.z += cellSize.y;
                    break;
                }
                case Facing.East:
                {
                    start.z += cellSize.y / 2;
                    end.z += cellSize.y / 2;
                    end.x += cellSize.x;
                    break;
                }
                case Facing.West:
                {
                    start.z += cellSize.y / 2;
                    end.z += cellSize.y / 2;
                    start.x += cellSize.x;
                    break;
                }
                case Facing.South:
                {
                    start.x += cellSize.x / 2;
                    end.x += cellSize.x / 2;
                    start.z += cellSize.y;
                    break;
                }
            }
        }
        else
        {
            start.z += cellSize.y / 2;
            end.z += cellSize.y / 2;
            start.x += cellSize.x / 2;
            end.x += cellSize.x / 2;
        }

        // Apply height offset for all directions
        start.y = heightOffset;
        end.y = heightOffset;
    }


}

