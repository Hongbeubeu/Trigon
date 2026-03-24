using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Evaluates newly placed tiles to detect mathematically completed axes strings.
/// Communicates with BoardLogic to compute and TileViewRegistry to animate destruction.
/// </summary>
public class LineClearHandler : ILineClearHandler
{
    private readonly IBoardLogic _boardLogic;
    private readonly ITileViewRegistry _viewRegistry;

    public LineClearHandler(IBoardLogic boardLogic, ITileViewRegistry viewRegistry)
    {
        _boardLogic = boardLogic;
        _viewRegistry = viewRegistry;
    }

    public int ClearCompletedLines(List<GridCoord> recentCoords, MonoBehaviour coroutineHost)
    {
        var completedLines = _boardLogic.FindCompletedLines(recentCoords);
        int score = BoardLogic.CalculateLineScore(completedLines);

        foreach (var line in completedLines)
        {
            coroutineHost.StartCoroutine(_viewRegistry.AnimateClearLine(line, _boardLogic));
        }

        return score;
    }
}
