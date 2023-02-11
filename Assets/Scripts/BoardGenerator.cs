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
		int x, y, z;
		Vector2 pos = new Vector2(posX, posY);
		BoardTile tempTile;
		for (int i = 0; i < row; i++)
		{
			x = y = i;
			z = 0;
			int indexInRow = 0;
			TypeTile typeTile;
			for (int j = 0; j < 2 * i + 1; j++)
			{
				if (j % 2 == 0)
				{
					tempTile = upTilePrefab;
					typeTile = TypeTile.UP;
				}
				else
				{
					tempTile = downTilePrefab;
					typeTile = TypeTile.DOWN;
				}

				if (x > 3 && y < row - 4 && z < row - 4)
				{
					BoardTile tile = Instantiate(tempTile, pos, Quaternion.identity);
					// BoardTile boardTile = tile.AddComponent<BoardTile>();
					tile.SetProperties(typeTile, new Vector3(x, y, z));
					tile.transform.SetParent(this.transform);
					GameManager.instance.boardTiles[new Vector3Int(x, y, z)] = tile;
					tile.GetComponent<SpriteRenderer>().color = boardColor;
					// SpriteMask mask = tile.GetComponent<SpriteMask>();
					// int so = tile.GetComponent<SpriteRenderer>().sortingOrder;
					// mask.isCustomRangeActive = true;
					// mask.frontSortingOrder = so;
					// mask.backSortingOrder = -1;
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

			x++;
			y = x;
			pos.x -= ((2 * i + 1) * deltaX + deltaX);
			pos.y--;
		}
	}
}