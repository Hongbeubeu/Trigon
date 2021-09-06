using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public GameObject upTilePrefab;
    public GameObject downTilePrefab;
    float deltaX = 0.576f;
    int row = 12;
    private void Start()
    {
        GenerateBoard();
        transform.localScale = new Vector2(0.5f, 0.5f);
    }
    void GenerateBoard()
    {
        float posX = 0f;
        float posY = 8f;
        int x, y, z;
        Vector2 pos = new Vector2(posX, posY);
        GameObject tempTile;
        for (int i = 0; i < row; i++)
        {
            x = y = i;
            z = 0;
            int indexInRow = 0;
            for (int j = 0; j < 2 * i + 1; j++)
            {
                if (j % 2 == 0)
                {
                    tempTile = upTilePrefab;
                }
                else
                {
                    tempTile = downTilePrefab;
                }

                if (x > 3 && y < row - 4 && z < row - 4)
                {
                    GameObject tile = Instantiate(tempTile, pos, Quaternion.identity);
                    tile.transform.SetParent(this.transform);
                    Color c = new Color((x * 50) / 255f, (y * 50) / 255f, (z * 50) / 255f);
                    tile.GetComponent<SpriteRenderer>().color = c;
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
