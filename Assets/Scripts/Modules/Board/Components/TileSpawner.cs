using Lean.Pool;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// Responsible for instantiating new randomized puzzle pieces into the player's spawn zones.
/// </summary>
public class TileSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnZones;
    [Tooltip("Ratio from the bottom of the screen to place the spawn zones (0 = bottom edge, 1 = top edge)")]
    [Range(0f, 1f)] [SerializeField] private float _bottomPaddingRatio = 0.15f;
    private ConfigService _configService;
    private GameManager _gameManager;
    private ICameraService _cameraService;
    private ConfigService ConfigService => _configService ??= ServiceLocator.Get<ConfigService>();
    private GameManager GameManager => _gameManager ??= ServiceLocator.Get<GameManager>();

    private void Start()
    {
        _cameraService = ServiceLocator.Get<ICameraService>();
        SyncSpawnZonesToCamera();
    }

    public void SpawnTiles()
    {
        SyncSpawnZonesToCamera();
        var viewConfig = ConfigService.GameView;
        var logicConfig = ConfigService.Logic;
        var palettes = viewConfig.ColorPalettes;
        var activePalette = palettes[Random.Range(0, palettes.Length)];
        
        var colliderSize = GetOptimalZoneSize();

        for (var i = 0; i < logicConfig.TilesPerSpawn; i++)
        {
            var prefabs = viewConfig.TilePrefabs;
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var tile = LeanPool.Spawn(prefab, _spawnZones[i].position, Quaternion.identity, _spawnZones[i]);
            tile.tag = Tags.Draggable;
            var scale = new Vector2(viewConfig.SpawnScale, viewConfig.SpawnScale);
            var color = activePalette.GetRandomColor();
            tile.Initialize(i, scale, color);
            tile.SetColliderSize(colliderSize);
            GameManager.RegisterSpawnedTile(i, tile);
        }
        GameManager.SetSpawnCount(logicConfig.TilesPerSpawn);
    }

    public void ResetSpawnZones()
    {
        foreach (var zone in _spawnZones)
        {
            for (var i = zone.childCount - 1; i >= 0; i--)
            {
                LeanPool.Despawn(zone.GetChild(i).gameObject);
            }
        }
    }

    private void SyncSpawnZonesToCamera()
    {
        var cam = _cameraService?.MainCamera;
        if (!cam || _spawnZones == null || _spawnZones.Length == 0) return;

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        float bottomY = cam.transform.position.y - cam.orthographicSize;
        float leftX = cam.transform.position.x - (screenWidth / 2f);

        float targetY = bottomY + (screenHeight * _bottomPaddingRatio);

        int count = _spawnZones.Length;
        float zoneWidth = screenWidth / count;
        float startX = leftX + (zoneWidth / 2f);

        for (int i = 0; i < count; i++)
        {
            if (_spawnZones[i] == null) continue;
            
            float targetX = startX + (i * zoneWidth);
            _spawnZones[i].position = new Vector3(targetX, targetY, 0f);
        }
    }

    private Vector2 GetOptimalZoneSize()
    {
        var cam = _cameraService?.MainCamera;
        if (!cam || _spawnZones == null || _spawnZones.Length == 0) return Vector2.zero;

        float screenHeight = cam.orthographicSize * 2f;
        float screenWidth = screenHeight * cam.aspect;

        int count = _spawnZones.Length;
        float availableWidthPerZone = screenWidth / count;
        float width = availableWidthPerZone * 0.85f;
        
        return new Vector2(width, width);
    }
}