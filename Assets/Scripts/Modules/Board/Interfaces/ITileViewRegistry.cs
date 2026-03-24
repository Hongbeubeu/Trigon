using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileViewRegistry
{
    IReadOnlyDictionary<int, CompositeTile> SpawnedTiles { get; }
    void RegisterBoardTileView(GridCoord coord, BoardTile view);
    void SpawnPlacedTile(GridCoord coord, TypeTile type, Vector2 position, Color color, int sortingOrder, Transform parent);
    void RegisterSpawnedTile(int id, CompositeTile view);
    void RemoveSpawnedTile(int id);
    void AnimateRemovePlacedTile(GridCoord coord);
    IEnumerator AnimateClearLine(List<GridCoord> line, IBoardLogic boardLogic);
    void DespawnAllPlacedTileViews();
    void ClearSpawnedTiles();
    void ClearPlacedTiles();
    void ShowPlaceholder(List<GridCoord> coords, Color color);
    void ClearPlaceholder();
    void SyncWorldPositions(BoardData boardData);
    void Dispose();
}
