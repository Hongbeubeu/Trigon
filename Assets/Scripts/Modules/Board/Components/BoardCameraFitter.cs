using System.Collections.Generic;
using UnityEngine;

public class BoardCameraFitter : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private RectTransform _targetRect;
    private static readonly float Sqrt3 = Mathf.Sqrt(3f);

    public enum FitMode
    {
        FitInside,  // Ensures the board never exceeds targetRect (shrinks on shorter screens)
        FitWidth,   // Always fits horizontally (may overflow vertically on short screens)
        FitHeight   // Always fits vertically
    }

    [SerializeField] private FitMode _fitMode = FitMode.FitInside;

    public void FitCameraToBoard(IReadOnlyList<BoardTile> tiles, float tileWidth)
    {
        if (_camera == null || !_camera.orthographic || _targetRect == null) return;
        if (tiles == null || tiles.Count == 0) return;
        var min = new Vector2(float.MaxValue, float.MaxValue);
        var max = new Vector2(float.MinValue, float.MinValue);
        foreach (var tile in tiles)
        {
            if (tile.SpriteRenderer == null) continue;
            var bounds = tile.SpriteRenderer.bounds;
            if (bounds.min.x < min.x) min.x = bounds.min.x;
            if (bounds.max.x > max.x) max.x = bounds.max.x;
            if (bounds.min.y < min.y) min.y = bounds.min.y;
            if (bounds.max.y > max.y) max.y = bounds.max.y;
        }
        var boardWidth = max.x - min.x;
        var boardHeight = max.y - min.y;
        var boardCenterX = (min.x + max.x) / 2f;
        var boardCenterY = (min.y + max.y) / 2f;
        
        var corners = new Vector3[4];
        _targetRect.GetWorldCorners(corners);
        
        var canvas = _targetRect.GetComponentInParent<Canvas>();
        bool isOverlay = canvas != null && canvas.renderMode == RenderMode.ScreenSpaceOverlay;

        var screenMin = isOverlay ? (Vector2)corners[0] : RectTransformUtility.WorldToScreenPoint(_camera, corners[0]);
        var screenMax = isOverlay ? (Vector2)corners[2] : RectTransformUtility.WorldToScreenPoint(_camera, corners[2]);
        
        var vpMin = new Vector2(screenMin.x / Screen.width, screenMin.y / Screen.height);
        var vpMax = new Vector2(screenMax.x / Screen.width, screenMax.y / Screen.height);
        var vpWidth = vpMax.x - vpMin.x;
        var vpHeight = vpMax.y - vpMin.y;
        var vpCenterX = (vpMin.x + vpMax.x) / 2f;
        var vpCenterY = (vpMin.y + vpMax.y) / 2f;
        var aspect = _camera.aspect;
        
        var orthoFromWidth = boardWidth / (vpWidth * 2f * aspect);
        var orthoFromHeight = boardHeight / (vpHeight * 2f);
        
        float orthoSize = orthoFromWidth;
        switch (_fitMode)
        {
            case FitMode.FitInside:
                orthoSize = Mathf.Max(orthoFromWidth, orthoFromHeight);
                break;
            case FitMode.FitWidth:
                orthoSize = orthoFromWidth;
                break;
            case FitMode.FitHeight:
                orthoSize = orthoFromHeight;
                break;
        }
        
        var camX = boardCenterX - (2f * vpCenterX - 1f) * orthoSize * aspect;
        var camY = boardCenterY - (2f * vpCenterY - 1f) * orthoSize;
        _camera.orthographicSize = orthoSize;
        _camera.transform.position = new Vector3(camX, camY, _camera.transform.position.z);
    }
}