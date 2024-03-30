using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AssemblyLineManager
{
    private List<AssemblyLine> assemblyLines;
    private AssemblyLineSystem assemblyLineSystem;
    public AssemblyLineManager(List<AssemblyLine> assemblyLines, AssemblyLineSystem assemblyLineSystem)
    {
        this.assemblyLines = assemblyLines;
        this.assemblyLineSystem = assemblyLineSystem;
    }
    public void PlaceTransportablePiece(ITransportable newPiece)
    {
        if(newPiece != null)
        {
            AddPieceToAssemblyLine(newPiece);
        }
    }

    private void AddPieceToAssemblyLine(ITransportable piece)
    {
        ITransportable intersectedPiece;
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out intersectedPiece);
        List<AssemblyLine> endLines = FindLinesPieceIsEndOf(piece);

        if(intersectedLine != null && endLines.Count > 0)
        {
            HandleMultipleIntersections(piece, intersectedPiece, intersectedLine, endLines);
        }
        else if(intersectedLine != null)
        {
            HandleSingleIntersection(piece, intersectedPiece, intersectedLine);
        }
        else if(endLines.Count > 0)
        {
            HandleEndConnections(piece, endLines);
        }
        else
        {
            CreateNewLine(piece);
        }
    }

    private void HandleEndConnections(ITransportable piece, List<AssemblyLine> endLines)
    {
        AssemblyLine correctEndLine = FindCorrectEndLine(endLines, piece);
        if (correctEndLine != null)
        {
            correctEndLine.AddPiece(piece);
            HandleConnectingLines(correctEndLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);
        }
        else
        {
            AssemblyLine newLine = CreateNewLine(piece);
            HandleConnectingLines(newLine, endLines, piece);
        }
    }

    private void HandleSingleIntersection(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine)
    {
        //The case where the new piece is just the new start of an intersected line.
        if (intersectedPiece.GetInputDirections().Contains(piece.Facing) && intersectedPiece.Facing == piece.Facing)
        {
            intersectedLine.AddPiece(piece);
        }
        //If the new piece is intersecting at the middle of a line without being the end of other lines
        else
        {
            AssemblyLine newLine = CreateNewLine(piece);
            HandleConnectingLines(intersectedLine, new List<AssemblyLine> { newLine }, intersectedPiece);
        }
    }

    private void HandleMultipleIntersections(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        //We start by considering the scenario where there are two lines are facing the same way.
        AssemblyLine correctEndLine = FindCorrectEndLine(endLines, intersectedPiece, piece);
        if (correctEndLine != null)
        {
            CombineLinesFacingSameWay(piece, intersectedLine, endLines, correctEndLine);
        }
        //Alright we assume the endlines do not face the same way as the intersected line. 
        else
        {
            correctEndLine = FindCorrectEndLine(endLines, piece);
            //First case is that the two lines are facing each other. 
            if(correctEndLine != null && FacingExtentions.GetOppositeFacing(piece.Facing) == intersectedPiece.Facing)
            {
                endLines.Remove(correctEndLine);
                CombineOppositeFacingLines(piece, intersectedPiece, correctEndLine, intersectedLine, endLines);
            }
            //Second case is that the new piece is facing the same way as an end line, but not the intersected line.
            else if (correctEndLine != null)
            {
                endLines.Remove(correctEndLine);
                CombineEndLineFacingPiece(piece, intersectedPiece, intersectedLine, endLines, correctEndLine);
            }
            //However the new piece might still face the same way as the intersected line, without having a same facing end line.
            else if(piece.Facing == intersectedPiece.Facing && intersectedPiece.GetInputDirections().Contains(piece.Facing))
            {
                CombineEndLinesWithIntersectedLine(piece, intersectedLine, endLines);
            }
            //In case they don't face the same way as the piece, and the piece doesn't face the same way as the line.
            else
            {
                CombineEndLinesFacingDifferentThanPiece(piece, intersectedPiece, intersectedLine, endLines);
            }
        }
    }
    private void CombineLinesFacingSameWay(ITransportable piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We merge the two lines. The first part of loop detection happens in MergeLines
        AssemblyLine resultingLine = MergeLines(intersectedLine, correctEndLine, piece);
        HandleConnectingLines(resultingLine, endLines.Except(new[] { correctEndLine }).ToList(), piece);
        //Now we have the next loop scenario, if one of the connecting lines that was added piece results in a loop.
        foreach(AssemblyLine line in resultingLine.GetAllConnections())
        {
            if(PrePlaceLoopDetection(line, resultingLine))
            {
                resultingLine.RemoveConnection(line);
                assemblyLines.Add(line);
                GetIntersectedAssemblyLine(line.GetEndPiece(), out ITransportable i);
                resultingLine.HandleLoop(line, i);
                break;
            }
        }
    }
    private void CombineOppositeFacingLines(ITransportable piece, ITransportable intersectedPiece, AssemblyLine correctEndLine, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        correctEndLine.AddPiece(piece);
        //The two lines should become connecting lines of each other without removing from AssemblyLines.
        intersectedLine.AddConnectingAssemblyLine(correctEndLine, intersectedPiece);
        correctEndLine.AddConnectingAssemblyLine(intersectedLine, piece);
        //We add the endlines as connecting lines to the line with the piece being placed.
        HandleConnectingLines(correctEndLine, endLines, piece);
        //This can't possibly loop. 
    }
    private void CombineEndLineFacingPiece(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines, AssemblyLine correctEndLine)
    {
        //We add the piece to our end line.
        correctEndLine.AddPiece(piece);
        //And now we need to ensure that all endlines are connecting lines to this line.
        HandleConnectingLines(correctEndLine, endLines, piece);

        if(PrePlaceLoopDetection(correctEndLine, intersectedLine))
        {
            intersectedLine.HandleLoop(correctEndLine, intersectedPiece);
            return;
        }

        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { correctEndLine }, intersectedPiece);
        return;
    }
    private void CombineEndLinesWithIntersectedLine(ITransportable piece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        intersectedLine.AddPiece(piece);
        //We add the endlines as connecting lines to this line
        AssemblyLine loopingEndLine = null;
        foreach(AssemblyLine line in endLines)
        {
            if(PrePlaceLoopDetection(line, intersectedLine))
            {
                loopingEndLine = line;
                intersectedLine.HandleLoop(line, piece);
                break;
            }
        }

        HandleConnectingLines(intersectedLine, endLines.Except(new[] {loopingEndLine}).ToList(), piece);
    }

    private void CombineEndLinesFacingDifferentThanPiece(ITransportable piece, ITransportable intersectedPiece, AssemblyLine intersectedLine, List<AssemblyLine> endLines)
    {
        AssemblyLine newLine = CreateNewLine(piece);
        //We add the endlines as connecting lines to this line
        HandleConnectingLines(newLine, endLines, piece);

        if(PrePlaceLoopDetection(newLine, intersectedLine))
        {
            intersectedLine.HandleLoop(newLine, intersectedPiece);
            return;
        }
        //And now we need to add this line as a connecting line to the intersected line, at intersected piece.
        HandleConnectingLines(intersectedLine, new List<AssemblyLine> { newLine }, intersectedPiece);
    }

    private AssemblyLine GetIntersectedAssemblyLine(ITransportable piece, out ITransportable intersectedPiece)
    {
        HashSet<AssemblyLine> visited = new HashSet<AssemblyLine>();
        foreach(AssemblyLine line in assemblyLines)
        {
            visited.Clear();
            AssemblyLine intersectedLine = RecursiveIntersectionSearch(line, piece, visited, out intersectedPiece);
            if(intersectedLine != null)
            {
                return intersectedLine;
            }
        }
        intersectedPiece = null;
        return null;
    }
    private AssemblyLine RecursiveIntersectionSearch(AssemblyLine line, ITransportable piece, HashSet<AssemblyLine> visited, out ITransportable intersectedPiece)
    {
        //if the current line is intersected
        AssemblyLine intersectedLine = line.GetIntersectedAssemblyLine(piece.GetNextCellCoords(), visited, out intersectedPiece);
        if(intersectedLine != null)
        {
            return intersectedLine;
        }
        return null;
    }

    private AssemblyLine CreateNewLine(ITransportable piece)
    {
        AssemblyLine newLine = new AssemblyLine(assemblyLineSystem);
        newLine.AddPiece(piece);
        assemblyLines.Add(newLine);
        return newLine;
    }

    private AssemblyLine MergeLines(AssemblyLine startLine, AssemblyLine endLine, ITransportable piece)
    {
        // Add the connecting piece to the endLine (which will now be the start of the merged line).
        if(!startLine.ContainsPiece(piece, new HashSet<AssemblyLine>(), out AssemblyLine lineContainingPiece))
        {
            startLine.AddPiece(piece);
        }
        //Moves all nodes from startLine to endLine
        startLine.AppendLine(endLine);
        //Moves all connecting lines from startLine to endLine
        startLine.MoveConnectingLines(endLine);

        AssemblyLine nextLine = GetIntersectedAssemblyLine(endLine.GetEndPiece(), out ITransportable i);
        if (nextLine != null && i.GetInputDirections().Contains(endLine.GetEndPiece().Facing))
        {
            if (PrePlaceLoopDetection(startLine, endLine))
            {
                assemblyLines.Add(nextLine);
            }
            nextLine.ReplaceExistingLine(startLine, endLine, i);
            assemblyLines.Remove(endLine);
        }
        RemoveAssemblyLine(startLine);
        return endLine;
    }

    

    private void HandleConnectingLines(AssemblyLine mainLine, List<AssemblyLine> connectingLines, ITransportable connectingPiece)
    {
        foreach(AssemblyLine line in connectingLines)
        {
            if(connectingPiece.GetInputDirections().Contains(line.GetEndPiece().Facing))
            {
                mainLine.AddConnectingAssemblyLine(line, connectingPiece);
                assemblyLines.Remove(line);
            }
        }
    }
    private List<AssemblyLine> FindLinesPieceIsEndOf(ITransportable piece)
    {
        //Linq returns an empty list if no lines are found and not null.
        return assemblyLines.Where(line => line.IsPieceNewEnd(piece)).ToList();
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, ITransportable intersectedPiece, ITransportable newPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().Facing == intersectedPiece.Facing && newPiece.GetInputDirections().Contains(line.GetEndPiece().Facing) && intersectedPiece.GetInputDirections().Contains(newPiece.Facing) && newPiece.Facing == line.GetEndPiece().Facing);
    }

    private AssemblyLine FindCorrectEndLine(List<AssemblyLine> endLines, ITransportable intersectedPiece)
    {
        return endLines.FirstOrDefault(line => line.GetEndPiece().Facing == intersectedPiece.Facing);
    }
    //This is to be used before lines are actually connected.
    private bool PrePlaceLoopDetection(AssemblyLine correctEndLine, AssemblyLine intersectedLine)
    {
        HashSet<AssemblyLine> visited = new HashSet<AssemblyLine>();
        return CheckForLoop(correctEndLine, intersectedLine, visited);
    }

    private bool CheckForLoop(AssemblyLine currentLine, AssemblyLine targetLine, HashSet<AssemblyLine> visited)
    {
        if (currentLine == targetLine)
        {
            return true;
        }

        visited.Add(currentLine);

        foreach (var connectedLine in currentLine.GetAllConnections())
        {
            if (!visited.Contains(connectedLine) && CheckForLoop(connectedLine, targetLine, visited))
            {
                return true;
            }
        }

        return false;
    }
    #region Removal of Assembly Pieces
    //This is to be used once the lines are connected.
    private bool PostPlaceLoopDetection(AssemblyLine line)
    {
        HashSet<AssemblyLine> visited = new HashSet<AssemblyLine>();
        return DetectLoop(line, visited);
    }
    private bool DetectLoop(AssemblyLine targetLine, HashSet<AssemblyLine> visited)
    {
        if (visited.Contains(targetLine))
        {
            return true;
        }
        visited.Add(targetLine);
        foreach (AssemblyLine line in targetLine.GetAllConnections())
        {
            if (DetectLoop(line, visited))
            {
                return true;
            }
        }
        return false;
    }
    public void RemoveTransportablePiece(ITransportable piece)
    {
        AssemblyLine lineContainingPiece = FindLineContainingPiece(piece);
        if (lineContainingPiece != null)
        {
            // Handle removal based on the location of the piece in the line
            if (piece == lineContainingPiece.GetStartPiece() && piece == lineContainingPiece.GetEndPiece())
            {
                RemoveSinglePieceLine(lineContainingPiece, piece);
            }
            else if (piece == lineContainingPiece.GetEndPiece())
            {
                RemoveEndPiece(lineContainingPiece, piece);
            }
            else if (piece == lineContainingPiece.GetStartPiece())
            {
                RemoveStartPiece(lineContainingPiece, piece);
            }
            else
            {
                SplitLineAt(lineContainingPiece, piece);
            }
            CheckEndConnections();
        }
    }

    // Finds the line containing the specific piece
    private AssemblyLine FindLineContainingPiece(ITransportable piece)
    {
        HashSet<AssemblyLine> visitedLines = new HashSet<AssemblyLine>();
        foreach (AssemblyLine line in assemblyLines)
        {
            if(line.ContainsPiece(piece, visitedLines, out AssemblyLine lineContainingPiece))
            {
                return lineContainingPiece;
            }
        }
        return null;
    }


    // Handles removal when the piece is the only piece in the line
    private void RemoveSinglePieceLine(AssemblyLine lineContainingPiece, ITransportable piece)
    {
        //GetIntersectedAssemblyLine iterates through all AssemblyLine in assemblyLines. therefore ensure we don't remove any before checking intersection. 
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out ITransportable intersectedPiece);
        //Check Loop. Break removes a line from assemblyLines.
        if (PostPlaceLoopDetection(lineContainingPiece))
        {
            BreakLoop(lineContainingPiece);
        }
        //Check intersection
        if(intersectedLine != null && intersectedLine.HasConnection(lineContainingPiece))
        {
            intersectedLine.RemoveConnection(lineContainingPiece);
        }        
        //Check connections
        foreach(AssemblyLine line in lineContainingPiece.GetAllConnections())
        {
            assemblyLines.Add(line);
            lineContainingPiece.RemoveConnection(line);
        }
        //Removal
        assemblyLines.Remove(lineContainingPiece);
        lineContainingPiece.RemovePiece(piece);
    }

    // Handles removal when the piece is at the start of a line
    private void RemoveStartPiece(AssemblyLine lineContainingPiece, ITransportable piece)
    {
        if(PostPlaceLoopDetection(lineContainingPiece))
        {
            BreakLoop(lineContainingPiece);
        }
        List<AssemblyLine> connectingLines = lineContainingPiece.FindConnectingLines(piece);
        foreach(AssemblyLine connectingLine in connectingLines)
        {
            lineContainingPiece.RemoveConnection(connectingLine);
            assemblyLines.Add(connectingLine);
        }
        lineContainingPiece.RemovePiece(piece);
    }

    private void RemoveEndPiece(AssemblyLine lineContainingPiece, ITransportable piece)
    {
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(piece, out ITransportable intersectedPiece);
        if (PostPlaceLoopDetection(lineContainingPiece))
        {
            BreakLoop(lineContainingPiece);
        }
        if(intersectedLine != null && intersectedLine.HasConnection(lineContainingPiece))
        {
            intersectedLine.RemoveConnection(lineContainingPiece);
            assemblyLines.Add(lineContainingPiece);
        }
        List<AssemblyLine> connectingLines = lineContainingPiece.FindConnectingLines(piece);
        foreach (AssemblyLine connectingLine in connectingLines)
        {
            lineContainingPiece.RemoveConnection(connectingLine);
            assemblyLines.Add(connectingLine);
        }
        lineContainingPiece.RemovePiece(piece);
    }

    // Splits the line at the specified piece
    private void SplitLineAt(AssemblyLine lineContainingPiece, ITransportable piece)
    {
        AssemblyLine intersectedLine = GetIntersectedAssemblyLine(lineContainingPiece.GetEndPiece(), out ITransportable intersectedPiece);
        List<AssemblyLine> connectedLines = lineContainingPiece.FindConnectingLines(piece);
        if (PostPlaceLoopDetection(lineContainingPiece))
        {
            BreakLoop(lineContainingPiece);
        }
        foreach (AssemblyLine line in connectedLines)
        {
            lineContainingPiece.RemoveConnection(line);
            assemblyLines.Add(line);
        }
        AssemblyLine lineBeforeSplit = new AssemblyLine(assemblyLineSystem);
        AssemblyLine lineAfterSplit = new AssemblyLine(assemblyLineSystem);
        lineContainingPiece.Split(lineBeforeSplit, lineAfterSplit, piece);
        if(intersectedLine != null && intersectedLine.HasConnection(lineContainingPiece))
        {
            intersectedLine.ReplaceExistingLine(lineContainingPiece, lineAfterSplit, intersectedPiece);
        }
        else
        {
            assemblyLines.Add(lineAfterSplit);
        }

        assemblyLines.Add(lineBeforeSplit);
        RemoveAssemblyLine(lineContainingPiece);
    }

    // Breaks a loop in the assembly line system
    private void BreakLoop(AssemblyLine loopingLine)
    {
        HashSet<AssemblyLine> visited = new HashSet<AssemblyLine>();
        RecursiveBreakLoop(loopingLine, visited);
    }
    private void RecursiveBreakLoop (AssemblyLine currentLine, HashSet<AssemblyLine> visited)
    {
        if (visited.Contains(currentLine))
        {
            return;
        }
        visited.Add(currentLine);
        if(assemblyLines.Contains(currentLine))
        {
            assemblyLines.Remove(currentLine);
        }
        else
        {
            foreach(AssemblyLine line in currentLine.GetAllConnections())
            {
                RecursiveBreakLoop(line, visited);
            }
        }
    }
    //If after we've removed a piece a connection is now possible we connect these lines.
    private void CheckEndConnections()
    {
        int startCount = assemblyLines.Count;
        for(int i = 0; i < startCount; i++)
        {
            AssemblyLine intersectedLine = GetIntersectedAssemblyLine(assemblyLines[i].GetEndPiece(), out ITransportable intersectedPiece);
            if(intersectedLine != null && intersectedPiece.GetInputDirections().Contains(assemblyLines[i].GetEndPiece().Facing))
            {
                int countBeforeHandle = assemblyLines.Count;
                if(intersectedPiece.Facing == assemblyLines[i].GetEndPiece().Facing)
                {
                    MergeLines(intersectedLine, assemblyLines[i], intersectedPiece);
                }
                else
                {
                    HandleConnectingLines(intersectedLine, new List<AssemblyLine>{assemblyLines[i]}, intersectedPiece);
                }
                int countAfterHandle = assemblyLines.Count;

                int itemsRemoved = countBeforeHandle - countAfterHandle;
                i -= itemsRemoved;
                startCount -= itemsRemoved;
            }
        }
    }
    #endregion
    private void RemoveAssemblyLine(AssemblyLine line)
    {
        line.CleanUp();
        assemblyLines.Remove(line);
    }
}
