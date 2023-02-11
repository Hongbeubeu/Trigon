using UnityEngine;

public enum TypeTile
{
    UP,
    DOWN
}

public class BoardTile : MonoBehaviour
{
    public bool isContainsTile;
    public TypeTile type;
    public Vector3 positionInMatrix;

    public void SetProperties(TypeTile typeTile, Vector3 posInMatrix)
    {
        type = typeTile;
        positionInMatrix = posInMatrix;
    }
}
