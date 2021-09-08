using UnityEngine;
using DG.Tweening;

public class BaseTile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    public TypeTile type;
    Sequence sequence;
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

    public void Destroy()
    {
        sequence = DOTween.Sequence();
        transform.DOScale(Vector2.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            sequence.Kill();
            Destroy(gameObject);
        });
    }
}
