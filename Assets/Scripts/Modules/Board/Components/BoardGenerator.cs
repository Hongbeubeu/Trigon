using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    private readonly List<(Vector2 position, GridCoord coord)> _debugTiles = new();
    private readonly List<BoardTile> _spawnedBoardTiles = new();

    public void Generate(BoardData boardData, TileViewRegistry viewRegistry, LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        DespawnAll();
        var rowCount = logicConfig.BoardRowCount;
        var minAxis = logicConfig.BoardMinAxisValue;
        var tileWidth = viewConfig.TileWidth;
        var position = new Vector2(0f, viewConfig.BoardStartY);
        for (var row = 0; row < rowCount; row++)
        {
            var gridCoord = new GridCoord(row, row, 0);
            var tilesInRow = 2 * row + 1;
            for (var col = 0; col < tilesInRow; col++)
            {
                var isUpTile = col % 2 == 0;
                var prefab = isUpTile ? viewConfig.UpTilePrefab : viewConfig.DownTilePrefab;
                var isWithinBounds = gridCoord.x > minAxis - 1 &&
                                     gridCoord.y < rowCount - minAxis &&
                                     gridCoord.z < rowCount - minAxis;
                if (isWithinBounds)
                {
                    InstantiateTile(prefab, position, gridCoord, viewConfig.BoardColor, boardData, viewRegistry);
                }
                position.x += tileWidth;
                if (col % 2 == 0)
                    gridCoord.y--;
                else
                    gridCoord.z++;
            }
            position.x -= tilesInRow * tileWidth + tileWidth;
            position.y--;
        }
        ScaleBoard(viewConfig.BoardScale);
        viewRegistry.SyncWorldPositions(boardData);
    }

    public void DespawnAll()
    {
        foreach (var tile in _spawnedBoardTiles)
        {
            if (tile != null) LeanPool.Despawn(tile);
        }
        _spawnedBoardTiles.Clear();
        _debugTiles.Clear();
    }

    private void ScaleBoard(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }

    private void InstantiateTile(BoardTile prefab, Vector2 position, GridCoord gridCoord, Color boardColor, BoardData boardData, TileViewRegistry viewRegistry)
    {
        var tileView = LeanPool.Spawn(prefab, position, Quaternion.identity, transform);
        tileView.SpriteRenderer.color = boardColor;
        var cellData = new TileCellData(gridCoord, new Position2D(position.x, position.y), prefab.type);
        boardData.RegisterCell(cellData);
        viewRegistry.RegisterBoardTileView(gridCoord, tileView);
        _spawnedBoardTiles.Add(tileView);
#if UNITY_EDITOR
        _debugTiles.Add((position, gridCoord));
#endif
    }

#if UNITY_EDITOR
    [Header("Gizmo Debug")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Color _gizmoColor = Color.yellow;
    [SerializeField] private int _gizmoFontSize = 12;
    private void OnDrawGizmos()
    {
        if (!_showGizmos || _debugTiles.Count == 0) return;
        var style = new GUIStyle
                    {
                        fontSize = _gizmoFontSize,
                        fontStyle = FontStyle.Bold,
                        alignment = TextAnchor.MiddleCenter,
                        normal =
                        {
                            textColor = _gizmoColor
                        }
                    };
        Gizmos.color = _gizmoColor;
        foreach (var (position, coord) in _debugTiles)
        {
            var worldPos = transform.TransformPoint(position);
            UnityEditor.Handles.Label(worldPos, $"({coord.x},{coord.y},{coord.z})", style);
        }
    }
#endif
}