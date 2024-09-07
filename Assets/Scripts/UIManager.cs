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

	private static UIManager _instance;

	public static UIManager Instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<UIManager>();
			return _instance;
		}
	}

	#endregion

	[SerializeField] private CanvasGroup losePanel;
	[SerializeField] private CanvasGroup pausePanel;
	[SerializeField] private CanvasGroup playPanel;

	[SerializeField] private TextMeshProUGUI maxScore;
	[SerializeField] private TextMeshProUGUI currentScore;


	private void OnEnable()
	{
		SetActivePanel(UIPanel.PLAY);
	}

	public void SetActivePanel(UIPanel panel)
	{
		ResetPanel();
		switch (panel)
		{
			case UIPanel.PAUSE:
				pausePanel.alpha = 1;
				pausePanel.blocksRaycasts = true;
				break;
			case UIPanel.LOSE:
				losePanel.alpha = 1;
				losePanel.blocksRaycasts = true;
				break;
			case UIPanel.PLAY:
				playPanel.alpha = 1;
				playPanel.blocksRaycasts = true;
				break;
		}
	}

	public void SetCurrentScore(int score)
	{
		currentScore.SetText($"{score}");
	}

	public void SetMaxScore(int max)
	{
		maxScore.SetText($"{max}");
	}

	private void ResetPanel()
	{
		losePanel.alpha = 0;
		losePanel.blocksRaycasts = false;
		pausePanel.alpha = 0;
		pausePanel.blocksRaycasts = false;
		playPanel.alpha = 0;
		playPanel.blocksRaycasts = false;
	}

	public void Quit()
	{
		Application.Quit();
	}
}