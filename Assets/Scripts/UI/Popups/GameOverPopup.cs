using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameOverPopup : BasePopup
{
    [Header("Data")]
    [SerializeField] private GameOverPopupData popupData;

    [Header("References")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button quitButton;

    private int _lastScore;

    private void Start()
    {
        if (popupData != null && titleText != null)
            titleText.text = popupData.Title;

        replayButton.onClick.AddListener(OnReplayClicked);
        quitButton.onClick.AddListener(OnQuitClicked);
    }

    private void OnEnable()
    {
        GameEvents.OnScoreChanged += CacheScore;
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= CacheScore;
    }

    private void CacheScore(int score)
    {
        _lastScore = score;
    }

    public override void Show()
    {
        base.Show();

        if (scoreText != null && popupData != null)
            scoreText.text = string.Format(popupData.ScoreFormat, _lastScore);
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
        replayButton.onClick.RemoveListener(OnReplayClicked);
        quitButton.onClick.RemoveListener(OnQuitClicked);
    }
}
