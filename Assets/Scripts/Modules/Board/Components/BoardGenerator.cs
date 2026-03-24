using System.Collections.Generic;
using System.Linq;
using Lean.Pool;
using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _targetRect;
    private static readonly float Sqrt3 = Mathf.Sqrt(3f);
    private readonly List<(Vector2 position, GridCoord coord)> _debugTiles = new();
    private readonly List<BoardTile> _spawnedBoardTiles = new();
    private float _upYOffset;
    private float _downYOffset;

    public void Generate(BoardData boardData, TileViewRegistry viewRegistry, LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        DespawnAll();
        var rows = logicConfig.BoardRowCount;
        var cutOffLines = logicConfig.CutOffLines;
        var tileWidth = viewConfig.TileWidth;
        var position = Vector2.zero;
        _upYOffset = tileWidth * Sqrt3 / 6f;
        _downYOffset = tileWidth * Sqrt3 / 3f;
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
            position.y -= tileWidth * Sqrt3 / 2f;
        }
        ScaleBoard(viewConfig.BoardScale);
        viewRegistry.SyncWorldPositions(boardData);
        FitCameraToRect(tileWidth);
    }

    private void FitCameraToRect(float tileWidth)
    {
        if (!_camera.orthographic) return;

        // Board world-space bounds from actual visible tiles
        if (_debugTiles.Count == 0) return;
        var tileHalfWidth = tileWidth / 2f;
        var tileVertexOffset = tileWidth * Sqrt3 / 3f;
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        foreach (var (pos, _) in _debugTiles)
        {
            if (pos.x < min.x) min.x = pos.x;
            if (pos.x > max.x) max.x = pos.x;
            if (pos.y < min.y) min.y = pos.y;
            if (pos.y > max.y) max.y = pos.y;
        }
        min.x -= tileHalfWidth;
        max.x += tileHalfWidth;
        min.y -= tileVertexOffset;
        max.y += tileVertexOffset;
        var boardWidth = max.x - min.x;
        var boardHeight = max.y - min.y;
        var boardCenterX = (min.x + max.x) / 2f;
        var boardCenterY = (min.y + max.y) / 2f;

        // Rect screen-space bounds -> viewport (0..1)
        var corners = new Vector3[4];
        _targetRect.GetWorldCorners(corners);
        var screenMin = RectTransformUtility.WorldToScreenPoint(_camera, corners[0]);
        var screenMax = RectTransformUtility.WorldToScreenPoint(_camera, corners[2]);
        var vpMin = new Vector2(screenMin.x / Screen.width, screenMin.y / Screen.height);
        var vpMax = new Vector2(screenMax.x / Screen.width, screenMax.y / Screen.height);
        var vpWidth = vpMax.x - vpMin.x;
        var vpHeight = vpMax.y - vpMin.y;
        var vpCenterX = (vpMin.x + vpMax.x) / 2f;
        var vpCenterY = (vpMin.y + vpMax.y) / 2f;

        // Solve ortho size so board fits inside the rect's viewport region
        var aspect = _camera.aspect;
        var orthoFromWidth = boardWidth / (vpWidth * 2f * aspect);
        var orthoFromHeight = boardHeight / (vpHeight * 2f);
        var orthoSize = Mathf.Max(orthoFromWidth, orthoFromHeight);

        // Solve camera position so board center maps to rect center in viewport
        var camX = boardCenterX - (2f * vpCenterX - 1f) * orthoSize * aspect;
        var camY = boardCenterY - (2f * vpCenterY - 1f) * orthoSize;
        _camera.orthographicSize = orthoSize;
        _camera.transform.position = new Vector3(camX, camY, _camera.transform.position.z);
    }

    public void DespawnAll()
    {
        foreach (var tile in _spawnedBoardTiles.Where(tile => tile != null))
        {
            LeanPool.Despawn(tile);
        }
        _spawnedBoardTiles.Clear();
        _debugTiles.Clear();
    }

    private void ScaleBoard(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }

    private void InstantiateTile(BoardTile prefab, Vector2 position, bool isUpTile, GridCoord gridCoord, Color boardColor, BoardData boardData, TileViewRegistry viewRegistry)
    {
        var rotation = isUpTile ? Quaternion.identity : Quaternion.Euler(0f, 0f, 180f);
        var tileView = LeanPool.Spawn(prefab, position, rotation, transform);
        tileView.TypeTile = isUpTile ? TypeTile.Up : TypeTile.Down;
        tileView.SpriteRenderer.color = boardColor;
        var cellData = new TileCellData(gridCoord, new Position2D(position.x, position.y), prefab.TypeTile);
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