﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    public int columns = 10;
    public int rows = 10;

    public GameObject[] floorTiles;
    public GameObject highlightTile;
    public GameObject outerWall;
    public GameObject[] obstacle;

    private Transform boardHolder;
    public List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns+1; x++)
            for (int y = -1; y < rows+1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)]; // losuje 1 floor tile
                GameObject toInstantiate2 = highlightTile;
                if (x == -1 || y == -1 || x == columns || y == columns)
                    toInstantiate = outerWall;

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                GameObject instance2 = Instantiate(toInstantiate2, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance2.GetComponent<SpriteRenderer>().enabled = false;
                instance.transform.SetParent(boardHolder);
                instance2.transform.SetParent(boardHolder);
            }
    }

    public void SetupScene()
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(obstacle, 15, 15);
    }

    // random object on scene
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }
}
