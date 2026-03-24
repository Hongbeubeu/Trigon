using UnityEngine;

/// <summary>
/// Represents the visual background slot on the grid where shapes can be dropped.
/// </summary>
public class BoardTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public TypeTile TypeTile { get; set; }
    public SpriteRenderer SpriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();
}