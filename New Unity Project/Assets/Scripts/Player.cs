using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MovingObject {
    public Text healthText;
    public Text movementText;
    public Text weaponText;
    public Text actionText;
    
    protected override void Start ()
    {
        base.Start();
        WeaponInformation();
    }
    
    private void UpdateUI()
    {
        healthText.text = "Health: " + healthPoints;
        movementText.text = "Movement: " + movementLeft;
        weaponText.text = "Weapon: " + weapon.name;
        if (action) actionText.text = "Action: -ready-";
        if (!action) actionText.text = "Action: -none-";
    }

    void Update ()
    {
        UpdateUI();
        if (GameManager.instance.WhosTurn() == id)
        {
            Vector3 pointerPosition = GameManager.instance.mousePosition;
            if (!GameManager.instance.isAnythingMoving)
            {
                if (Input.GetMouseButtonDown(0) && !GameManager.instance.paused)
                {
                    if (movementLeft > 0)
                    {
                        BoardManager boardManager = GameManager.instance.boardScript;
                        RaycastHit2D skullHit = Physics2D.Linecast(pointerPosition, pointerPosition, 1 << LayerMask.NameToLayer("BlockingLayer"));
                        if (skullHit.transform == null) // checking if skull
                        {
                            if (pointerPosition.x < 0 || pointerPosition.x > boardManager.rows - 1 || pointerPosition.y < 0 || pointerPosition.y > boardManager.columns - 1 || transform.position == pointerPosition)
                                return;
                            else
                            {
                                movementLeft = Move(transform.position, pointerPosition, movementLeft);
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
                    EndTurn("Player finished his turn.");
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    if (action)
                    {
                        SwapWeapon();
                    }
                    else print("You cant swap weapon this turn.");
                }

                /* Moving with arrows
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
                */
            }
        }
    }

    public void EndPlayerTurn()
    {
        if (GameManager.instance.WhosTurn() == id)
        {
            EndTurn("Player finished his turn.");
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

    private void SwapWeapon()
    {
        if (equippedWeaponId == 0) equippedWeaponId = 1;
        else equippedWeaponId = 0;
        //print("You equipped weapon " +  equippedWeaponId);
        action = false;
        WeaponInformation();
    }

    private void CheckIfGameOver()
    {
        if (healthPoints <= 0)
        {
            GameManager.instance.GameOver();
            UpdateUI();
            healthText.text = "DEAD";
            Destroy(gameObject);
        }
    }

    public void LoseHealth(int loss)
    {
        FloatingTextController.CreateFloatingText(loss.ToString(), transform);
        //healthPoints -= loss;
        healthPoints -= 0;
        CheckIfGameOver();
    }
}
