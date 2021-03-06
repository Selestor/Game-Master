﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour {

    public int columns = 10;
    public int rows = 10;

    public int enemies = 1;

    public GameObject[] floorTiles;
    public GameObject highlightTile;
    public GameObject outerWall;
    public GameObject[] obstacle;
    public Enemy[] enemy;
    public Player player;

    public List<Vector3> gridFreePositions = new List<Vector3>();

    private Transform boardHolder;

    private List<MovingObject> actors;
    public void AddActor(MovingObject actor)
    {
        actors.Add(actor);
    }

    public List<MovingObject> ActorsList()
    {
        return actors;
    }

    void InitializeList()
    {
        gridFreePositions.Clear();
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
            {
                if (x != 0 && y != 0 && x != columns - 1 && y != rows - 1) gridFreePositions.Add(new Vector3(x, y, 0f));
            }
            
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = -1; x < columns+1; x++)
            for (int y = -1; y < rows+1; y++)
            {
                GameObject toInstantiate;
                if (x == -1 || y == -1 || x == columns || y == columns)
                    toInstantiate = outerWall;
                else
                {
                    toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                    GameObject toInstantiate2 = highlightTile;
                    GameObject instance2 = Instantiate(toInstantiate2,
                        new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                    instance2.GetComponent<SpriteRenderer>().enabled = false;
                    instance2.transform.SetParent(boardHolder);
                }
                GameObject instance = Instantiate(toInstantiate,
                    new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
    }

    public void SetupScene()
    {
        actors = new List<MovingObject>();
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(obstacle, 15);
        SpawnEnemyAtRandom(enemy, enemies);
        AddActor(player);
    }

    // random object on scene
    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridFreePositions.Count);
        Vector3 randomPosition = gridFreePositions[randomIndex];
        gridFreePositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    void SpawnEnemyAtRandom(Enemy[] enemyArray, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 randomPosition = RandomPosition();

            Enemy enemyClone = enemyArray[Random.Range(0, enemyArray.Length)];
            enemyClone = Instantiate(enemyClone, randomPosition, Quaternion.identity);
            enemyClone.GetComponent<Enemy>().id = i + 1;
            AddActor(enemyClone);
        }
    }
}
