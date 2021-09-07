using UnityEngine;

public class BaseTile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    public TypeTile type;
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteMask = GetComponent<SpriteMask>();
        string name = gameObject.name;
        if (name.Contains("Up"))
        {
            type = TypeTile.UP;
        }
        else
            type = TypeTile.DOWN;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        spriteRenderer.sortingOrder = sortingOrder;
        spriteMask.frontSortingOrder = sortingOrder;
    }
}
