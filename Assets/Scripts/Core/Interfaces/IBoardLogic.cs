using System.Collections.Generic;

public interface IBoardLogic
{
    Position2D FindNearestAvailablePosition(Position2D position, TypeTile type);
    bool IsInvalidPosition(Position2D position);
    GridCoord GetCoordAtPosition(Position2D position);
    GridCoord PlaceTile(Position2D tileWorldPosition);
    void RemoveTile(GridCoord coord);
    bool CanFitShape(List<GridCoord> gridOffsets, TypeTile rootType);
    List<List<GridCoord>> FindCompletedLines(List<GridCoord> recentCoords);
}
