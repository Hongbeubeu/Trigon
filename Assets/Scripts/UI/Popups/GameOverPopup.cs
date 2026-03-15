using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopup : BasePopup
{
    [Header("Data")]
    [SerializeField] private GameOverPopupData _popupData;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _replayButton;

    private void Start()
    {
        if (_popupData != null && _titleText != null)
            _titleText.text = _popupData.Title;

        _replayButton.onClick.AddListener(OnReplayClicked);
    }

    private static void OnReplayClicked()
    {
        GameEvents.RaiseReplayRequested();
    }

    private static void OnQuitClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _replayButton.onClick.RemoveListener(OnReplayClicked);
    }
}
