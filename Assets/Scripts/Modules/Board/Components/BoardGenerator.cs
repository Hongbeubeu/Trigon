using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

/// <summary>
/// Pre-computes and maps out the mathematical hexagonal board pattern into
/// actual physical world-space Unity UI components on start.
/// </summary>
public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private BoardCameraFitter _cameraFitter;
    private readonly List<BoardTile> _spawnedBoardTiles = new();
    private float _upYOffset;
    private float _downYOffset;

    public void Generate(BoardData boardData, ITileViewRegistry viewRegistry, LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        DespawnAll();
        var rows = logicConfig.BoardRowCount;
        var cutOffLines = logicConfig.CutOffLines;
        var tileWidth = viewConfig.TileWidth;
        var position = Vector2.zero;
        _upYOffset = tileWidth * HexGridExtensions.SQRT3 / 6f;
        _downYOffset = tileWidth * HexGridExtensions.SQRT3 / 3f;
        for (var row = 0; row < rows; row++)
        {
            var gridCoord = new GridCoord(row, row, 0);
            var columns = 2 * row + 1;
            position.x = -(columns - 1) * tileWidth / 4f;
            for (var column = 0; column < columns; column++)
            {
                var isUpTile = column % 2 == 0;
                var offset = isUpTile ? _upYOffset : _downYOffset;
                var isWithinBounds = gridCoord.x > cutOffLines - 1 &&
                                     gridCoord.y < rows - cutOffLines &&
                                     gridCoord.z < rows - cutOffLines;
                position.y += offset;
                if (isWithinBounds)
                {
                    InstantiateTile(viewConfig.BoardTilePrefab, position, isUpTile, gridCoord, viewConfig.BoardColor, boardData, viewRegistry);
                }
                position.y -= offset;
                position.x += tileWidth / 2f;
                if (column % 2 == 0)
                    gridCoord.y--;
                else
                    gridCoord.z++;
            }
            position.y -= tileWidth * HexGridExtensions.SQRT3 / 2f;
        }
        ScaleBoard(viewConfig.BoardScale);
        viewRegistry.SyncWorldPositions(boardData);
        if (_cameraFitter != null)
        {
            _cameraFitter.FitCameraToBoard(_spawnedBoardTiles, tileWidth);
        }
    }

    public void DespawnAll()
    {
        foreach (var tile in _spawnedBoardTiles.Where(tile => tile != null))
        {
            LeanPool.Despawn(tile);
        }
        _spawnedBoardTiles.Clear();
#if UNITY_EDITOR
        _debugTiles.Clear();
#endif
    }

    private void ScaleBoard(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }

    private void InstantiateTile(BoardTile prefab, Vector2 position, bool isUpTile, GridCoord gridCoord, Color boardColor, BoardData boardData, ITileViewRegistry viewRegistry)
    {
        var rotation = isUpTile ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        var boardTile = LeanPool.Spawn(prefab, position, rotation, transform);
        boardTile.TileType = isUpTile ? TileType.Up : TileType.Down;
        boardTile.SpriteRenderer.color = boardColor;
        var tileData = new TileData(gridCoord, new Position2D(position.x, position.y), boardTile.TileType);
        boardData.RegisterTiles(tileData);
        viewRegistry.RegisterBoardTileView(gridCoord, boardTile);
        _spawnedBoardTiles.Add(boardTile);
#if UNITY_EDITOR
        _debugTiles.Add((position, gridCoord));
#endif
    }

#if UNITY_EDITOR
    [Header("Gizmo Debug")]
    [SerializeField] private bool _showGizmos = true;
    [SerializeField] private Color _gizmoColor = Color.yellow;
    [SerializeField] private int _gizmoFontSize = 12;
    private readonly List<(Vector2 position, GridCoord coord)> _debugTiles = new();
    
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