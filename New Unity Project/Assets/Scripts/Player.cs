using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

    public int healthPoints = 8;
    public int moveRange = 6;

    private APathAlgorythm movingAlgorythm;

    //private Animator animator;
	// Use this for initialization
	protected override void Start () {
        base.Start();
	}

    private void OnDisable()
    {
        //tutaj jakby cos sie zmienialo, np poziom gry
    }
    
    void Update ()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            movingAlgorythm = gameObject.AddComponent(typeof(APathAlgorythm)) as APathAlgorythm;
            Vector3 end = GameManager.instance.mousePosition;
            List<Vector3> shortestPath = new List<Vector3>();
            shortestPath = movingAlgorythm.ReturnShortestPath(transform.position, end);

            shortestPath = Reverse(shortestPath);

            foreach (Vector3 step in shortestPath)
             {
                Move(step);
            }
            //Move(end);
        }
    }

    List<Vector3> Reverse(List<Vector3> list)
    {
        List<Vector3> reversedList = new List<Vector3>();

        for (int i = list.Count - 1; i > 0; i--)
            reversedList.Add(list[i]);

        return reversedList;
    }

    private void CheckIfGameOver()
    {
        if (healthPoints <= 0)
            GameManager.instance.GameOver();
    }

    public void LoseHealth(int loss)
    {
        healthPoints -= loss;
        CheckIfGameOver();
    }
}
