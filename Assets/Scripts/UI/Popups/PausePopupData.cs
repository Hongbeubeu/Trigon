using UnityEngine;

[CreateAssetMenu(fileName = "PausePopupData", menuName = "Trigon/UI/Pause Popup Data")]
public class PausePopupData : BasePopupData
{
    [SerializeField] private string continueButtonText = "Continue";
    [SerializeField] private string quitButtonText = "Quit";

    public string ContinueButtonText => continueButtonText;
    public string QuitButtonText => quitButtonText;
}
