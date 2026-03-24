using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
using UnityEngine;

/// <summary>
/// The central routing component for active gameplay. Exclusively manages
/// checking lose conditions and broadcasting score/line clear events.
/// </summary>
public class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private TileSpawner tileSpawner;
    [SerializeField] private Transform tilesOnBoardZone;

    private IDataService _dataService;
    private IBoardLogic _boardLogic;
    private ILineClearHandler _lineClearHandler;
    private IScoreService _scoreService;
    private ITileViewRegistry _viewRegistry;
    private GameStateController _stateController;
    private ConfigService _configService;

    public Transform TilesOnBoardZone => tilesOnBoardZone;

    private void Start()
    {
        _dataService = ServiceLocator.Get<IDataService>();
        _boardLogic = ServiceLocator.Get<IBoardLogic>();
        _lineClearHandler = ServiceLocator.Get<ILineClearHandler>();
        _scoreService = ServiceLocator.Get<IScoreService>();
        _viewRegistry = ServiceLocator.Get<ITileViewRegistry>();
        _stateController = ServiceLocator.Get<GameStateController>();
        _configService = ServiceLocator.Get<ConfigService>();

        ServiceLocator.Register(this);

        _dataService.Board.BuildAxisMappings();
        StartNewGame();
    }

    private void OnEnable()
    {
        GameEvents.OnReplayRequested += OnReplayRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnReplayRequested -= OnReplayRequested;
    }

    private void OnReplayRequested()
    {
        ReplayGame();
    }

    private void StartNewGame()
    {
        _dataService.ResetSession();
        _viewRegistry.ClearSpawnedTiles();
        _viewRegistry.ClearPlacedTiles();

        _stateController.ChangeState(GameState.Playing);
        _scoreService.Reset();
        tileSpawner.ResetSpawnZones();
        tileSpawner.SpawnTiles();
    }

    public void RegisterSpawnedTile(int id, CompositeTile tile)
    {
        _viewRegistry.RegisterSpawnedTile(id, tile);
    }

    public void SetSpawnCount(int count)
    {
        _dataService.Session.TilesRemainingInSpawn = count;
        CheckLose();
    }

    public void OnTilePlacedOnBoard(CompositeTile compositeTile, List<GridCoord> placedCoords)
    {
        _scoreService.AddScore(placedCoords.Count);
        _viewRegistry.RemoveSpawnedTile(compositeTile.Id);

        var lineScore = _lineClearHandler.ClearCompletedLines(placedCoords, this);
        _scoreService.AddScore(lineScore);

        StartCoroutine(DelayedCheckLose());

        _dataService.Session.TilesRemainingInSpawn--;
        if (_dataService.Session.TilesRemainingInSpawn <= 0)
        {
            tileSpawner.SpawnTiles();
        }
    }

    private void ReplayGame()
    {
        _viewRegistry.DespawnAllPlacedTileViews();
        DestroyAllCompositeTiles();
        _scoreService.SaveMaxScoreIfNeeded();
        StartNewGame();
    }

    private void CheckLose()
    {
        var spawnedTiles = _viewRegistry.SpawnedTiles;
        if (spawnedTiles.Count == 0 || _stateController.CurrentState == GameState.Lost) return;

        var anyCanPlace = false;

        foreach (var kvp in spawnedTiles)
        {
            var compositeTile = kvp.Value;
            var rootType = compositeTile.BaseTiles[0].type;
            var canFit = _boardLogic.CanFitShape(compositeTile.GridOffsets, rootType);
            compositeTile.SetPlaceable(canFit);

            if (canFit) anyCanPlace = true;
        }

        if (!anyCanPlace)
        {
            _stateController.ChangeState(GameState.Lost);
        }
    }

    private IEnumerator DelayedCheckLose()
    {
        yield return new WaitForSeconds(_configService.Logic.LineClearSettleDelay);
        CheckLose();
    }

    private void DestroyAllCompositeTiles()
    {
        var compositeTiles = FindObjectsByType<CompositeTile>(FindObjectsSortMode.None);
        foreach (var tile in compositeTiles)
        {
            LeanPool.Despawn(tile);
        }
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<GameManager>();
        StopAllCoroutines();
    }
}
