using System.Collections.Generic;
using UnityEngine;

public class CompositeTile : AGameState
{
    public int id;
    Vector2 rootPos;
    Vector2 rootScale;
    public Color rootColor;
    Color loseColor;
    int topSortingOrder = 5;
    int rootSortingOrder = 2;
    public List<BaseTile> baseTiles = new List<BaseTile>();
    public List<Vector2> baseTilePosDistance = new List<Vector2>();
    public bool isPause = false;
    public bool canPutToBoard = true;
    private void Awake()
    {
        rootPos = transform.position;
        rootScale = transform.localScale;
        loseColor = new Color(176f / 255f, 176f / 255f, 176 / 255f, 1);
        for (int i = 0; i < transform.childCount; i++)
        {
            BaseTile baseTile = transform.GetChild(i).gameObject.AddComponent<BaseTile>();
            baseTile.SetSortingOrder(rootSortingOrder);
            baseTile.loseColor = loseColor;
            baseTiles.Add(baseTile);
        }

        InitBaseTilePosition();
    }

    void InitBaseTilePosition()
    {
        Vector2 rootPoint = baseTiles[0].transform.position;
        baseTilePosDistance.Add(Vector2.zero);
        for (int i = 1; i < baseTiles.Count; i++)
        {
            Vector2 currentPoint = baseTiles[i].transform.position;
            Vector2 distanceVector = currentPoint - rootPoint;
            baseTilePosDistance.Add(distanceVector);
        }
    }

#if UNITY_EDITOR
    private void OnMouseDown()
    {
        if (isPause || !canPutToBoard)
            return;
        SetScaleOnPickUp();
    }

    private void OnMouseDrag()
    {
        if (isPause || !canPutToBoard)
            return;
        Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        if (isPause || !canPutToBoard)
            return;
        CheckValidPositionToPutTilesDown();
    }
#endif

    public void SetScaleOnPickUp()
    {
        transform.localScale = new Vector2(0.5f, 0.5f);
        SetSortingOrder(topSortingOrder);
    }

    public void ResetPosition()
    {
        transform.position = rootPos;
        transform.localScale = rootScale;
        SetSortingOrder(rootSortingOrder);
    }

    public void CheckValidPositionToPutTilesDown()
    {
        List<Vector2> desPos = new List<Vector2>();
        Vector2 currentFirstPoint = baseTiles[0].transform.position;
        Vector2 res;
        for (int i = 0; i < baseTilePosDistance.Count; i++)
        {
            res = GameManager.instance.CheckPosition(currentFirstPoint + baseTilePosDistance[i], baseTiles[i].type);
            if (res.x + 100 == 0 && res.y == 0)
            {
                ResetPosition();
                return;
            }
            else
            {
                desPos.Add(res);
            }
            if (i == 0)
                currentFirstPoint = res;
        }

        for (int i = 0; i < baseTiles.Count; i++)
        {
            baseTiles[i].transform.position = desPos[i];
            GameManager.instance.SetTileToBoard(baseTiles[i].gameObject);
        }
        GameManager.instance.Score += baseTiles.Count;
        GameManager.instance.tileOnSpawner.Remove(id);
        SetSortingOrder(rootSortingOrder);
        GameManager.instance.ClearCross();
        GameManager.instance.NumberTileOnSpawnZone--;
        Destroy(gameObject);
    }

    public void SetCanPutToBoard(bool canPut)
    {
        if (canPutToBoard == canPut)
            return;
        canPutToBoard = canPut;
        Color tempColor;
        if (canPut)
            tempColor = rootColor;
        else
            tempColor = loseColor;
        foreach (var item in baseTiles)
        {
            item.SetColor(tempColor);
        }
    }

    void SetSortingOrder(int sortingOrder)
    {
        for (int i = 0; i < baseTiles.Count; i++)
        {
            baseTiles[i].SetSortingOrder(sortingOrder);
        }
    }

    public void Play()
    {
        isPause = false;
    }

    public void Pause()
    {
        isPause = true;
    }

    public void Destroy()
    {
        foreach (var item in baseTiles)
        {
            if (item != null)
                item.Destroy();
        }
    }
}
