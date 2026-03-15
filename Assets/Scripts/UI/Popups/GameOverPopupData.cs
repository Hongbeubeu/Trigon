using UnityEngine;

[CreateAssetMenu(fileName = "GameOverPopupData", menuName = "Trigon/UI/Game Over Popup Data")]
public class GameOverPopupData : BasePopupData
{
    [SerializeField] private string scoreFormat = "Score: {0}";
    [SerializeField] private string replayButtonText = "Replay";
    [SerializeField] private string quitButtonText = "Quit";

    public string ScoreFormat => scoreFormat;
    public string ReplayButtonText => replayButtonText;
    public string QuitButtonText => quitButtonText;
}
