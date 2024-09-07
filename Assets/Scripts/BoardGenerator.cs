using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
	public BoardTile upTilePrefab;
	public BoardTile downTilePrefab;
	public Color boardColor;
	private const float DELTA_X = 0.576f;
	private const int ROW = 12;

	public void ScaleBoard()
	{
		transform.localScale = new Vector2(0.5f, 0.5f);
	}

	public void GenerateBoard()
	{
		const float posX = 0f;
		const float posY = 8f;
		var pos = new Vector2(posX, posY);
		for (var i = 0; i < ROW; i++)
		{
			int y;
			var x = y = i;
			var z = 0;
			var indexInRow = 0;
			for (var j = 0; j < 2 * i + 1; j++)
			{
				var tempTile = j % 2 == 0 ? upTilePrefab : downTilePrefab;

				if (x > 3 && y < ROW - 4 && z < ROW - 4)
				{
					InstantiateBoardTile(tempTile, pos, tempTile.type, x, y, z);
				}

				pos.x += DELTA_X;
				indexInRow++;
				{
					if (indexInRow % 2 != 0)
					{
						y--;
					}

					if (indexInRow % 2 == 0)
					{
						z++;
					}
				}
			}

			pos.x -= (2 * i + 1) * DELTA_X + DELTA_X;
			pos.y--;
		}
	}

	private void InstantiateBoardTile(BoardTile tempTile, Vector2 pos, TypeTile typeTile, int x, int y, int z)
	{
		var tile = Instantiate(tempTile, pos, Quaternion.identity);
		tile.SetProperties(typeTile, new Vector3(x, y, z));
		tile.transform.SetParent(transform);
		GameManager.Instance.boardTiles[new Vector3Int(x, y, z)] = tile;
		tile.GetComponent<SpriteRenderer>().color = boardColor;
	}
}