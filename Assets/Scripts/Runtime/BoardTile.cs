using UnityEngine;

public class BoardTile : MonoBehaviour
{
    public TypeTile type;
    [HideInInspector] public bool isContainsTile;
    [HideInInspector] public Vector3 positionInMatrix;
}
