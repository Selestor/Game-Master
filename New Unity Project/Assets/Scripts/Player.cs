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
    
    void Update ()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Vector3 end = GameManager.instance.mousePosition;
            if (end.x < 0 || end.x > GameManager.instance.boardScript.rows - 1 || end.y < 0 || end.y > GameManager.instance.boardScript.columns - 1 || transform.position == end)
                return;
            else
                Move(transform.position, end);
        }
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
