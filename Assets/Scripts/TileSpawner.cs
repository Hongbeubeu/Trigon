using UnityEngine;

public class TileSpawner : MonoBehaviour
{
	public Transform[] spawnZones;
	public Color[] colorPack1;
	public Color[] colorPack2;
	public Color[] colorPack3;
	public Color[] colorPack4;

	public Color[] colorPack;

	[SerializeField] private CompositeTile[] tilePrefabs;


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
			var tile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], spawnZones[i].position,
				Quaternion.identity);
			tile.tag = "Draggable";
			// BoxCollider2D collide = tile.gameObject.AddComponent<BoxCollider2D>();
			// collide.size = new Vector2(2.5f, 2.5f);
			var scale = new Vector2(0.45f, 0.45f);
			tile.rootScale = scale;
			tile.transform.localScale = scale;
			tile.id = i;
			GameManager.instance.tileOnSpawner.Add(i, tile);
			tile.transform.SetParent(spawnZones[i]);
			var randColor = colorPack[Random.Range(0, colorPack.Length)];
			tile.rootColor = randColor;
			tile.ChangeColorTile(randColor);
			tile.InitBaseTilePosition();
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