using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PausePopup : BasePopup
{
    [Header("Data")]
    [SerializeField] private PausePopupData popupData;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button quitButton;

    private void Start()
    {
        if (popupData != null && titleText != null)
            titleText.text = popupData.Title;

        continueButton.onClick.AddListener(OnContinueClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
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
        continueButton.onClick.RemoveListener(OnContinueClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
