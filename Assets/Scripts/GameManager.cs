using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Singleton

	static GameManager _instance;

	public static GameManager instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<GameManager>();
			return _instance;
		}
	}

	#endregion

	int score;
	BoardGenerator boardGenerator;
	TileSpawner spawner;
	Transform tileOnBoardZone;
	private int numberTileOnSpawnZone;
	public Dictionary<Vector3Int, BoardTile> boardTiles = new Dictionary<Vector3Int, BoardTile>();
	public Dictionary<Vector2, Vector3Int> positionToMatrix = new Dictionary<Vector2, Vector3Int>();
	Dictionary<int, List<Vector3Int>> dictionarySameCrossX = new Dictionary<int, List<Vector3Int>>();
	Dictionary<int, List<Vector3Int>> dictionarySameCrossY = new Dictionary<int, List<Vector3Int>>();
	Dictionary<int, List<Vector3Int>> dictionarySameCrossZ = new Dictionary<int, List<Vector3Int>>();
	Dictionary<Vector3Int, BaseTile> tilesOnBoard = new Dictionary<Vector3Int, BaseTile>();
	public Dictionary<int, CompositeTile> tileOnSpawner = new Dictionary<int, CompositeTile>();
	List<Vector2> tileAlreadyAdded = new List<Vector2>();
	List<List<Vector3Int>> crossToClears = new List<List<Vector3Int>>();
	List<int> crossXToCleared = new List<int>();
	List<int> crossYToCleared = new List<int>();
	List<int> crossZToCleared = new List<int>();
	public bool isPause;
	public bool isLose;

	public int NumberTileOnSpawnZone
	{
		get => numberTileOnSpawnZone;
		set
		{
			numberTileOnSpawnZone = value;
			if (value == 0)
			{
				spawner.RandomTile();
			}
			else if (value == 3)
				CheckLose();
		}
	}

	public int Score
	{
		get => score;
		set
		{
			score = value;
			UIManager.instance.SetCurrentScore(score);
		}
	}

	void ResetProperties()
	{
		tilesOnBoard.Clear();
		tileOnSpawner.Clear();
		tileAlreadyAdded.Clear();
		crossToClears.Clear();
		crossXToCleared.Clear();
		crossYToCleared.Clear();
		crossZToCleared.Clear();
		isPause = false;
		isLose = false;
		ResetBoardTiles();
	}

	void ResetBoardTiles()
	{
		foreach (var item in boardTiles)
		{
			item.Value.isContainsTile = false;
		}
	}

	private void Awake()
	{
		boardGenerator = FindObjectOfType<BoardGenerator>();
		tileOnBoardZone = GameObject.Find("Tiles On Board Zone").transform;
		spawner = FindObjectOfType<TileSpawner>();
		boardGenerator.GenerateBoard();
		boardGenerator.ScaleBoard();
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

	public void NewGame()
	{
		ResetProperties();
		spawner.RandomColorPack();
		spawner.ResetSpawnZone();
		spawner.RandomTile();
		LoadScore();
	}

	void LoadScore()
	{
		UIManager.instance.SetMaxScore(GetMaxScore());
		Score = 0;
	}

	public void InitBoardMapping()
	{
		foreach (var item in boardTiles)
		{
			int keyX = item.Key.x, keyY = item.Key.y, keyZ = item.Key.z;

			// Contain same x
			if (dictionarySameCrossX.ContainsKey(keyX))
			{
				List<Vector3Int> listSameX = dictionarySameCrossX[keyX];
				listSameX.Add(item.Key);
				dictionarySameCrossX[keyX] = listSameX;
			}
			else
			{
				List<Vector3Int> listSameX = new List<Vector3Int>();
				listSameX.Add(item.Key);
				dictionarySameCrossX[keyX] = listSameX;
			}

			// Contain same y
			if (dictionarySameCrossY.ContainsKey(keyY))
			{
				List<Vector3Int> listSameY = dictionarySameCrossY[keyY];
				listSameY.Add(item.Key);
				dictionarySameCrossY[keyY] = listSameY;
			}
			else
			{
				List<Vector3Int> listSameY = new List<Vector3Int>();
				listSameY.Add(item.Key);
				dictionarySameCrossY[keyY] = listSameY;
			}

			// contain same z
			if (dictionarySameCrossZ.ContainsKey(keyZ))
			{
				List<Vector3Int> listSameZ = dictionarySameCrossZ[keyZ];
				listSameZ.Add(item.Key);
				dictionarySameCrossZ[keyZ] = listSameZ;
			}
			else
			{
				List<Vector3Int> listSameZ = new List<Vector3Int>();
				listSameZ.Add(item.Key);
				dictionarySameCrossZ[keyZ] = listSameZ;
			}

			positionToMatrix[item.Value.transform.position] = item.Key;
		}
	}

	public void CheckSameCrossX(int x)
	{
		if (crossXToCleared.Contains(x))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in dictionarySameCrossX[x])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		crossToClears.Add(crossToClear);
		crossXToCleared.Add(x);
	}

	public void CheckSameCrossY(int y)
	{
		if (crossYToCleared.Contains(y))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in dictionarySameCrossY[y])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		crossToClears.Add(crossToClear);
		crossYToCleared.Add(y);
	}

	public void CheckSameCrossZ(int z)
	{
		if (crossZToCleared.Contains(z))
			return;
		List<Vector3Int> crossToClear = new List<Vector3Int>();

		foreach (var item in dictionarySameCrossZ[z])
		{
			if (!boardTiles[item].isContainsTile)
				return;
			crossToClear.Add(item);
		}

		crossToClears.Add(crossToClear);
		crossZToCleared.Add(z);
	}

	public void ClearCross()
	{
		FindCrossToClear();
		int gainedScore = 0;

		foreach (var crossToClear in crossToClears)
		{
			gainedScore += crossToClear.Count;
			StartCoroutine(ClearCrossCoroutine(crossToClear));
		}

		Timer.Schedule(this, 0.26f, CheckLose);

		gainedScore *= crossToClears.Count;
		Score += gainedScore;

		crossToClears.Clear();
		crossXToCleared.Clear();
		crossYToCleared.Clear();
		crossZToCleared.Clear();
	}

	IEnumerator ClearCrossCoroutine(List<Vector3Int> crossToClear)
	{
		foreach (var item in crossToClear)
		{
			boardTiles[item].isContainsTile = false;
			if (tilesOnBoard.ContainsKey(item))
				tilesOnBoard[item].DestroyAnim();
			tilesOnBoard.Remove(item);
			yield return new WaitForSeconds(0.01f);
		}
	}

	void FindCrossToClear()
	{
		foreach (var tilePos in tileAlreadyAdded)
		{
			CheckSameCrossX(positionToMatrix[tilePos].x);
			CheckSameCrossY(positionToMatrix[tilePos].y);
			CheckSameCrossZ(positionToMatrix[tilePos].z);
		}

		ResetTileAlreadyAdded();
	}

	public void CheckLose()
	{
		if (tileOnSpawner.Count == 0 || isLose)
			return;
		bool checkLose = true;
		foreach (var item in tileOnSpawner)
		{
			CompositeTile compositetTile = item.Value;
			TypeTile type = compositetTile.baseTiles[0].type;
			Vector2 res;
			bool itemCanPutDown = false;
			foreach (var boardTile in boardTiles)
			{
				if (!boardTile.Value.isContainsTile && type == boardTile.Value.type)
				{
					Vector2 firstPosition = boardTile.Value.transform.position;
					bool canPutDown = true;
					for (int i = 0; i < compositetTile.baseTilePosDistance.Count; i++)
					{
						res = CheckPosition(firstPosition + compositetTile.baseTilePosDistance[i],
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
			}

			if (!itemCanPutDown)
				item.Value.SetCanPutToBoard(false);
		}

		if (checkLose)
			LoseGame();
	}

	public Vector2 CheckPosition(Vector2 pos, TypeTile type)
	{
		foreach (var item in boardTiles)
		{
			BoardTile boardTile = boardTiles[item.Key];
			if (boardTile.type != type)
				continue;
			if (Mathf.Abs(boardTile.transform.position.x - pos.x) < 0.25f
			    && Mathf.Abs(boardTile.transform.position.y - pos.y) < 0.25f)
			{
				if (!boardTile.isContainsTile)
				{
					return boardTile.transform.position;
				}
			}
		}

		return new Vector2(-100, 0);
	}

	public void SetTileToBoard(BaseTile tile)
	{
		Vector2 pos = tile.transform.position;
		Vector2 correctPos = FindPosNearest(pos);
		tileAlreadyAdded.Add(correctPos);
		tilesOnBoard.Add(positionToMatrix[correctPos], tile);
		boardTiles[positionToMatrix[correctPos]].isContainsTile = true;
		tile.transform.SetParent(tileOnBoardZone);
	}

	public void ResetTileAlreadyAdded()
	{
		tileAlreadyAdded.Clear();
	}

	Vector2 FindPosNearest(Vector2 pos)
	{
		foreach (var item in positionToMatrix)
		{
			if (Mathf.Abs(item.Key.x - pos.x) < 0.01f && Mathf.Abs(item.Key.y - pos.y) < 0.01f)
				return item.Key;
		}

		return new Vector2(-100, 0);
	}

	public void PauseGame()
	{
		UIManager.instance.SetActivePanel(UIPanel.PAUSE);
	}

	public void ContinueGame()
	{
		UIManager.instance.SetActivePanel(UIPanel.PLAY);
	}

	public void LoseGame()
	{
		isLose = true;
		CheckMaxScore();
		UIManager.instance.SetActivePanel(UIPanel.LOSE);
		DisableTileOnLose();
	}

	void DisableTileOnLose()
	{
		foreach (var item in tilesOnBoard)
		{
			item.Value.SetLoseColor();
		}

		// CompositeTile[] compositeTiles = FindObjectsOfType<CompositeTile>();
		// foreach (var compositeTile in tileOnSpawner)
		// {
		// 	foreach (var baseTile in compositeTile.Value.baseTiles)
		// 	{
		// 		baseTile.SetLoseColor();
		// 	}
		// }
	}

	void DestroRestTiles()
	{
		foreach (var item in tilesOnBoard)
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

	void CheckMaxScore()
	{
		if (score > GetMaxScore())
		{
			PlayerPrefs.SetInt("Max Score", score);
		}
	}

	int GetMaxScore()
	{
		return PlayerPrefs.GetInt("Max Score", 0);
	}

	public void ReplayGame()
	{
		DestroRestTiles();
		CheckMaxScore();
		UIManager.instance.SetActivePanel(UIPanel.PLAY);
		NewGame();
	}

	private void OnDestroy()
	{
		StopAllCoroutines();
	}
}