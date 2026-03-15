using UnityEngine;
using DG.Tweening;

public class BaseTile : MonoBehaviour
{
    private const float DESTROY_ANIM_DURATION = 0.25f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color loseColor;

    public TypeTile type;

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
    }

    public void DestroyAnim()
    {
        transform.DOScale(Vector2.zero, DESTROY_ANIM_DURATION)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
