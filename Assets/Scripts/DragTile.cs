using System;
using UnityEngine;

public class DragTile : MonoBehaviour
{
	// private bool dragging = false;
	// private Transform toDrag;
	// private CompositeTile compositeTile;
	// private Camera camera;
	// private Vector2 targetPosition;
	//
	// private void Awake()
	// {
	// 	camera = Camera.main;
	// }
	//
	// private void FixedUpdate()
	// {
	// 	if (Input.touchCount != 1)
	// 	{
	// 		dragging = false;
	// 		return;
	// 	}
	//
	// 	Vector2 worldPos;
	// 	Touch touch = Input.touches[0];
	// 	Vector2 touchPos = touch.position;
	//
	// 	if (touch.phase == TouchPhase.Began)
	// 	{
	// 		RaycastHit2D hit2D = Physics2D.Raycast(camera.ScreenToWorldPoint(touchPos), Vector2.zero);
	// 		if (hit2D.collider.CompareTag("Draggable"))
	// 		{
	// 			toDrag = hit2D.collider.transform;
	// 			compositeTile = toDrag.GetComponent<CompositeTile>();
	// 			if (compositeTile.isPause || !compositeTile.canPutToBoard)
	// 				return;
	// 			compositeTile.SetScaleOnPickUp();
	// 			// worldPos = new Vector3(touchPos.x, touchPos.y);
	// 			// worldPos = camera.ScreenToWorldPoint(worldPos);
	// 			dragging = true;
	// 		}
	// 	}
	//
	// 	if (dragging && touch.phase == TouchPhase.Moved)
	// 	{
	// 		worldPos = camera.ScreenToWorldPoint(touchPos);
	// 		worldPos.y += 2f;
	// 		targetPosition = worldPos;
	// 	}
	//
	// 	if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
	// 	{
	// 		dragging = false;
	// 		compositeTile.CheckValidPositionToPutTilesDown();
	// 	}
	// }
	//
	// private void Update()
	// {
	// 	if (toDrag != null)
	// 		toDrag.position = targetPosition;
	// }
}