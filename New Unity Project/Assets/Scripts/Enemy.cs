using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MovingObject
{
    // Use this for initialization
    protected override void Start()
    {
        base.Start();

        str = 0;
        dex = 0;
        healthPoints = 4;
        baseArmor = 10;
        moveRange = 4;
        
        int randomNumber = UnityEngine.Random.Range(0, 2);
        equippedWeaponId = randomNumber;

        movementLeft = moveRange;
        action = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.WhosTurn() == id)
        {
            if (!GameManager.instance.isAnythingMoving)
            {
                //attack
                Player player = GameObject.FindWithTag("Player").transform.GetComponent<Player>();
                if (CheckIfInRange(transform.position, GameManager.instance.playerPosition, GetWeaponRange()) && action) Attack<Player>(player);
                else
                {
                    //move to player
                    Vector3 destination = new Vector3();
                    destination = DestinationPoint();
                    movementLeft = Move(transform.position, destination, movementLeft);
                }
                if (CheckIfInRange(transform.position, GameManager.instance.playerPosition, GetWeaponRange()) && action) Attack<Player>(player);
                EndEnemyTurn();
            }
        }
    }

    private void EndEnemyTurn()
    {
        print("It was my turn, id = " + id);
        GameManager.instance.turnScript.EndTurn();
        action = true;
        movementLeft = moveRange;
    }

    private Vector3 DestinationPoint()
    {
        Vector3 playerPosition = new Vector3();
        playerPosition = GameManager.instance.playerPosition;

        int distanceFromPlayer;
        distanceFromPlayer = Mathf.RoundToInt(Mathf.Abs(transform.position.x - playerPosition.x) + Mathf.Abs(transform.position.y - playerPosition.y));
        if (distanceFromPlayer > 1)
        {
            Vector3 destination = new Vector3();
            List<Vector3> adjecentSquares = new List<Vector3>();

            int[] distances = new int[4];
            int iter = 0;
            for (int i = -1; i < 2; i++)
                for (int j = -1; j < 2; j++)
                {
                    if (Mathf.Abs(i) != Mathf.Abs(j))
                    {
                        Vector3 possibleSquare = new Vector3();
                        possibleSquare.x = playerPosition.x + i;
                        possibleSquare.y = playerPosition.y + j;
                        RaycastHit2D hit = Physics2D.Linecast(possibleSquare, possibleSquare, 1 << LayerMask.NameToLayer("BlockingLayer"));
                        if (hit.transform == null)
                        {
                            adjecentSquares.Add(possibleSquare);
                            int distance;
                            List<Vector3> shortestPath = new List<Vector3>();
                            shortestPath = GetShortestPath(transform.position, possibleSquare);
                            distance = movingAlgorythm.ReturnPathMovementCost(shortestPath);
                            distances[iter] = distance;
                            iter++;
                        }
                    }
                }

            int smallestDistance = distances[0];
            int id = 0;
            for (int j = 0; j < iter; j++)
            {
                if (smallestDistance > distances[j])
                    id = j;
            }

            destination = adjecentSquares[id];

            return destination;
        }
        else return transform.position;
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
            GameManager.instance.turnScript.RemoveFromQueue(this.id);
        }
    }

    protected override void Attack<T>(T component)
    {
        Player target = component as Player;
        Vector3 targetLocation = target.transform.position;

        WeaponManager.Weapon weapon = new WeaponManager.Weapon();
        weapon = GameManager.instance.weaponScript.weaponList.Find(i => i.weaponId == equippedWeaponId);

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
