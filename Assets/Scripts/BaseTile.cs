using UnityEngine;
using DG.Tweening;

public class BaseTile : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    SpriteMask spriteMask;
    public TypeTile type;
    public Color loseColor;
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

    public void SetColor(Color color)
    {
        spriteRenderer.color = color;
    }

    public void SetLoseColor()
    {
        spriteRenderer.color = loseColor;
    }

    public void SetSortingOrder(int sortingOrder)
    {
        spriteRenderer.sortingOrder = sortingOrder;
        spriteMask.frontSortingOrder = sortingOrder;
    }

    public void DestroyAnim()
    {
        if (gameObject == null)
            return;
        sequence = DOTween.Sequence();
        transform.DOScale(Vector2.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() =>
        {
            sequence.Kill();
            Destroy(gameObject);
        });
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        sequence.Kill();
    }
}
