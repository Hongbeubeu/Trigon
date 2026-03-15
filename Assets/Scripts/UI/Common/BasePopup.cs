using UnityEngine;

public abstract class BasePopup : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public bool IsVisible { get; private set; }

    public virtual void Show()
    {
        IsVisible = true;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

    public virtual void Hide()
    {
        IsVisible = false;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }
}
