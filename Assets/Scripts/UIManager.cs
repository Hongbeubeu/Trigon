using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private CanvasGroup losePanel;
    [SerializeField] private CanvasGroup pausePanel;
    [SerializeField] private CanvasGroup playPanel;
    [SerializeField] private TextMeshProUGUI maxScoreText;
    [SerializeField] private TextMeshProUGUI currentScoreText;

    private string _scoreFormat;

    private void OnEnable()
    {
        GameEvents.OnScoreChanged += UpdateCurrentScore;
        GameEvents.OnMaxScoreLoaded += UpdateMaxScore;
        GameEvents.OnGameStateChanged += OnGameStateChanged;
    }

    private void Start()
    {
        if (ServiceLocator.TryGet<ConfigService>(out var config))
        {
            _scoreFormat = config.UI.ScoreFormat;
        }
        else
        {
            _scoreFormat = "{0}";
        }
    }

    private void OnDisable()
    {
        GameEvents.OnScoreChanged -= UpdateCurrentScore;
        GameEvents.OnMaxScoreLoaded -= UpdateMaxScore;
        GameEvents.OnGameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                ActivatePanel(playPanel);
                break;
            case GameState.Paused:
                ActivatePanel(pausePanel);
                break;
            case GameState.Lost:
                ActivatePanel(losePanel);
                break;
        }
    }

    private void UpdateCurrentScore(int score)
    {
        currentScoreText.SetText(string.Format(_scoreFormat, score));
    }

    private void UpdateMaxScore(int maxScore)
    {
        maxScoreText.SetText(string.Format(_scoreFormat, maxScore));
    }

    private void ActivatePanel(CanvasGroup target)
    {
        SetPanelState(losePanel, false);
        SetPanelState(pausePanel, false);
        SetPanelState(playPanel, false);
        SetPanelState(target, true);
    }

    private static void SetPanelState(CanvasGroup panel, bool active)
    {
        panel.alpha = active ? 1f : 0f;
        panel.blocksRaycasts = active;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
