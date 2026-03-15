using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private BoardGenerator boardGenerator;
    [SerializeField] private TileSpawner tileSpawner;
    [SerializeField] private Transform tilesOnBoardZone;

    [Header("Config Databases")]
    [SerializeField] private LogicConfig logicConfig;
    [SerializeField] private GameViewConfig gameViewConfig;
    [SerializeField] private UIViewConfig uiViewConfig;

    private DataService _dataService;
    private BoardLogic _boardLogic;
    private LineClearHandler _lineClearHandler;
    private ScoreService _scoreService;
    private TileViewRegistry _viewRegistry;

    public Transform TilesOnBoardZone => tilesOnBoardZone;

    private void Awake()
    {
        var configService = new ConfigService(logicConfig, gameViewConfig, uiViewConfig);
        ServiceLocator.Register(configService);

        Application.targetFrameRate = logicConfig.TargetFrameRate;

        _dataService = new DataService();
        _boardLogic = new BoardLogic(_dataService.Board, logicConfig);
        _viewRegistry = new TileViewRegistry(logicConfig, gameViewConfig);
        _scoreService = new ScoreService(_dataService.Session, logicConfig);
        _lineClearHandler = new LineClearHandler(_boardLogic, _viewRegistry);

        ServiceLocator.Register(_dataService);
        ServiceLocator.Register(_boardLogic);
        ServiceLocator.Register(_viewRegistry);
        ServiceLocator.Register(this);

        boardGenerator.Generate(_dataService.Board, _viewRegistry, logicConfig, gameViewConfig);
        boardGenerator.ScaleBoard(gameViewConfig.BoardScale);
        _viewRegistry.SyncWorldPositions(_dataService.Board);
    }

    private void Start()
    {
        _dataService.Board.BuildAxisMappings();
        StartNewGame();
    }

    private void Update()
    {
        if (_dataService.Session.State == GameState.Lost) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void StartNewGame()
    {
        _dataService.ResetSession();
        _viewRegistry.ClearSpawnedTiles();
        _viewRegistry.ClearPlacedTiles();

        SetState(GameState.Playing);
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

    public void OnTilePlacedOnBoard(CompositeTile compositeTile, List<Vector3Int> placedCoords)
    {
        _scoreService.AddScore(placedCoords.Count);
        _viewRegistry.RemoveSpawnedTile(compositeTile.Id);

        int lineScore = _lineClearHandler.ClearCompletedLines(placedCoords, this);
        _scoreService.AddScore(lineScore);

        StartCoroutine(DelayedCheckLose());

        _dataService.Session.TilesRemainingInSpawn--;
        if (_dataService.Session.TilesRemainingInSpawn <= 0)
        {
            tileSpawner.SpawnTiles();
        }
    }

    public void ReplayGame()
    {
        _viewRegistry.DestroyAllPlacedTileViews();
        DestroyAllCompositeTiles();
        _scoreService.SaveMaxScoreIfNeeded();
        StartNewGame();
    }

    private void TogglePause()
    {
        var session = _dataService.Session;
        if (session.State == GameState.Paused)
            SetState(GameState.Playing);
        else if (session.State == GameState.Playing)
            SetState(GameState.Paused);
    }

    private void SetState(GameState state)
    {
        _dataService.Session.State = state;
        GameEvents.RaiseGameStateChanged(state);
    }

    private void CheckLose()
    {
        var spawnedTiles = _viewRegistry.SpawnedTiles;
        if (spawnedTiles.Count == 0 || _dataService.Session.State == GameState.Lost) return;

        bool anyCanPlace = false;

        foreach (var kvp in spawnedTiles)
        {
            var composite = kvp.Value;
            var types = new List<TypeTile>();
            foreach (var baseTile in composite.BaseTiles)
            {
                types.Add(baseTile.type);
            }

            bool canFit = _boardLogic.CanFitShape(composite.TileOffsets, types);
            composite.SetPlaceable(canFit);

            if (canFit) anyCanPlace = true;
        }

        if (!anyCanPlace)
        {
            _scoreService.SaveMaxScoreIfNeeded();
            _viewRegistry.SetAllPlacedTilesToLoseColor();
            SetState(GameState.Lost);
        }
    }

    private IEnumerator DelayedCheckLose()
    {
        yield return new WaitForSeconds(logicConfig.LineClearSettleDelay);
        CheckLose();
    }

    private void DestroyAllCompositeTiles()
    {
        var compositeTiles = FindObjectsByType<CompositeTile>(FindObjectsSortMode.None);
        foreach (var tile in compositeTiles)
        {
            tile.DestroyTile();
        }
    }

    private void OnDestroy()
    {
        ServiceLocator.Unregister<ConfigService>();
        ServiceLocator.Unregister<DataService>();
        ServiceLocator.Unregister<BoardLogic>();
        ServiceLocator.Unregister<TileViewRegistry>();
        ServiceLocator.Unregister<GameManager>();
        StopAllCoroutines();
    }
}
