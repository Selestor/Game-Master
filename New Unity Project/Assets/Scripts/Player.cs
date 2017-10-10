using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {
    protected override void Start ()
    {
        base.Start();
        
        str = 1;
        dex = 1;
        healthPoints = 8;
        id = 0;
        moveRange = 6;
    }

    private void OnDisable()
    {
        //tutaj jakby cos sie zmienialo, np poziom gry
    }
    
    void Update ()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pointerPosition = GameManager.instance.mousePosition;
            BoardManager boardManager = GameManager.instance.boardScript;

            if (boardManager.gridFreePositions.Contains(pointerPosition) || pointerPosition.x == 0 || pointerPosition.y == 0) // checking if skull
            {
                Vector3 end = GameManager.instance.mousePosition;
                if (end.x < 0 || end.x > boardManager.rows - 1 || end.y < 0 || end.y > boardManager.columns - 1 || transform.position == end)
                    return;
                else
                {
                    Move(transform.position, end);
                }
            }
        }

        if(Input.GetMouseButtonDown(1))
        {
            Vector3 pointerPosition = GameManager.instance.mousePosition;
            int layerMask = 1 << 8;
            RaycastHit2D hit = Physics2D.Linecast(pointerPosition, pointerPosition, layerMask);
            if(hit && hit.transform.name == "Enemy(Clone)")
            {
                Attack<Enemy>(hit.transform.GetComponent<Enemy>());
            }
        }

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0 || vertical != 0)
            SimpleMove(horizontal, vertical);
    }

    protected override void Attack<T>(T component)
    {
        Enemy target = component as Enemy;
        Vector3 targetLocation = target.transform.position;

        if (CheckIfInRange(transform.position, targetLocation, 1))
        {
            int attack = UnityEngine.Random.Range(1, 20);
            attack += str;

            print("You attack " + component.name + ". Your attack score is " + attack + " vs target armor of " + target.baseArmor);

            if (attack >= target.baseArmor)
            {
                int damage = UnityEngine.Random.Range(1, 6);
                damage += str;
                print("You score a hit for " + damage + " damage!");
                target.LoseHealth(damage);
            }
            else print("Your attack missed.");
        }
        else print("Target is out of range.");
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
