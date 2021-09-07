using UnityEngine;

public enum TypeTile
{
    UP,
    DOWN
}

public class BoardTile : MonoBehaviour
{
    public bool isContainsTile = false;
    public Vector2 position;
    public TypeTile type;
    public Vector3 positionInMatrix;

    public void SetProperties(Vector2 pos, TypeTile type, Vector3 posInMatrix)
    {
        this.position = pos;
        this.type = type;
        this.positionInMatrix = posInMatrix;
    }
}
