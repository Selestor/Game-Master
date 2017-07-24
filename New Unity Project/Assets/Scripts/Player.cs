using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {

    public int healthPoints = 8;
    public int moveRange = 6;

    //private Animator animator;
	// Use this for initialization
	protected override void Start () {
        base.Start();
	}

    private void OnDisable()
    {
        //tutaj jakby cos sie zmienialo, np poziom gry
    }
    
    void Update () {
        if (!GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButtonDown(0))
        {
            List<Vector3> shortestPath = new List<Vector3>();
            Vector3 mousePosition = GameManager.instance.mousePosition;
            shortestPath = GameManager.instance.shortestPath;

            foreach (Vector3 posVec in shortestPath)
            {
                float x = Mathf.RoundToInt(posVec.x) - transform.position.x;
                float y = Mathf.RoundToInt(posVec.y) - transform.position.y;
                if (Mathf.Abs(x) > Mathf.Abs(y))
                {
                    horizontal = x > 0 ? 1 : -1;

                }
                else
                {
                    vertical = y > 0 ? 1 : -1;
                }
            }
        }

        if (horizontal != 0)
            vertical = 0;

        RaycastHit2D hit;
        if (horizontal != 0 || vertical != 0)
            Move(horizontal, vertical, out hit);
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
