using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    private const int TILES_PER_SPAWN = 3;
    private const float TILE_SCALE = 0.45f;

    [SerializeField] private Transform[] spawnZones;
    [SerializeField] private CompositeTile[] tilePrefabs;
    [SerializeField] private ColorPalette[] colorPalettes;

    private ColorPalette _activeColorPalette;
    private GameManager _gameManager;
    private GameManager GameManagerRef => _gameManager ??= ServiceLocator.Get<GameManager>();

    public void SpawnTiles()
    {
        SelectRandomPalette();

        for (int i = 0; i < TILES_PER_SPAWN; i++)
        {
            var prefab = tilePrefabs[Random.Range(0, tilePrefabs.Length)];
            var tile = Instantiate(prefab, spawnZones[i].position, Quaternion.identity, spawnZones[i]);
            tile.tag = "Draggable";

            var scale = new Vector2(TILE_SCALE, TILE_SCALE);
            var color = _activeColorPalette.GetRandomColor();
            tile.Initialize(i, scale, color);

            GameManagerRef.RegisterSpawnedTile(i, tile);
        }

        GameManagerRef.SetSpawnCount(TILES_PER_SPAWN);
    }

    public void ResetSpawnZones()
    {
        foreach (var zone in spawnZones)
        {
            for (int i = zone.childCount - 1; i >= 0; i--)
            {
                Destroy(zone.GetChild(i).gameObject);
            }
        }
    }

    private void SelectRandomPalette()
    {
        _activeColorPalette = colorPalettes[Random.Range(0, colorPalettes.Length)];
    }
}
