using UnityEngine;
using DG.Tweening;

public class BaseTile : MonoBehaviour
{
	[SerializeField] private SpriteRenderer spriteRenderer;
	public TypeTile type;
	public Color loseColor;

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
		transform.DOScale(Vector2.zero, 0.25f).SetEase(Ease.Linear).OnComplete(() => { Destroy(gameObject); });
	}

	public void Destroy()
	{
		Destroy(gameObject);
	}
}