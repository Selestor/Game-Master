using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class APathAlgorythm : MonoBehaviour {
    private int columns;
    private int rows;

    private float[,] pathScoring;
    private List<Vector3> openList = new List<Vector3>();
    private List<Vector3> closedList = new List<Vector3>();

    public void DoTheThing()
    {
        for (int x = 0; x < columns; x++)
            for (int y = 0; y < rows; y++)
                pathScoring[x, y] = 0;
        CalculateShortestPath(GameManager.instance.playerPosition, GameManager.instance.mousePosition);
        GameManager.instance.shortestPath = closedList;
    }

    private void CalculateShortestPath(Vector3 begining, Vector3 end)
    {
        columns = GameManager.instance.GetComponent<BoardManager>().columns;
        rows = GameManager.instance.GetComponent<BoardManager>().rows;

        closedList.Add(begining);
        AddAdjecentToList(openList, begining);

        int[] coordinates = LowestScoreIndex(pathScoring);
        int x = coordinates[0];
        int y = coordinates[1];

        Vector3 S = new Vector3(x, y);
        openList.Remove(S);
        closedList.Add(S);

        if (S != end) CalculateShortestPath(S, end);
    }



    private void AddAdjecentToList(List<Vector3> list,Vector3 current)
    {
        Vector3 offset;
        for (int i = 0; i < 4; i++)
        {
            offset = new Vector3(1, 0, 0);
            if (i == 1) offset = new Vector3(-1, 0, 0);
            else if (i == 2) offset = new Vector3(0, 1, 0);
            else if (i == 3) offset = new Vector3(0, -1, 0); 
            Vector3 adjecent = current + offset;
            if (adjecent.x > 0 && adjecent.x < columns && adjecent.y > 0 && adjecent.y < rows)
            {
                if (!closedList.Contains(adjecent))
                {
                    float F;
                    F = CalculatePathScore(current, adjecent);
                    if (!openList.Contains(adjecent))
                    {
                        list.Add(adjecent);
                        pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)] = F;
                    }
                    else
                    {
                        float oldF = pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)];
                        if (F < oldF)
                        {
                            list.Remove(adjecent);
                            list.Add(adjecent);
                            pathScoring[Mathf.RoundToInt(adjecent.x), Mathf.RoundToInt(adjecent.y)] = F;
                        }
                    }
                }
            }
        }
    }

    private float CalculatePathScore(Vector3 current, Vector3 adjecent)
    {
        float G = pathScoring[Mathf.RoundToInt(current.x), Mathf.RoundToInt(current.y)] + 1;
        float H = Mathf.Abs(closedList[(closedList.Count - 1)].x - adjecent.x) + Mathf.Abs(closedList[(closedList.Count - 1)].y - adjecent.y);
        float F = G + H;
        return F;
    }

    private int[] LowestScoreIndex(float[,] scoreBoard)
    {
        int[] lowestScoreIndex = new int[2];
        float min = scoreBoard[0, 0];

        int x = Mathf.RoundToInt(Mathf.Sqrt(scoreBoard.Length));
        
        for (int i = 0; i < x; i++)
            for (int j = 0; j< x; j++)
            {
                if ( min > scoreBoard[i, j])
                {
                    min = scoreBoard[i, j];
                    lowestScoreIndex[0] = i;
                    lowestScoreIndex[1] = j;
                }
            }

        return lowestScoreIndex;
    }
}
