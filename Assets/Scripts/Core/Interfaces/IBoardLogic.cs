using System.Collections.Generic;

public interface IBoardLogic
{
    Position2D FindNearestAvailablePosition(Position2D position, TileType tileType);
    bool IsInvalidPosition(Position2D position);
    GridCoord GetCoordAtPosition(Position2D position);
    GridCoord PlaceTile(Position2D tileWorldPosition);
    void RemoveTile(GridCoord coord);
    bool CanFitShape(List<GridCoord> gridOffsets, TileType rootTileType);
    List<List<GridCoord>> FindCompletedLines(List<GridCoord> recentCoords);
}
