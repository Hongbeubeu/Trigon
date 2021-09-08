using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
              pausePanel;

    private void Awake()
    {
        Init();
    }

    void Init()
    {
        losePanel = transform.Find("Lose Panel");
        pausePanel = transform.Find("Pause Panel");

        losePanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
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
            default:
                ResetPanel();
                break;
        }
    }

    void ResetPanel()
    {
        losePanel.gameObject.SetActive(false);
        pausePanel.gameObject.SetActive(false);
    }
}
