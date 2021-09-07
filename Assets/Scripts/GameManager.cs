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

    public Dictionary<Vector3Int, BoardTile> matrixTiles = new Dictionary<Vector3Int, BoardTile>();
    public void ShowBoard()
    {
        foreach (var item in matrixTiles)
        {
            Debug.Log(item.Key + ": " + item.Value);
        }
    }
}
