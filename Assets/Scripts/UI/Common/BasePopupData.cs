using UnityEngine;

public abstract class BasePopupData : ScriptableObject
{
    [SerializeField] private string title;

    public string Title => title;
}
