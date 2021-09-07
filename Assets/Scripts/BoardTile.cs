using UnityEngine;

public enum TypeTile
{
    UP,
    DOWN
}

public class BoardTile : MonoBehaviour
{
    public bool isContainsTile = false;
    public TypeTile type;
    public Vector3 positionInMatrix;

    public void SetProperties(TypeTile type, Vector3 posInMatrix)
    {
        this.type = type;
        this.positionInMatrix = posInMatrix;
    }
}
