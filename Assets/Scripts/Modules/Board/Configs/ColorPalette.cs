using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Trigon/Color Palette")]
public class ColorPalette : ScriptableObject
{
    [SerializeField] private Color[] _colors;

    public Color GetRandomColor()
    {
        return _colors[Random.Range(0, _colors.Length)];
    }
}
