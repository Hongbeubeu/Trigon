using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudView : MonoBehaviour
{
    [SerializeField] private CanvasGroup hudPanel;
    [SerializeField] private TextMeshProUGUI maxScoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;
    [SerializeField] private Button pauseButton;
    [SerializeField] private UIViewConfig uiViewConfig;

    private string _scoreFormat = "{0}";

    private void Awake()
    {
        if (uiViewConfig != null)
        {
            _scoreFormat = uiViewConfig.ScoreFormat;
        }
    }

    private void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateCurrentScore;
        GameEvents.OnMaxScoreLoaded += UpdateMaxScore;
        GameEvents.OnGameStateChanged += OnGameStateChanged;
        pauseButton.onClick.AddListener(OnPauseClicked);
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateCurrentScore;
        GameEvents.OnMaxScoreLoaded -= UpdateMaxScore;
        GameEvents.OnGameStateChanged -= OnGameStateChanged;
        pauseButton.onClick.RemoveListener(OnPauseClicked);
    }

    private static void OnPauseClicked()
    {
        GameEvents.RaisePauseRequested();
    }

    private void OnGameStateChanged(GameState state)
    {
        SetPanelVisible(hudPanel, state == GameState.Playing);
    }

    private void UpdateCurrentScore(int score)
    {
        currentScoreText.SetText(string.Format(_scoreFormat, score));
    }

    private void UpdateMaxScore(int maxScore)
    {
        maxScoreText.SetText(string.Format(_scoreFormat, maxScore));
    }

    private static void SetPanelVisible(CanvasGroup panel, bool visible)
    {
        panel.alpha = visible ? 1f : 0f;
        panel.blocksRaycasts = visible;
    }
}
