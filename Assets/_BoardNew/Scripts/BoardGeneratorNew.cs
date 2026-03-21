using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SampleNew
{
    public class BoardGeneratorNew : MonoBehaviour
    {
        private static readonly float Sqrt3 = Mathf.Sqrt(3f);
        [SerializeField] private GameObject _cellPrefab;
        [SerializeField] private int _rows;
        [SerializeField] private int _cutOffLines;
        [SerializeField] private float _tileWidth;
        [SerializeField] private Camera _camera;
        [SerializeField] private RectTransform _targetRect;
        private float _upYOffset;
        private float _downYOffset;
        private readonly List<(Vector3 position, GridCoord coord)> _tileGizmos = new();

        private void Start()
        {
            _upYOffset = _tileWidth * Sqrt3 / 6f;
            _downYOffset = _tileWidth * Sqrt3 / 3f;
            StartCoroutine(Generate());
        }

        private IEnumerator Generate()
        {
            yield return null;
            var position = new Vector2(0f, 0f);
            for (var row = 0; row < _rows; row++)
            {
                var gridCoord = new GridCoord(row, row, 0);
                var columns = row * 2 + 1;
                position.x = -(columns - 1) * _tileWidth / 4f;
                for (var column = 0; column < columns; column++)
                {
                    var isUpTile = column % 2 == 0;
                    var offset = isUpTile ? _upYOffset : _downYOffset;
                    position.y += offset;
                    var isWithinBounds = gridCoord.x > _cutOffLines - 1 &&
                                         gridCoord.y < _rows - _cutOffLines &&
                                         gridCoord.z < _rows - _cutOffLines;
                    if (isWithinBounds)
                    {
                        Instantiate(_cellPrefab, position, Quaternion.Euler(0, 0, isUpTile ? 0 : 180f), transform);
                        _tileGizmos.Add((position, gridCoord));
                    }
                    position.y -= offset;
                    position.x += _tileWidth / 2f;
                    if (column % 2 == 0)
                        gridCoord.y--;
                    else
                        gridCoord.z++;
                }
                position.y -= _tileWidth * Sqrt3 / 2f;
            }
            FitCameraToRect();
        }

        private void FitCameraToRect()
        {
            if (!_camera.orthographic) return;

            // Board world-space bounds from actual visible tiles
            if (_tileGizmos.Count == 0) return;
            var tileHalfWidth = _tileWidth / 2f;
            var tileVertexOffset = _tileWidth * Sqrt3 / 3f;
            var min = new Vector2(float.MaxValue, float.MaxValue);
            var max = new Vector2(float.MinValue, float.MinValue);
            foreach (var (pos, _) in _tileGizmos)
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

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (_tileGizmos.Count == 0) return;
            var style = new GUIStyle
                        {
                            fontSize = 50,
                            alignment = TextAnchor.MiddleCenter,
                            normal = { textColor = Color.black }
                        };
            foreach (var (position, coord) in _tileGizmos)
            {
                UnityEditor.Handles.Label(position, coord.ToString(), style);
            }
        }
#endif
    }
}