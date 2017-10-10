using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void Attack<T>(T component)
    {
        /*Player target = component as Player;

        int attack = UnityEngine.Random.Range(1, 20);
        */
    }

    public void LoseHealth(int loss)
    {
        healthPoints -= loss;
        CheckIfDead();
    }

    private void CheckIfDead()
    {
        if (healthPoints <= 0)
        {
            gameObject.SetActive(false);
            GameManager.instance.boardScript.gridFreePositions.Add(new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0));
            print("Enemy died!");
        }
    }
}
