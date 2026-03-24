using UnityEngine;

public class BoardTile : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    public TypeTile TypeTile { get; set; }
    public SpriteRenderer SpriteRenderer => _spriteRenderer ??= GetComponent<SpriteRenderer>();
}