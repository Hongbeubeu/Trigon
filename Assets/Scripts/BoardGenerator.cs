using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    private const float TILE_WIDTH = 0.576f;
    private const int ROW_COUNT = 12;
    private const float START_Y = 8f;
    private const int MIN_AXIS_VALUE = 4;

    [SerializeField] private BoardTile upTilePrefab;
    [SerializeField] private BoardTile downTilePrefab;
    [SerializeField] private Color boardColor;

    public void Generate(BoardData boardData, TileViewRegistry viewRegistry)
    {
        var position = new Vector2(0f, START_Y);

        for (int row = 0; row < ROW_COUNT; row++)
        {
            int x = row, y = row, z = 0;
            int tilesInRow = 2 * row + 1;

            for (int col = 0; col < tilesInRow; col++)
            {
                bool isUpTile = col % 2 == 0;
                var prefab = isUpTile ? upTilePrefab : downTilePrefab;

                bool isWithinBounds = x > MIN_AXIS_VALUE - 1 &&
                                      y < ROW_COUNT - MIN_AXIS_VALUE &&
                                      z < ROW_COUNT - MIN_AXIS_VALUE;

                if (isWithinBounds)
                {
                    InstantiateTile(prefab, position, x, y, z, boardData, viewRegistry);
                }

                position.x += TILE_WIDTH;

                if (col % 2 == 0) y--;
                else z++;
            }

            position.x -= tilesInRow * TILE_WIDTH + TILE_WIDTH;
            position.y--;
        }
    }

    public void ScaleBoard()
    {
        transform.localScale = new Vector2(0.5f, 0.5f);
    }

    private void InstantiateTile(BoardTile prefab, Vector2 position, int x, int y, int z,
        BoardData boardData, TileViewRegistry viewRegistry)
    {
        var tileView = Instantiate(prefab, position, Quaternion.identity, transform);
        tileView.GetComponent<SpriteRenderer>().color = boardColor;

        var coord = new Vector3Int(x, y, z);
        var cellData = new TileCellData(coord, position, prefab.type);
        boardData.RegisterCell(cellData);
        viewRegistry.RegisterBoardTileView(coord, tileView);
    }
}
