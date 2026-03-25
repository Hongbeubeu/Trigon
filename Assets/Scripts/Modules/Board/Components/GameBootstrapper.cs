using UnityEngine;

/// <summary>
/// Executed first on Awake. Wires all pure logic and visual components
/// into the global ServiceLocator for dependency inversion.
/// </summary>
public class GameBootstrapper : MonoBehaviour
{
    [Header("Config Databases")]
    [SerializeField] private AppConfig appConfig;
    [SerializeField] private LogicConfig logicConfig;
    [SerializeField] private GameViewConfig gameViewConfig;

    [Header("Scene References")]
    [SerializeField] private Camera mainCamera;
    [SerializeField] private BoardGenerator boardGenerator;

    private IDataService _dataService;
    private IBoardLogic _boardLogic;
    private ILineClearHandler _lineClearHandler;
    private IScoreService _scoreService;
    private ITileViewRegistry _viewRegistry;
    private GameStateController _stateController;
    private ICameraService _cameraService;

    private void Awake()
    {
        var configService = new ConfigService(appConfig, logicConfig, gameViewConfig);
        ServiceLocator.Register(configService);

        Application.targetFrameRate = appConfig.TargetFrameRate;

        _cameraService = new CameraService(mainCamera != null ? mainCamera : Camera.main);
        ServiceLocator.Register<ICameraService>(_cameraService);

        _dataService = new DataService();
        var persistence = new PlayerPrefsScorePersistence(appConfig.MaxScoreKey);
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
        ServiceLocator.Unregister<ICameraService>();
        ServiceLocator.Unregister<IDataService>();
        ServiceLocator.Unregister<IBoardLogic>();
        ServiceLocator.Unregister<ITileViewRegistry>();
        ServiceLocator.Unregister<IScoreService>();
        ServiceLocator.Unregister<ILineClearHandler>();
        ServiceLocator.Unregister<GameStateController>();
    }
}
