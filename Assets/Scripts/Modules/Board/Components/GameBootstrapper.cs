using UnityEngine;

/// <summary>
/// Executed first on Awake. Wires all pure logic and visual components
/// into the global ServiceLocator for dependency inversion.
/// </summary>
public class GameBootstrapper : MonoBehaviour
{
    [Header("Config Databases")]
    [SerializeField] private LogicConfig logicConfig;
    [SerializeField] private GameViewConfig gameViewConfig;

    [Header("Scene References")]
    [SerializeField] private BoardGenerator boardGenerator;

    private IDataService _dataService;
    private IBoardLogic _boardLogic;
    private ILineClearHandler _lineClearHandler;
    private IScoreService _scoreService;
    private ITileViewRegistry _viewRegistry;
    private GameStateController _stateController;

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

        _stateController = new GameStateController(_dataService, _scoreService);

        ServiceLocator.Register<IDataService>(_dataService);
        ServiceLocator.Register<IBoardLogic>(_boardLogic);
        ServiceLocator.Register<ITileViewRegistry>(_viewRegistry);
        ServiceLocator.Register<IScoreService>(_scoreService);
        ServiceLocator.Register<ILineClearHandler>(_lineClearHandler);
        ServiceLocator.Register(_stateController);

        boardGenerator.Generate(_dataService.Board, _viewRegistry, logicConfig, gameViewConfig);
        _viewRegistry.SyncWorldPositions(_dataService.Board);
    }

    private void Update()
    {
        _stateController?.Update();
    }
    
    private void OnDestroy()
    {
        _stateController?.Dispose();
        _viewRegistry?.Dispose();
        ServiceLocator.Unregister<ConfigService>();
        ServiceLocator.Unregister<IDataService>();
        ServiceLocator.Unregister<IBoardLogic>();
        ServiceLocator.Unregister<ITileViewRegistry>();
        ServiceLocator.Unregister<IScoreService>();
        ServiceLocator.Unregister<ILineClearHandler>();
        ServiceLocator.Unregister<GameStateController>();
    }
}
