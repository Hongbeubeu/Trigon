using System.Collections.Generic;
using UnityEngine;

public class BoardCameraFitter : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _targetRect;
    private static readonly float Sqrt3 = Mathf.Sqrt(3f);

    public void FitCameraToBoard(IReadOnlyList<BoardTile> tiles, float tileWidth)
    {
        if (_camera == null || !_camera.orthographic || _targetRect == null) return;
        if (tiles == null || tiles.Count == 0) return;

        var tileHalfWidth = tileWidth / 2f;
        var tileVertexOffset = tileWidth * Sqrt3 / 3f;
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        
        foreach (var tile in tiles)
        {
            var pos = tile.transform.localPosition;
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

        var aspect = _camera.aspect;
        var orthoFromWidth = boardWidth / (vpWidth * 2f * aspect);
        var orthoFromHeight = boardHeight / (vpHeight * 2f);
        var orthoSize = Mathf.Max(orthoFromWidth, orthoFromHeight);

        var camX = boardCenterX - (2f * vpCenterX - 1f) * orthoSize * aspect;
        var camY = boardCenterY - (2f * vpCenterY - 1f) * orthoSize;
        _camera.orthographicSize = orthoSize;
        _camera.transform.position = new Vector3(camX, camY, _camera.transform.position.z);
    }
}
