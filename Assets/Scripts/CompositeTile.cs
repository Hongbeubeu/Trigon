using System.Collections.Generic;
using UnityEngine;

public class CompositeTile : MonoBehaviour
{
    Vector2 rootPos;
    Vector2 rootScale;
    int topSortingOrder = 5;
    int rootSortingOrder = 2;

    List<BaseTile> baseTiles = new List<BaseTile>();

    private void Awake()
    {
        rootPos = transform.position;
        rootScale = transform.localScale;
        for (int i = 0; i < transform.childCount; i++)
        {
            BaseTile baseTile = transform.GetChild(i).gameObject.AddComponent<BaseTile>();
            baseTile.SetSortingOrder(rootSortingOrder);
            baseTiles.Add(baseTile);
        }
    }

    private void OnMouseDown()
    {
        transform.localScale = new Vector2(0.5f, 0.5f);
        SetSortingOrder(topSortingOrder);
    }

    private void OnMouseDrag()
    {
        Vector2 screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        transform.position = worldPos;
    }

    private void OnMouseUp()
    {
        transform.position = rootPos;
        transform.localScale = rootScale;
        SetSortingOrder(rootSortingOrder);
    }

    void SetSortingOrder(int sortingOrder)
    {
        for (int i = 0; i < baseTiles.Count; i++)
        {
            baseTiles[i].SetSortingOrder(sortingOrder);
        }
    }
}
