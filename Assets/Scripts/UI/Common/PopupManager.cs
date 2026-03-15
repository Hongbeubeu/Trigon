using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    private readonly Dictionary<Type, BasePopup> _popups = new();
    private readonly Stack<BasePopup> _stack = new();

    private void Awake()
    {
        var popups = GetComponentsInChildren<BasePopup>(true);
        foreach (var popup in popups)
        {
            _popups[popup.GetType()] = popup;
            popup.Hide();
        }
    }

    private void OnEnable()
    {
        GameEvents.OnGameStateChanged += OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        HideAll();

        switch (state)
        {
            case GameState.Paused:
                Show<PausePopup>();
                break;
            case GameState.Lost:
                Show<GameOverPopup>();
                break;
        }
    }

    public T Show<T>() where T : BasePopup
    {
        if (!_popups.TryGetValue(typeof(T), out var popup)) return null;

        popup.Show();
        _stack.Push(popup);
        return (T)popup;
    }

    public void HideCurrent()
    {
        if (_stack.Count > 0)
        {
            _stack.Pop().Hide();
        }
    }

    public void HideAll()
    {
        while (_stack.Count > 0)
        {
            _stack.Pop().Hide();
        }
    }
}
