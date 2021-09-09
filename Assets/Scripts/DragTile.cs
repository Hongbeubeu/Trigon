using UnityEngine;

public class DragTile : MonoBehaviour
{
#if PLATFORM_ANDROID
    private bool dragging = false;
    private Transform toDrag;

    private void Update()
    {
        if (Input.touchCount != 1)
        {
            dragging = false;
            return;
        }

        Vector2 worldPos;
        Touch touch = Input.touches[0];
        Vector2 touchPos = touch.position;

        if (touch.phase == TouchPhase.Began)
        {
            RaycastHit2D hit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(touchPos), Vector2.zero);
            if (hit2D.collider.tag == "Draggable")
            {
                toDrag = hit2D.collider.transform;
                CompositeTile compositeTile = toDrag.GetComponent<CompositeTile>();
                if (compositeTile.isPause || !compositeTile.canPutToBoard)
                    return;
                compositeTile.SetScaleOnPickUp();
                worldPos = new Vector3(touchPos.x, touchPos.y);
                worldPos = Camera.main.ScreenToWorldPoint(worldPos);
                dragging = true;
            }
        }

        if (dragging && touch.phase == TouchPhase.Moved)
        {
            worldPos = Camera.main.ScreenToWorldPoint(touchPos);
            worldPos.y += 0.65f;
            toDrag.position = worldPos;
        }

        if (dragging && (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled))
        {
            dragging = false;
            CompositeTile compositeTile = toDrag.GetComponent<CompositeTile>();
            compositeTile.CheckValidPositionToPutTilesDown();
        }
    }
#endif
}
