using UnityEngine;

[CreateAssetMenu(fileName = "UIViewConfig", menuName = "Trigon/UI View Config")]
public class UIViewConfig : ScriptableObject
{
    [SerializeField] private string scoreFormat = "{0}";

    public string ScoreFormat => scoreFormat;
}
