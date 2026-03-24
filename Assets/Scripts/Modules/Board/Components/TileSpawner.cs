using Lean.Pool;
using UnityEngine;

/// <summary>
/// Responsible for instantiating new randomized puzzle pieces into the player's UI rack.
/// </summary>
public class TileSpawner : MonoBehaviour
{
    [SerializeField] private Transform[] _spawnZones;

    private ConfigService _configService;
    private GameManager _gameManager;
    private ConfigService ConfigRef => _configService ??= ServiceLocator.Get<ConfigService>();
    private GameManager GameManagerRef => _gameManager ??= ServiceLocator.Get<GameManager>();

    public void SpawnTiles()
    {
        var viewConfig = ConfigRef.GameView;
        var logicConfig = ConfigRef.Logic;
        var palettes = viewConfig.ColorPalettes;
        var activePalette = palettes[Random.Range(0, palettes.Length)];

        for (var i = 0; i < logicConfig.TilesPerSpawn; i++)
        {
            var prefabs = viewConfig.TilePrefabs;
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var tile = LeanPool.Spawn(prefab, _spawnZones[i].position, Quaternion.identity, _spawnZones[i]);
            tile.tag = Tags.Draggable;
            var scale = new Vector2(viewConfig.SpawnScale, viewConfig.SpawnScale);
            var color = activePalette.GetRandomColor();
            tile.Initialize(i, scale, color);

            GameManagerRef.RegisterSpawnedTile(i, tile);
        }

        GameManagerRef.SetSpawnCount(logicConfig.TilesPerSpawn);
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
}
