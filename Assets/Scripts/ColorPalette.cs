using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Trigon/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color[] colors;

    public Color GetRandomColor()
    {
        return colors[Random.Range(0, colors.Length)];
    }
}
