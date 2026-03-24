using System.Collections.Generic;
using UnityEngine;

public interface ILineClearHandler
{
    int ClearCompletedLines(List<GridCoord> recentCoords, MonoBehaviour coroutineHost);
}
