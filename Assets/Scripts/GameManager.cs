using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Singleton

	private static GameManager _instance;

	public static GameManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<GameManager>();
			return _instance;
		}
	}

	#endregion

	private int _score;
	private BoardGenerator _boardGenerator;
	private TileSpawner _spawner;
	private Transform _tileOnBoardZone;
	private int _numberTileOnSpawnZone;
	public readonly Dictionary<Vector3Int, BoardTile> boardTiles = new();
	private readonly Dictionary<Vector2, Vector3Int> _positionToMatrix = new();
	private readonly Dictionary<int, List<Vector3Int>> _dictionarySameCrossX = new();
	private readonly Dictionary<int, List<Vector3Int>> _dictionarySameCrossY = new();
	private readonly Dictionary<int, List<Vector3Int>> _dictionarySameCrossZ = new();
	private readonly Dictionary<Vector3Int, BaseTile> _tilesOnBoard = new();
	public readonly Dictionary<int, CompositeTile> tileOnSpawner = new();
	private readonly List<Vector2> _tileAlreadyAdded = new();
	private readonly List<List<Vector3Int>> _crossToClears = new();
	private readonly List<int> _crossXToCleared = new();
	private readonly List<int> _crossYToCleared = new();
	private readonly List<int> _crossZToCleared = new();
	public bool isPause;
	public bool isLose;

	public int NumberTileOnSpawnZone
	{
		get => _numberTileOnSpawnZone;
		set
		{
			_numberTileOnSpawnZone = value;
			if (value == 0)
			{
				_spawner.RandomTile();
			}
			else if (value == 3)
				CheckLose();
		}
	}

	public int Score
	{
		get => _score;
		set
		{
			_score = value;
			UIManager.Instance.SetCurrentScore(_score);
		}
	}

	private void ResetProperties()
	{
		_tilesOnBoard.Clear();
		tileOnSpawner.Clear();
		_tileAlreadyAdded.Clear();
		_crossToClears.Clear();
		_crossXToCleared.Clear();
		_crossYToCleared.Clear();
		_crossZToCleared.Clear();
		isPause = false;
		isLose = false;
		ResetBoardTiles();
	}

	private void ResetBoardTiles()
	{
		foreach (var item in boardTiles)
		{
			item.Value.isContainsTile = false;
		}
	}

	private void Awake()
	{
		_boardGenerator = FindObjectOfType<BoardGenerator>();
		_tileOnBoardZone = GameObject.Find("Tiles On Board Zone").transform;
		_spawner = FindObjectOfType<TileSpawner>();
		_boardGenerator.GenerateBoard();
		_boardGenerator.ScaleBoard();
		Application.targetFrameRate = 120;
	}

	private void OnEnable()
	{
		ResetProperties();
	}

	private void Start()
	{
		InitBoardMapping();
		NewGame();
	}

	private void Update()
	{
		if (isLose)
			return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!isPause)
			{
				isPause = true;
				PauseGame();
			}
			else
			{
				isPause = false;
				ContinueGame();
			}
		}
	}

	private void NewGame()
	{
		ResetProperties();
		_spawner.RandomColorPack();
		_spawner.ResetSpawnZone();
		_spawner.RandomTile();
		LoadScore();
	}

	private void LoadScore()
	{
		UIManager.Instance.SetMaxScore(GetMaxScore());
		Score = 0;
	}

	private void InitBoardMapping()
	{
		foreach (var item in boardTiles)
		{
			int keyX = item.Key.x, keyY = item.Key.y, keyZ = item.Key.z;

			// Contain same x
			if (_dictionarySameCrossX.ContainsKey(keyX))
			{
				List<Vector3Int> listSameX = _dictionarySameCrossX[keyX];
				listSameX.Add(item.Key);
				_dictionarySameCrossX[keyX] = listSameX;
			}
			else
			{
				List<Vector3Int> listSameX = new List<Vector3Int>();
				listSameX.Add(item.Key);
				_dictionarySameCrossX[keyX] = listSameX;
			}

			// Contain same y
			if (_dictionarySameCrossY.ContainsKey(keyY))
			{
				List<Vector3Int> listSameY = _dictionarySameCrossY[keyY];
				listSameY.Add(item.Key);
				_dictionarySameCrossY[keyY] = listSameY;
			}
			else
			{
				List<Vector3Int> listSameY = new List<Vector3Int>();
				listSameY.Add(item.Key);
				_dictionarySameCrossY[keyY] = listSameY;
			}

			// contain same z
			if (_dictionarySameCrossZ.ContainsKey(keyZ))
			{
				List<Vector3Int> listSameZ = _dictionarySameCrossZ[keyZ];
				listSameZ.Add(item.Key);
				_dictionarySameCrossZ[keyZ] = listSameZ;
			}
			else
			{
				List<Vector3Int> listSameZ = new List<Vector3Int>();
				listSameZ.Add(item.Key);
				_dictionarySameCrossZ[keyZ] = listSameZ;
			}

			_positionToMatrix[item.Value.transform.position] = item.Key;
		}
	}

	private void CheckSameCrossX(int x)
	{
		if (_crossXToCleared.Contains(x))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in _dictionarySameCrossX[x])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		_crossToClears.Add(crossToClear);
		_crossXToCleared.Add(x);
	}

	private void CheckSameCrossY(int y)
	{
		if (_crossYToCleared.Contains(y))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in _dictionarySameCrossY[y])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		_crossToClears.Add(crossToClear);
		_crossYToCleared.Add(y);
	}

	private void CheckSameCrossZ(int z)
	{
		if (_crossZToCleared.Contains(z))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in _dictionarySameCrossZ[z])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		_crossToClears.Add(crossToClear);
		_crossZToCleared.Add(z);
	}

	public void ClearCross()
	{
		FindCrossToClear();
		int gainedScore = 0;

		foreach (var crossToClear in _crossToClears)
		{
			gainedScore += crossToClear.Count;
			StartCoroutine(ClearCrossCoroutine(crossToClear));
		}

		Timer.Schedule(this, 0.26f, CheckLose);

		gainedScore *= _crossToClears.Count;
		Score += gainedScore;

		_crossToClears.Clear();
		_crossXToCleared.Clear();
		_crossYToCleared.Clear();
		_crossZToCleared.Clear();
	}

	private IEnumerator ClearCrossCoroutine(List<Vector3Int> crossToClear)
	{
		foreach (var item in crossToClear)
		{
			boardTiles[item].isContainsTile = false;
			if (_tilesOnBoard.TryGetValue(item, value: out var value))
				value.DestroyAnim();
			_tilesOnBoard.Remove(item);
			yield return new WaitForSeconds(0.01f);
		}
	}

	private void FindCrossToClear()
	{
		foreach (var tilePos in _tileAlreadyAdded)
		{
			CheckSameCrossX(_positionToMatrix[tilePos].x);
			CheckSameCrossY(_positionToMatrix[tilePos].y);
			CheckSameCrossZ(_positionToMatrix[tilePos].z);
		}

		ResetTileAlreadyAdded();
	}

	private void CheckLose()
	{
		if (tileOnSpawner.Count == 0 || isLose)
			return;
		var checkLose = true;
		foreach (var item in tileOnSpawner)
		{
			var compositetTile = item.Value;
			var type = compositetTile.baseTiles[0].type;
			var itemCanPutDown = false;
			foreach (var boardTile in boardTiles)
			{
				if (boardTile.Value.isContainsTile || type != boardTile.Value.type) continue;
				Vector2 firstPosition = boardTile.Value.transform.position;
				var canPutDown = true;
				for (var i = 0; i < compositetTile.baseTilePosDistance.Count; i++)
				{
					var res = FindNearestTilePosition(firstPosition + compositetTile.baseTilePosDistance[i],
						compositetTile.baseTiles[i].type);
					if (res.x + 100 != 0 || res.y != 0) continue;
					canPutDown = false;
					break;
				}

				if (!canPutDown) continue;
				checkLose = false;
				itemCanPutDown = true;
				item.Value.SetCanPutToBoard(true);
				break;
			}

			if (!itemCanPutDown)
				item.Value.SetCanPutToBoard(false);
		}

		if (checkLose)
			LoseGame();
	}

	public Vector2 FindNearestTilePosition(Vector2 pos, TypeTile type)
	{
		foreach (var boardTile in from item in boardTiles
		                          select boardTiles[item.Key]
		                          into boardTile
		                          where boardTile.type == type
		                          where Mathf.Abs(boardTile.transform.position.x - pos.x) < 0.25f
		                             && Mathf.Abs(boardTile.transform.position.y - pos.y) < 0.25f
		                          where !boardTile.isContainsTile
		                          select boardTile)
		{
			return boardTile.transform.position;
		}

		return new Vector2(-100, 0);
	}

	public void SetTileToBoard(BaseTile tile)
	{
		Vector2 pos = tile.transform.position;
		var correctPos = FindPosNearest(pos);
		_tileAlreadyAdded.Add(correctPos);
		_tilesOnBoard.Add(_positionToMatrix[correctPos], tile);
		boardTiles[_positionToMatrix[correctPos]].isContainsTile = true;
		tile.transform.SetParent(_tileOnBoardZone);
	}

	private void ResetTileAlreadyAdded()
	{
		_tileAlreadyAdded.Clear();
	}

	private Vector2 FindPosNearest(Vector2 pos)
	{
		foreach (var item in _positionToMatrix.Where(item =>
			         Mathf.Abs(item.Key.x - pos.x) < 0.01f && Mathf.Abs(item.Key.y - pos.y) < 0.01f))
		{
			return item.Key;
		}

		return new Vector2(-100, 0);
	}

	public void PauseGame()
	{
		UIManager.Instance.SetActivePanel(UIPanel.PAUSE);
	}

	public void ContinueGame()
	{
		UIManager.Instance.SetActivePanel(UIPanel.PLAY);
	}

	private void LoseGame()
	{
		isLose = true;
		CheckMaxScore();
		UIManager.Instance.SetActivePanel(UIPanel.LOSE);
		DisableTileOnLose();
	}

	private void DisableTileOnLose()
	{
		foreach (var item in _tilesOnBoard)
		{
			item.Value.SetLoseColor();
		}
	}

	private void DestroyRestTiles()
	{
		foreach (var item in _tilesOnBoard)
		{
			if (item.Value != null)
				item.Value.Destroy();
		}

		CompositeTile[] compositeTiles = FindObjectsOfType<CompositeTile>();
		foreach (var compositeTile in compositeTiles)
		{
			compositeTile.Destroy();
		}
	}

	private void CheckMaxScore()
	{
		if (_score > GetMaxScore())
		{
			PlayerPrefs.SetInt("Max Score", _score);
		}
	}

	private static int GetMaxScore()
	{
		return PlayerPrefs.GetInt("Max Score", 0);
	}

	public void ReplayGame()
	{
		DestroyRestTiles();
		CheckMaxScore();
		UIManager.Instance.SetActivePanel(UIPanel.PLAY);
		NewGame();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}