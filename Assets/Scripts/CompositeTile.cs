using System.Collections.Generic;
using UnityEngine;

public class CompositeTile : MonoBehaviour
{
	public int id;
	public Vector2 rootPos;
	public Vector2 rootScale;
	public Color rootColor;
	private Color _loseColor;
	private const int TOP_SORTING_ORDER = 5;
	private const int ROOT_SORTING_ORDER = 2;
	public List<BaseTile> baseTiles = new();
	public List<Vector2> baseTilePosDistance = new();
	public bool isPause;
	public bool canPutToBoard = true;
	public List<SpriteRenderer> spriteChildren;

	private void Awake()
	{
		rootPos = transform.position;
		_loseColor = new Color(176f / 255f, 176f / 255f, 176 / 255f, 1);
	}

	public void InitBaseTilePosition()
	{
		Vector2 rootPoint = baseTiles[0].transform.position;
		baseTilePosDistance.Add(Vector2.zero);
		for (var i = 1; i < baseTiles.Count; i++)
		{
			Vector2 currentPoint = baseTiles[i].transform.position;
			var distanceVector = currentPoint - rootPoint;
			baseTilePosDistance.Add(distanceVector);
		}
	}

	public void ChangeColorTile(Color color)
	{
		for (var i = 0; i < spriteChildren.Count; i++)
		{
			spriteChildren[i].color = color;
		}
	}

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
		var screenPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		if (Camera.main == null) return;
		Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
		worldPos.y += 1f;
		transform.position = worldPos;
	}

	private void OnMouseUp()
	{
		if (isPause || !canPutToBoard)
			return;
		CheckValidPositionToPutTilesDown();
	}

	private void SetScaleOnPickUp()
	{
		transform.localScale = new Vector2(0.5f, 0.5f);
		SetSortingOrder(TOP_SORTING_ORDER);
	}

	private void ResetPosition()
	{
		transform.position = rootPos;
		transform.localScale = rootScale;
		SetSortingOrder(ROOT_SORTING_ORDER);
	}

	private void CheckValidPositionToPutTilesDown()
	{
		var desPos = new List<Vector2>();
		Vector2 currentFirstPoint = baseTiles[0].transform.position;
		for (int i = 0; i < baseTilePosDistance.Count; i++)
		{
			var res = GameManager.Instance.FindNearestTilePosition(currentFirstPoint + baseTilePosDistance[i],
				baseTiles[i].type);
			if (res.x + 100 == 0 && res.y == 0)
			{
				ResetPosition();
				return;
			}

			desPos.Add(res);

			if (i == 0)
				currentFirstPoint = res;
		}

		for (var i = 0; i < baseTiles.Count; i++)
		{
			baseTiles[i].transform.position = desPos[i];
			GameManager.Instance.SetTileToBoard(baseTiles[i]);
		}

		GameManager.Instance.Score += baseTiles.Count;
		GameManager.Instance.tileOnSpawner.Remove(id);
		SetSortingOrder(ROOT_SORTING_ORDER);
		GameManager.Instance.ClearCross();
		GameManager.Instance.NumberTileOnSpawnZone--;
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
			tempColor = _loseColor;
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

	public void Destroy()
	{
		foreach (var item in baseTiles)
		{
			if (item != null)
				item.Destroy();
		}
	}
}