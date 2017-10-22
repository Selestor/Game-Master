using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject {
    public bool action = true;
    public int movementLeft;

    protected override void Start ()
    {
        base.Start();
        
        str = 1;
        dex = 1;
        healthPoints = 8;
        baseArmor = 10;
        moveRange = 6;

        equippedWeaponId = 1;

        id = 0;

        movementLeft = 6;
        action = true;
    }

    private void OnDisable()
    {
        //tutaj jakby cos sie zmienialo, np poziom gry
    }
    
    void Update ()
    {
        //transform.position = new Vector3() { x = 0, y = 0 };
        if (GameManager.instance.WhosTurn() == id)
        {
            if (!isMoving)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (movementLeft > 0)
                    {
                        Vector3 pointerPosition = GameManager.instance.mousePosition;
                        BoardManager boardManager = GameManager.instance.boardScript;

                        RaycastHit2D skullHit = Physics2D.Linecast(pointerPosition, pointerPosition, 1 << LayerMask.NameToLayer("BlockingLayer"));
                        if (skullHit.transform == null) // checking if skull
                        {
                            Vector3 end = GameManager.instance.mousePosition;
                            if (end.x < 0 || end.x > boardManager.rows - 1 || end.y < 0 || end.y > boardManager.columns - 1 || transform.position == end)
                                return;
                            else
                            {
                                movementLeft = Move(transform.position, end, movementLeft);
                            }
                        }
                        else print("You cant move here.");
                    }
                    else print("You cant move this turn.");
                }

                if (Input.GetMouseButtonDown(1))
                {
                    if (action)
                    {
                        Vector3 pointerPosition = GameManager.instance.mousePosition;
                        int layerMask = 1 << 8;
                        RaycastHit2D hit = Physics2D.Linecast(pointerPosition, pointerPosition, layerMask);
                        if (hit && hit.transform.name == "Enemy(Clone)")
                        {
                            Attack<Enemy>(hit.transform.GetComponent<Enemy>());
                        }
                    }
                    else print("You cant attack this turn.");
                }

                if (Input.GetKeyDown("space"))
                {
                    GameManager.instance.turnScript.EndTurn();
                    movementLeft = moveRange;
                    action = true;
                    print("You finished your turn.");
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (action)
                    {
                        SwapWeapon();
                    }
                    else print("You cant swap weapon this turn.");
                }
                int horizontal = 0;
                int vertical = 0;

                horizontal = (int)Input.GetAxisRaw("Horizontal");
                vertical = (int)Input.GetAxisRaw("Vertical");

                if (horizontal != 0 || vertical != 0)
                {
                    if (movementLeft > 0)
                    {
                        movementLeft = SimpleMove(horizontal, vertical, movementLeft);
                    }
                    else print("You cant move this turn");
                }
            }
        }
    }

    protected override void Attack<T>(T component)
    {
        Enemy target = component as Enemy;
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

    private void SwapWeapon()
    {
        if (equippedWeaponId == 0) equippedWeaponId = 1;
        else equippedWeaponId = 0;
        print("You equipped weapon " +  equippedWeaponId);
        action = false;
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
