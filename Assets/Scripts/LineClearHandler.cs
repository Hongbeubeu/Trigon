using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineClearHandler
{
    private const float CLEAR_TILE_DELAY = 0.01f;

    private readonly Dictionary<int, List<Vector3Int>> _linesByX = new();
    private readonly Dictionary<int, List<Vector3Int>> _linesByY = new();
    private readonly Dictionary<int, List<Vector3Int>> _linesByZ = new();
    private readonly BoardState _boardState;

    public LineClearHandler(BoardState boardState)
    {
        _boardState = boardState;
    }

    public void BuildAxisMapping()
    {
        _linesByX.Clear();
        _linesByY.Clear();
        _linesByZ.Clear();

        foreach (var kvp in _boardState.BoardTiles)
        {
            var coord = kvp.Key;
            AddToAxis(_linesByX, coord.x, coord);
            AddToAxis(_linesByY, coord.y, coord);
            AddToAxis(_linesByZ, coord.z, coord);
        }
    }

    public int ClearCompletedLines(List<Vector3Int> recentCoords, MonoBehaviour coroutineHost)
    {
        var completedLines = new List<List<Vector3Int>>();
        var checkedX = new HashSet<int>();
        var checkedY = new HashSet<int>();
        var checkedZ = new HashSet<int>();

        foreach (var coord in recentCoords)
        {
            TryCollectLine(_linesByX, coord.x, checkedX, completedLines);
            TryCollectLine(_linesByY, coord.y, checkedY, completedLines);
            TryCollectLine(_linesByZ, coord.z, checkedZ, completedLines);
        }

        int totalTiles = 0;
        foreach (var line in completedLines)
        {
            totalTiles += line.Count;
            coroutineHost.StartCoroutine(ClearLineAnimated(line));
        }

        return totalTiles * completedLines.Count;
    }

    private static void AddToAxis(Dictionary<int, List<Vector3Int>> dict, int key, Vector3Int coord)
    {
        if (!dict.TryGetValue(key, out var list))
        {
            list = new List<Vector3Int>();
            dict[key] = list;
        }

        list.Add(coord);
    }

    private void TryCollectLine(
        Dictionary<int, List<Vector3Int>> axisLines,
        int axisValue,
        HashSet<int> alreadyChecked,
        List<List<Vector3Int>> results)
    {
        if (!alreadyChecked.Add(axisValue)) return;
        if (!axisLines.TryGetValue(axisValue, out var line)) return;

        foreach (var coord in line)
        {
            if (!_boardState.BoardTiles[coord].isContainsTile)
                return;
        }

        results.Add(new List<Vector3Int>(line));
    }

    private IEnumerator ClearLineAnimated(List<Vector3Int> line)
    {
        foreach (var coord in line)
        {
            _boardState.RemoveTileAt(coord);
            yield return new WaitForSeconds(CLEAR_TILE_DELAY);
        }
    }
}
