﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSpawner : MonoBehaviour
{
    public Transform[] zone;
    public Color[] colors;

    string tilePrefabPath = "Prefabs/Composite Tiles";

    GameObject[] tilePrefabs;
    private void Start()
    {
        tilePrefabs = Resources.LoadAll<GameObject>(tilePrefabPath);
        RandomTile();
    }

    void RandomTile()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject tile = Instantiate(tilePrefabs[Random.Range(0, tilePrefabs.Length)], zone[i].position, Quaternion.identity);
            BoxCollider2D collider = tile.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(2.5f, 2.5f);
            tile.transform.localScale = new Vector2(0.45f, 0.45f);
            tile.AddComponent<CompositeTile>();
            tile.transform.SetParent(zone[i]);
            Color randColor = colors[Random.Range(0, colors.Length)];
            for (int j = 0; j < tile.transform.childCount; j++)
            {
                tile.transform.GetChild(j).GetComponent<SpriteRenderer>().color = randColor;
            }
        }
    }
}
