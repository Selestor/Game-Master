using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MovingObject
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        int randomNumber = UnityEngine.Random.Range(0, 2);
        equippedWeaponId = randomNumber;
        
        movementLeft = moveRange;
        action = true;

        WeaponInformation();
    }



    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.WhosTurn() == id)
        {
            if (!GameManager.instance.isAnythingMoving)
            {
                Player player;
                if (GameObject.FindWithTag("Player") == null) return;
                else player = GameObject.FindWithTag("Player").transform.GetComponent<Player>();
                bool isPlayerInRange = CheckIfInRange(transform.position, GameManager.instance.playerPosition, GetWeaponRange());

                //move to player
                if (!isPlayerInRange)
                {
                    Vector3 destination = new Vector3();
                    destination = DestinationPoint();
                    movementLeft = Move(transform.position, destination, movementLeft);
                }
                //attack
                if (isPlayerInRange && action) Attack<Player>(player);
                if ((isPlayerInRange && action == false) || (action == true && !HasMovementLeft() && !isPlayerInRange && !GameManager.instance.isAnythingMoving))
                {
                    StartCoroutine(Delay());
                    EndTurn("Enemy nr " + id + " finished.");
                }
            }
        }
    }

    IEnumerator Delay()
    {
        GameManager.instance.isAnythingMoving = true;
        yield return new WaitForSecondsRealtime(1);
        GameManager.instance.isAnythingMoving = false;
    }

    private bool HasMovementLeft()
    {
        RaycastHit2D hit = Physics2D.Linecast(transform.position, transform.position, 1 << LayerMask.NameToLayer("Obstacle"));
        if(hit.transform == null)
        {
            if (movementLeft == 0) return false;
            else return true;
        }
        else
        {
            if (movementLeft < GameManager.instance.puddleCost) return false;
            else return true;
        }
    }

    private Vector3 DestinationPoint()
    {
        Vector3 playerPosition = new Vector3();
        playerPosition = GameManager.instance.playerPosition;
        int distanceFromPlayer;
        distanceFromPlayer = DistanceFromPlayer(transform.position);

        if (distanceFromPlayer > weapon.range)
        {
            Vector3 destination = new Vector3();
            List<Vector3> listAdjecentSquares = new List<Vector3>();

            List<int> listDistances = new List<int>();
            for (int i = -weapon.range; i <= weapon.range; i++)
                for (int j = -weapon.range; j <= weapon.range; j++)
                {
                    float x = playerPosition.x + i;
                    float y = playerPosition.y + j;
                    if ((Mathf.Abs(i) + Mathf.Abs(j)) == weapon.range && x >=0 && y >= 0 && x < GameManager.instance.boardScript.columns && y < GameManager.instance.boardScript.rows)
                    {
                        Vector3 possibleSquare = new Vector3();
                        possibleSquare.x = x;
                        possibleSquare.y = y;
                        RaycastHit2D hit = Physics2D.Linecast(possibleSquare, possibleSquare, 1 << LayerMask.NameToLayer("BlockingLayer"));
                        if (hit.transform == null)
                        {
                            listAdjecentSquares.Add(possibleSquare);
                            int distance;
                            List<Vector3> shortestPath = new List<Vector3>();
                            shortestPath = GetShortestPath(transform.position, possibleSquare);
                            if (shortestPath.Count > 0)
                            {
                                distance = movingAlgorythm.ReturnPathMovementCost(shortestPath);
                                listDistances.Add(distance);
                            }
                            else listAdjecentSquares.Remove(possibleSquare);
                        }
                    }
                }

            //without this condition, enemies who has no good place to go <especally if melee> will return error
            if (listDistances.Count > 0)
            {
                int smallestDistance = listDistances[0];
                int id = 0;
                for (int i = 0; i < listDistances.Count; i++)
                {
                    if (smallestDistance > listDistances[i])
                        id = i;
                }
                destination = listAdjecentSquares[id];

                return destination;
            }
            else
            {
                movementLeft = 0;
                return transform.position;
            }
        }
        else return transform.position;
    }

    private int DistanceFromPlayer(Vector3 square)
    {
        Vector3 playerPosition = new Vector3();
        playerPosition = GameManager.instance.playerPosition;
        int distanceFromPlayer = Mathf.RoundToInt(Mathf.Abs(square.x - playerPosition.x) + Mathf.Abs(square.y - playerPosition.y));
        return distanceFromPlayer;
    }

    public void LoseHealth(int loss)
    {
        FloatingTextController.CreateFloatingText(loss.ToString(), transform);
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
            GameManager.instance.turnScript.RemoveFromQueue(this.id);
            if(GameManager.instance.turnScript.CheckIfPlayerWin())
            {
                GameManager.instance.GameOver();
            }
        }
    }

    protected override void Attack<T>(T component)
    {
        Player target = component as Player;
        Vector3 targetLocation = target.transform.position;

        if (CheckIfInRange(transform.position, targetLocation, weapon.range))
        {
            int modifier = 0;
            if (weapon.attribute == "str") modifier = str;
            if (weapon.attribute == "dex") modifier = dex;
            int attack = UnityEngine.Random.Range(1, 20);
            attack += modifier;

            print("You attack " + component.name + ". Your attack score is " + attack + " vs target armor of " + target.baseArmor);

            Vector3 offset = new Vector3();
            if (attack >= target.baseArmor)
            {
                int damage = UnityEngine.Random.Range(weapon.minDamage, weapon.maxDamage);
                damage += modifier;
                offset.x = 0;
                offset.y = 0;
                print("You score a hit for " + damage + " damage!");
                target.LoseHealth(damage);
            }
            else
            {
                if (weapon.weaponId == 0)
                {
                    int randX = 0;
                    int randY = 0;
                    while (randX == 0 && randY == 0)
                    {
                        randX = UnityEngine.Random.Range(-1, 1);
                        randY = UnityEngine.Random.Range(-1, 1);
                    }
                    offset.x = 0 + randX;
                    offset.y = 0 + randY;
                }
                print("Your attack missed.");
                FloatingTextController.CreateFloatingText("Miss", target.transform);
            }

            //SPAWN ARROW
            if (weapon.weaponId == 0)
            {
                Vector3 location = targetLocation + offset;
                float angle = Vector3.Angle(new Vector3(0, 1, 0), location - transform.position);
                if (location.x > transform.position.x) angle = -angle;
                GameObject arrow = Instantiate(GameManager.instance.weaponScript.arrowPrefab, transform.position, Quaternion.Euler(new Vector3(0, 0, angle)));
                arrow.GetComponent<Arrow>().target = location;
            }

            action = false;
        }
        else print("Target is out of range.");
    }
}
