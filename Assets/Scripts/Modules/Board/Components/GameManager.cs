using System.Collections;
using System.Collections.Generic;
using Lean.Pool;
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

    private DataService _dataService;
    private BoardLogic _boardLogic;
    private LineClearHandler _lineClearHandler;
    private ScoreService _scoreService;
    private TileViewRegistry _viewRegistry;
    private StateMachine<GameState> _stateMachine;

    public Transform TilesOnBoardZone => tilesOnBoardZone;

    private void Awake()
    {
        var configService = new ConfigService(logicConfig, gameViewConfig);
        ServiceLocator.Register(configService);

        Application.targetFrameRate = logicConfig.TargetFrameRate;

        _dataService = new DataService();
        var persistence = new PlayerPrefsScorePersistence(logicConfig.MaxScoreKey);
        _boardLogic = new BoardLogic(_dataService.Board, logicConfig.SnapThreshold, logicConfig.ExactMatchThreshold);
        _viewRegistry = new TileViewRegistry(logicConfig, gameViewConfig);
        _scoreService = new ScoreService(_dataService.Session, persistence);
        _lineClearHandler = new LineClearHandler(_boardLogic, _viewRegistry);

        InitStateMachine();

        ServiceLocator.Register(_dataService);
        ServiceLocator.Register(_boardLogic);
        ServiceLocator.Register(_viewRegistry);
        ServiceLocator.Register(this);

        boardGenerator.Generate(_dataService.Board, _viewRegistry, logicConfig, gameViewConfig);
        boardGenerator.ScaleBoard(gameViewConfig.BoardScale);
        _viewRegistry.SyncWorldPositions(_dataService.Board);
    }

    private void InitStateMachine()
    {
        _stateMachine = new StateMachine<GameState>();

        var context = new GameContext(
            _dataService.Session,
            _scoreService,
            _stateMachine);

        _stateMachine.RegisterState(GameState.Playing, new PlayingState(context));
        _stateMachine.RegisterState(GameState.Paused, new PausedState(context));
        _stateMachine.RegisterState(GameState.Lost, new LostState(context));
    }

    private void OnEnable()
    {
        GameEvents.OnPauseRequested += OnPauseRequested;
        GameEvents.OnResumeRequested += OnResumeRequested;
        GameEvents.OnReplayRequested += OnReplayRequested;
    }

    private void OnDisable()
    {
        GameEvents.OnPauseRequested -= OnPauseRequested;
        GameEvents.OnResumeRequested -= OnResumeRequested;
        GameEvents.OnReplayRequested -= OnReplayRequested;
    }

    private void Start()
    {
        _dataService.Board.BuildAxisMappings();
        StartNewGame();
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void OnPauseRequested()
    {
        if (_stateMachine.CurrentKey == GameState.Playing)
            _stateMachine.ChangeState(GameState.Paused);
    }

    private void OnResumeRequested()
    {
        _stateMachine.ChangeState(GameState.Playing);
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

        _stateMachine.ChangeState(GameState.Playing);
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

        int lineScore = _lineClearHandler.ClearCompletedLines(placedCoords, this);
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
        _viewRegistry.DestroyAllPlacedTileViews();
        DestroyAllCompositeTiles();
        _scoreService.SaveMaxScoreIfNeeded();
        StartNewGame();
    }

    private void CheckLose()
    {
        var spawnedTiles = _viewRegistry.SpawnedTiles;
        if (spawnedTiles.Count == 0 || _stateMachine.CurrentKey == GameState.Lost) return;

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
            _stateMachine.ChangeState(GameState.Lost);
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
            LeanPool.Despawn(tile);
        }
    }

    private void OnDestroy()
    {
        _viewRegistry.Dispose();
        ServiceLocator.Unregister<ConfigService>();
        ServiceLocator.Unregister<DataService>();
        ServiceLocator.Unregister<BoardLogic>();
        ServiceLocator.Unregister<TileViewRegistry>();
        ServiceLocator.Unregister<GameManager>();
        StopAllCoroutines();
    }
}
