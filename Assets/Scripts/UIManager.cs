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

	[SerializeField] private GameObject losePanel;

	[SerializeField] private GameObject pausePanel;
	[SerializeField] private GameObject playPanel;

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
				pausePanel.SetActive(true);
				break;
			case UIPanel.LOSE:
				losePanel.SetActive(true);
				break;
			case UIPanel.PLAY:
				playPanel.SetActive(true);
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

	void ResetPanel()
	{
		losePanel.SetActive(false);
		pausePanel.SetActive(false);
		playPanel.SetActive(false);
	}

	public void Quit()
	{
		Application.Quit();
	}
}