using UnityEngine;
using TMPro;

public enum UIPanel
{
    PAUSE,
    LOSE,
    PLAY
}
public class UIManager : MonoBehaviour
{
    #region Singleton
    static UIManager _instance;

    public static UIManager instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<UIManager>();
            return _instance;
        }
    }
    #endregion
    Transform losePanel,
              pausePanel,
              playPanel;
    TextMeshProUGUI maxScore, currentScore;
    private void Awake()
    {
        Init();
    }

    private void OnEnable()
    {
        SetActivePanel(UIPanel.PLAY);
    }

    void Init()
    {
        losePanel = transform.Find("Lose Panel");
        pausePanel = transform.Find("Pause Panel");
        playPanel = transform.Find("Play Panel");

        maxScore = playPanel.Find("Score Panel/Score Max").GetComponent<TextMeshProUGUI>();
        currentScore = playPanel.Find("Score Panel/Score Current").GetComponent<TextMeshProUGUI>();
    }

    public void SetActivePanel(UIPanel panel)
    {
        ResetPanel();
        switch (panel)
        {
            case UIPanel.PAUSE:
                pausePanel.gameObject.SetActive(true);
                break;
            case UIPanel.LOSE:
                losePanel.gameObject.SetActive(true);
                break;
            case UIPanel.PLAY:
                playPanel.gameObject.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void SetCurrentScore(int score)
    {
        currentScore.SetText("Current: " + score);
    }

    public void SetMaxScore(int max)
    {
        maxScore.SetText("Max: " + max);
    }

    void ResetPanel()
    {
        losePanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
