using UnityEngine;
using System.IO;

[CreateAssetMenu(fileName = "TriangleGenerator", menuName = "Tools/TriangleGenerator")]
public class TriangleGeneratorSO : ScriptableObject
{
    [Header("Settings")]
    public int edgeLength = 512; // This is the base of the triangle (Width)
    public string fileName = "TriangleFit.png";
    public Color triangleColor = Color.white;

    public void Generate()
    {
        // Width is 'a', Height is 'h = (sqrt(3)/2) * a'
        int width = edgeLength;
        int height = Mathf.RoundToInt((Mathf.Sqrt(3f) / 2f) * edgeLength);

        // Create texture with calculated rectangle dimensions
        Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
        Color transparent = new Color(0, 0, 0, 0);

        // Fill background
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                texture.SetPixel(x, y, transparent);
            }
        }

        float a = (float)width;
        float h = (float)height;

        // Vertices defined to fit perfectly at the edges of the rectangle
        // Top vertex (Center X, Max Y)
        Vector2 v1 = new Vector2(a / 2f, h); 
        // Bottom Left (0, 0)
        Vector2 v2 = new Vector2(0, 0); 
        // Bottom Right (Max X, 0)
        Vector2 v3 = new Vector2(a, 0); 

        // Draw triangle pixels
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (IsInsideTriangle(new Vector2(x, y), v1, v2, v3))
                {
                    texture.SetPixel(x, y, triangleColor);
                }
            }
        }

        texture.Apply();
        SaveTexture(texture);
    }

    private bool IsInsideTriangle(Vector2 p, Vector2 p1, Vector2 p2, Vector2 p3)
    {
        float d1 = Sign(p, p1, p2);
        float d2 = Sign(p, p2, p3);
        float d3 = Sign(p, p3, p1);
        bool has_neg = (d1 < 0) || (d2 < 0) || (d3 < 0);
        bool has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);
        return !(has_neg && has_pos);
    }

    private float Sign(Vector2 p1, Vector2 p2, Vector2 p3)
    {
        return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
    }

    private void SaveTexture(Texture2D tex)
    {
        byte[] bytes = tex.EncodeToPNG();
        string path = Path.Combine(Application.dataPath, fileName);
        File.WriteAllBytes(path, bytes);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
        Debug.Log($"[TriangleGenerator] Generated rect PNG ({tex.width}x{tex.height}) at: {path}");
    }
}