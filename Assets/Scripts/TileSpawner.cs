using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public Transform[] spawnZones;
    public Color[] colorPack1;
    public Color[] colorPack2;
    public Color[] colorPack3;
    public Color[] colorPack4;

    public Color[] colorPack;

    string tilePrefabPath = "Prefabs/Composite Tiles";
    GameObject[] tilePrefabs;
    private void Awake()
    {
        tilePrefabs = Resources.LoadAll<GameObject>(tilePrefabPath);
    }

    public void RandomColorPack()
    {
        int randomPack = Random.Range(1, 5);
        switch (randomPack)
        {
            case 1:
                colorPack = colorPack1;
                break;
            case 2:
                colorPack = colorPack2;
                break;
            case 3:
                colorPack = colorPack3;
                break;
            case 4:
                colorPack = colorPack4;
                break;
            default:
                colorPack = colorPack1;
                break;
        }
    }

    public void RandomTile()
    {
        GameManager.instance.tileOnSpawner.Clear();
        for (int i = 0; i < 3; i++)
        {
            GameObject tile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], spawnZones[i].position, Quaternion.identity);
            tile.tag = "Draggable";
            BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2.5f, 2.5f);
            tile.transform.localScale = new Vector2(0.45f, 0.45f);
            CompositeTile compositeTile = tile.AddComponent<CompositeTile>();
            compositeTile.id = i;
            GameManager.instance.tileOnSpawner.Add(i, compositeTile);
            tile.transform.SetParent(spawnZones[i]);
            Color randColor = colorPack[Random.Range(0, colorPack.Length)];
            compositeTile.rootColor = randColor;
            for (int j = 0; j < tile.transform.childCount; j++)
            {
                tile.transform.GetChild(j).GetComponent<SpriteRenderer>().color = randColor;
            }
        }
        GameManager.instance.NumberTileOnSpawnZone = 3;
    }

    public void ResetSpawnZone()
    {
        foreach (var item in spawnZones)
        {
            int children = item.childCount;
            for (int i = 0; i < children; i++)
            {
                Destroy(item.GetChild(i).gameObject);
            }
        }
    }
}
