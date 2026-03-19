using UnityEngine;

public class BoardGenerator : MonoBehaviour
{
    public void Generate(BoardData boardData, TileViewRegistry viewRegistry,
        LogicConfig logicConfig, GameViewConfig viewConfig)
    {
        int rowCount = logicConfig.BoardRowCount;
        int minAxis = logicConfig.BoardMinAxisValue;
        float tileWidth = viewConfig.TileWidth;
        var position = new Vector2(0f, viewConfig.BoardStartY);

        for (int row = 0; row < rowCount; row++)
        {
            int x = row, y = row, z = 0;
            int tilesInRow = 2 * row + 1;

            for (int col = 0; col < tilesInRow; col++)
            {
                bool isUpTile = col % 2 == 0;
                var prefab = isUpTile ? viewConfig.UpTilePrefab : viewConfig.DownTilePrefab;

                bool isWithinBounds = x > minAxis - 1 &&
                                      y < rowCount - minAxis &&
                                      z < rowCount - minAxis;

                if (isWithinBounds)
                {
                    InstantiateTile(prefab, position, x, y, z, viewConfig.BoardColor,
                        boardData, viewRegistry);
                }

                position.x += tileWidth;

                if (col % 2 == 0) y--;
                else z++;
            }

            position.x -= tilesInRow * tileWidth + tileWidth;
            position.y--;
        }
    }

    public void ScaleBoard(float scale)
    {
        transform.localScale = new Vector2(scale, scale);
    }

    private void InstantiateTile(BoardTile prefab, Vector2 position, int x, int y, int z,
        Color boardColor, BoardData boardData, TileViewRegistry viewRegistry)
    {
        var tileView = Instantiate(prefab, position, Quaternion.identity, transform);
        tileView.SpriteRenderer.color = boardColor;

        var coord = new GridCoord(x, y, z);
        var cellData = new TileCellData(coord, new Position2D(position.x, position.y), prefab.type);
        boardData.RegisterCell(cellData);
        viewRegistry.RegisterBoardTileView(coord, tileView);
    }
}
