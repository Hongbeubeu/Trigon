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
    public int rowNumber = 12;
    BoardGenerator boardGenerator;
    TileSpawner spawner;
    public Dictionary<Vector3Int, BoardTile> matrixTiles = new Dictionary<Vector3Int, BoardTile>();
    public Dictionary<Vector2, Vector3Int> positionToMatrix = new Dictionary<Vector2, Vector3Int>();
    Dictionary<int, List<Vector3Int>> dictionarySameCrossX = new Dictionary<int, List<Vector3Int>>();
    Dictionary<int, List<Vector3Int>> dictionarySameCrossY = new Dictionary<int, List<Vector3Int>>();
    Dictionary<int, List<Vector3Int>> dictionarySameCrossZ = new Dictionary<int, List<Vector3Int>>();
    Dictionary<Vector3Int, BaseTile> tilesOnBoard = new Dictionary<Vector3Int, BaseTile>();
    public Dictionary<int, CompositeTile> tileOnSpawner = new Dictionary<int, CompositeTile>();
    Transform tileOnBoardZone;
    private int numberTileOnSpawnZone;
    List<Vector2> tileAlreadyAdded = new List<Vector2>();
    List<List<Vector3Int>> crossToClears = new List<List<Vector3Int>>();
    List<int> crossXToCleared = new List<int>();
    List<int> crossYToCleared = new List<int>();
    List<int> crossZToCleared = new List<int>();
    public bool isPause = false;
    public bool isLose = false;
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
            else
                CheckLose();
        }
    }
    private void Awake()
    {
        boardGenerator = FindObjectOfType<BoardGenerator>();
        tileOnBoardZone = GameObject.Find("Tiles On Board Zone").transform;
        spawner = FindObjectOfType<TileSpawner>();
        boardGenerator.GenerateBoard();
        boardGenerator.ScaleBoard();
    }

    private void Start()
    {
        spawner.RandomTile();
        InitBoardMapping();
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
                PlayGame();
            }
        }
    }

    public void InitBoardMapping()
    {
        foreach (var item in matrixTiles)
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
            if (!matrixTiles[item].isContainsTile)
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
            if (!matrixTiles[item].isContainsTile)
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
            if (!matrixTiles[item].isContainsTile)
                return;
            crossToClear.Add(item);
        }
        crossToClears.Add(crossToClear);
        crossZToCleared.Add(z);
    }

    public void ClearCross()
    {
        FindCrossToClear();
        foreach (var crossToClear in crossToClears)
        {
            StartCoroutine(ClearCrossCoroutine(crossToClear));
        }
        crossToClears.Clear();
        crossXToCleared.Clear();
        crossYToCleared.Clear();
        crossZToCleared.Clear();
    }

    IEnumerator ClearCrossCoroutine(List<Vector3Int> crossToClear)
    {
        foreach (var item in crossToClear)
        {
            matrixTiles[item].isContainsTile = false;
            if (tilesOnBoard.ContainsKey(item))
                tilesOnBoard[item].Destroy();
            tilesOnBoard.Remove(item);
            yield return new WaitForSeconds(0.01f);
        }
        CheckLose();
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
        if (tileOnSpawner.Count == 0)
            return;
        bool checkLose = true;
        foreach (var item in tileOnSpawner)
        {
            CompositeTile compositetTile = item.Value;
            TypeTile type = compositetTile.transform.GetChild(0).GetComponent<BaseTile>().type;
            Vector2 res;
            bool itemCanPutDown = false;
            foreach (var boardTile in matrixTiles)
            {
                if (!boardTile.Value.isContainsTile && type == boardTile.Value.type)
                {
                    Vector2 firstPosition = boardTile.Value.transform.position;
                    bool canPutDown = true;
                    for (int i = 0; i < compositetTile.baseTilePosDistance.Count; i++)
                    {
                        res = CheckPosition(firstPosition + compositetTile.baseTilePosDistance[i], compositetTile.baseTiles[i].type);
                        if (res.x + 100 == 0 && res.y == 0)
                        {
                            canPutDown = false;
                            break;
                        }
                    }
                    if (canPutDown)
                    {
                        checkLose = false;
                        itemCanPutDown = true;
                        item.Value.SetCanPutToBoard(true);
                        break;
                    }
                }
            }
            if (!itemCanPutDown)
                item.Value.SetCanPutToBoard(false);
        }
        if (checkLose)
            LoseGame();
        return;
    }

    public Vector2 CheckPosition(Vector2 pos, TypeTile type)
    {
        foreach (var item in matrixTiles)
        {
            BoardTile boardTile = matrixTiles[item.Key];
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

    public void SetTileToBoard(GameObject tile)
    {
        Vector2 pos = tile.transform.position;
        Vector2 correctPos = FindPosNearest(pos);
        tileAlreadyAdded.Add(correctPos);
        tilesOnBoard.Add(positionToMatrix[correctPos], tile.GetComponent<BaseTile>());
        matrixTiles[positionToMatrix[correctPos]].isContainsTile = true;
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
        AGameState[] compositeTiles = FindObjectsOfType<AGameState>();
        foreach (var compositeTile in compositeTiles)
        {
            compositeTile.gameObject.GetComponent<CompositeTile>().Pause();
        }
    }

    public void PlayGame()
    {
        UIManager.instance.SetActivePanel(UIPanel.PLAY);
        AGameState[] compositeTiles = FindObjectsOfType<AGameState>();
        foreach (var compositeTile in compositeTiles)
        {
            compositeTile.gameObject.GetComponent<CompositeTile>().Play();
        }
    }

    public void LoseGame()
    {
        isLose = true;
        UIManager.instance.SetActivePanel(UIPanel.LOSE);
    }

    public void RePlayGame()
    {

    }
}
