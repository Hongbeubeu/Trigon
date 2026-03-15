using System.Collections.Generic;
using UnityEngine;

public class LineClearHandler
{
    private readonly BoardLogic _boardLogic;
    private readonly TileViewRegistry _viewRegistry;

    public LineClearHandler(BoardLogic boardLogic, TileViewRegistry viewRegistry)
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
