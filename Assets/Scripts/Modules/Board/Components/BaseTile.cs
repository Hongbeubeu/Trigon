using UnityEngine;
using DG.Tweening;
using Lean.Pool;

/// <summary>
/// Simple logical representation of a single hexagonal piece
/// used to assemble larger CompositeTiles.
/// </summary>
public class BaseTile : MonoBehaviour
{
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

    public void DestroyAnim(float duration)
    {
        transform.DOScale(Vector2.zero, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }

    public void DespawnAnim(float duration)
    {
        transform.DOScale(Vector2.zero, duration)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.localScale = Vector3.one;
                LeanPool.Despawn(gameObject);
            });
    }

    public void Destroy()
    {
        Destroy(gameObject);
    }
}
