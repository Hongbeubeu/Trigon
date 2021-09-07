using System;
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
    Transform tileOnBoard;
    private int numberTileOnSpawnZone;
    List<Vector2> tileAlreadyAdded = new List<Vector2>();
    List<List<Vector3Int>> crossToClears = new List<List<Vector3Int>>();
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
        }
    }
    private void Awake()
    {
        boardGenerator = FindObjectOfType<BoardGenerator>();
        tileOnBoard = GameObject.Find("Tiles On Board Zone").transform;
        spawner = FindObjectOfType<TileSpawner>();
        boardGenerator.GenerateBoard();
    }
    private void Start()
    {
        spawner.RandomTile();
        InitBoardMapping();
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

        foreach (var item in positionToMatrix)
        {
            Debug.Log(item.Key);
        }
        Debug.Log("___");
    }

    public void CheckSameCrossX(int x)
    {
        List<Vector3Int> crossToClear = new List<Vector3Int>();

        foreach (var item in dictionarySameCrossX[x])
        {
            if (!matrixTiles[item].isContainsTile)
                return;
        }

        crossToClears.Add(crossToClear);
    }
    public void CheckSameCrossY(int y)
    {
        List<Vector3Int> crossToClear = new List<Vector3Int>();
        foreach (var item in dictionarySameCrossY[y])
        {
            if (!matrixTiles[item].isContainsTile)
                return;
        }
        crossToClears.Add(crossToClear);
    }
    public void CheckSameCrossZ(int z)
    {
        List<Vector3Int> crossToClear = new List<Vector3Int>();
        foreach (var item in dictionarySameCrossZ[z])
        {
            if (!matrixTiles[item].isContainsTile)
                return;
        }
        crossToClears.Add(crossToClear);
    }

    public void ClearCross()
    {
        Debug.Log("clear");
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

        int crossX = positionToMatrix[correctPos].x;
        int crossY = positionToMatrix[correctPos].y;
        int crossZ = positionToMatrix[correctPos].z;
        matrixTiles[positionToMatrix[correctPos]].isContainsTile = true;
        CheckSameCrossX(crossX);
        CheckSameCrossY(crossY);
        CheckSameCrossZ(crossZ);
        tile.transform.SetParent(tileOnBoard);
        Debug.Log("ee");
    }

    public void ResetTileTemp()
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
}
