using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
	public BoardTile upTilePrefab;
	public BoardTile downTilePrefab;
	public Color boardColor;
	float deltaX = 0.576f;
	int row = 12;

	public void ScaleBoard()
	{
		transform.localScale = new Vector2(0.5f, 0.5f);
	}

	public void GenerateBoard()
	{
		float posX = 0f;
		float posY = 8f;
		var pos = new Vector2(posX, posY);
		for (int i = 0; i < row; i++)
		{
			int y;
			var x = y = i;
			var z = 0;
			int indexInRow = 0;
			for (int j = 0; j < 2 * i + 1; j++)
			{
				var tempTile = j % 2 == 0 ? upTilePrefab : downTilePrefab;

				if (x > 3 && y < row - 4 && z < row - 4)
				{
					InstantiateBoardTile(tempTile, pos, tempTile.type, x, y, z);
				}

				pos.x += deltaX;
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

			pos.x -= (2 * i + 1) * deltaX + deltaX;
			pos.y--;
		}
	}

	private void InstantiateBoardTile(BoardTile tempTile, Vector2 pos, TypeTile typeTile, int x, int y, int z)
	{
		var tile = Instantiate(tempTile, pos, Quaternion.identity);
		tile.SetProperties(typeTile, new Vector3(x, y, z));
		tile.transform.SetParent(transform);
		GameManager.instance.boardTiles[new Vector3Int(x, y, z)] = tile;
		tile.GetComponent<SpriteRenderer>().color = boardColor;
	}
}