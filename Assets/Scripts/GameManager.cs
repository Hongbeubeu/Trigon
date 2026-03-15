using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const float LINE_CLEAR_SETTLE_DELAY = 0.26f;
    private const int TARGET_FRAME_RATE = 120;

    [SerializeField] private BoardGenerator boardGenerator;
    [SerializeField] private TileSpawner tileSpawner;
    [SerializeField] private Transform tilesOnBoardZone;

    private BoardState _boardState;
    private LineClearHandler _lineClearHandler;
    private ScoreService _scoreService;
    private readonly Dictionary<int, CompositeTile> _spawnedTiles = new();
    private int _tilesRemainingInSpawn;
    private GameState _currentState;

    public BoardState Board => _boardState;
    public Transform TilesOnBoardZone => tilesOnBoardZone;
    public GameState CurrentState => _currentState;

    private void Awake()
    {
        Application.targetFrameRate = TARGET_FRAME_RATE;

        _boardState = new BoardState();
        _lineClearHandler = new LineClearHandler(_boardState);
        _scoreService = new ScoreService();

        ServiceLocator.Register(this);
        ServiceLocator.Register(_boardState);

        boardGenerator.Generate(_boardState);
        boardGenerator.ScaleBoard();
    }

    private void Start()
    {
        _boardState.BuildPositionMapping();
        _lineClearHandler.BuildAxisMapping();
        StartNewGame();
    }

    private void Update()
    {
        if (_currentState == GameState.Lost) return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    private void StartNewGame()
    {
        _boardState.Reset();
        _spawnedTiles.Clear();
        _tilesRemainingInSpawn = 0;

        SetState(GameState.Playing);
        _scoreService.Reset();
        tileSpawner.ResetSpawnZones();
        tileSpawner.SpawnTiles();
    }

    public void RegisterSpawnedTile(int id, CompositeTile tile)
    {
        _spawnedTiles[id] = tile;
    }

    public void SetSpawnCount(int count)
    {
        _tilesRemainingInSpawn = count;
        CheckLose();
    }

    public void OnTilePlacedOnBoard(CompositeTile compositeTile, List<Vector3Int> placedCoords)
    {
        _scoreService.AddScore(placedCoords.Count);
        _spawnedTiles.Remove(compositeTile.Id);

        int lineScore = _lineClearHandler.ClearCompletedLines(placedCoords, this);
        _scoreService.AddScore(lineScore);

        StartCoroutine(DelayedCheckLose());

        _tilesRemainingInSpawn--;
        if (_tilesRemainingInSpawn <= 0)
        {
            tileSpawner.SpawnTiles();
        }
    }

    public void ReplayGame()
    {
        _boardState.DestroyAllPlacedTiles();
        DestroyAllCompositeTiles();
        _scoreService.SaveMaxScoreIfNeeded();
        StartNewGame();
    }

    private void TogglePause()
    {
        if (_currentState == GameState.Paused)
            SetState(GameState.Playing);
        else if (_currentState == GameState.Playing)
            SetState(GameState.Paused);
    }

    private void SetState(GameState state)
    {
        _currentState = state;
        GameEvents.RaiseGameStateChanged(state);
    }

    private void CheckLose()
    {
        if (_spawnedTiles.Count == 0 || _currentState == GameState.Lost) return;

        bool anyCanPlace = false;

        foreach (var kvp in _spawnedTiles)
        {
            bool canFit = _boardState.CanFitCompositeTile(kvp.Value);
            kvp.Value.SetPlaceable(canFit);

            if (canFit) anyCanPlace = true;
        }

        if (!anyCanPlace)
        {
            _scoreService.SaveMaxScoreIfNeeded();
            _boardState.SetAllTilesToLoseColor();
            SetState(GameState.Lost);
        }
    }

    private IEnumerator DelayedCheckLose()
    {
        yield return new WaitForSeconds(LINE_CLEAR_SETTLE_DELAY);
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
        ServiceLocator.Unregister<GameManager>();
        ServiceLocator.Unregister<BoardState>();
        StopAllCoroutines();
    }
}
