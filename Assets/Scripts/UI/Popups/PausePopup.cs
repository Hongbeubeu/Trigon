using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePopup : BasePopup
{
    [Header("Data")]
    [SerializeField] private PausePopupData _popupData;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private Button _continueButton;
    [SerializeField] private Button _quitButton;

    private void Start()
    {
        if (_popupData != null && _titleText != null)
            _titleText.text = _popupData.Title;

        _continueButton.onClick.AddListener(OnContinueClicked);
        _quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnContinueClicked()
    {
        GameEvents.RaiseResumeRequested();
    }

    private static void OnQuitClicked()
    {
        Application.Quit();
    }

    private void OnDestroy()
    {
        _continueButton.onClick.RemoveListener(OnContinueClicked);
        _quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
