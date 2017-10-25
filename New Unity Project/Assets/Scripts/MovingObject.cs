using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;
    public bool isMoving;

    public APathAlgorythm movingAlgorythm;

    public int str;
    public int dex;
    public int baseArmor;
    public int healthPoints;
    public int moveRange;
    public int equippedWeaponId;

    public int id;

    public bool action;
    public int movementLeft;

    // Use this for initialization
    protected virtual void Start () {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        movingAlgorythm = gameObject.AddComponent(typeof(APathAlgorythm)) as APathAlgorythm;
        isMoving = false;
    }

    protected List<Vector3> GetShortestPath(Vector3 start, Vector3 end)
    {
        List<Vector3> shortestPath = new List<Vector3>();

        boxCollider.enabled = false;
        shortestPath = movingAlgorythm.ReturnShortestPath(start, end);
        boxCollider.enabled = true;

        return shortestPath;
    }

    protected int Move (Vector3 start, Vector3 end, int movementLeft)
    {
        List<Vector3> shortestPath = new List<Vector3>();
        List<Vector3> newShortestPath = new List<Vector3>();


        shortestPath = GetShortestPath(start, end);
        /*
        boxCollider.enabled = false;
        shortestPath = movingAlgorythm.ReturnShortestPath(start, end);
        boxCollider.enabled = true;
        */
        /*
        if (shortestPath.Count - 1 >= movementLeft)
        {
            shortestPath.RemoveRange(movementLeft + 1, shortestPath.Count - movementLeft - 1);
            movementLeft = 0;
        }
        else movementLeft = movementLeft - (shortestPath.Count - 1);
        StartCoroutine(SmoothMovement(shortestPath));
        return movementLeft;
        */
        RaycastHit2D puddleHit = Physics2D.Linecast(transform.position, transform.position, 1 << LayerMask.NameToLayer("Obstacle"));
        int movementCost;
        if (puddleHit.transform == null) movementCost = 1;
        else movementCost = GameManager.instance.puddleCost;

        foreach (Vector3 step in shortestPath)
        {
            if (movementLeft >= movementCost)
            {
                newShortestPath.Add(step);
                movementLeft -= movementCost;

                puddleHit = Physics2D.Linecast(step, step, 1 << LayerMask.NameToLayer("Obstacle"));
                if (puddleHit.transform == null) movementCost = 1;
                else movementCost = GameManager.instance.puddleCost;
            }
        }
        StartCoroutine(SmoothMovement(newShortestPath));
        return movementLeft;
    }

    protected IEnumerator SmoothMovement(List<Vector3> path)
    {
        GameManager.instance.isAnythingMoving = true;
        foreach (Vector3 step in path)
        {
            float sqrRemainingDistance = (transform.position - step).sqrMagnitude;

            while (sqrRemainingDistance > float.Epsilon)
            {
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, step, inverseMoveTime * Time.deltaTime);
                rb2D.MovePosition(newPosition);
                sqrRemainingDistance = (transform.position - step).sqrMagnitude;
                yield return null;
            }
        }
        GameManager.instance.isAnythingMoving = false;
        if (this.tag == "Player") GameManager.instance.playerPosition = transform.position;
    }

    protected virtual int SimpleMove(int xDir, int yDir, int movementLeft)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);
        
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, 1 << LayerMask.NameToLayer("BlockingLayer"));
        RaycastHit2D puddleHit = Physics2D.Linecast(start, start, 1 << LayerMask.NameToLayer("Obstacle"));
        boxCollider.enabled = true;

        int movemententCost;
        if (puddleHit.transform == null) movemententCost = 1;
        else movemententCost = GameManager.instance.puddleCost;

        if (movementLeft >= movemententCost)
        {
            if (hit.transform == null)
            {
                StartCoroutine(SimpleSmoothMovement(new Vector2(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y))));
                movementLeft -= movemententCost;
            }
            else print("There is smth in the way");
        }
        else print("You cant move this turn.");
        return movementLeft;
    }

    protected IEnumerator SimpleSmoothMovement(Vector3 end)
    {
        GameManager.instance.isAnythingMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        GameManager.instance.isAnythingMoving = false;
        if (this.tag == "Player") GameManager.instance.playerPosition = transform.position;
    }
    
    protected bool CheckIfInRange(Vector3 userPosition, Vector3 targetPosition, int weaponRange)
    {
        float distance = Mathf.Abs(targetPosition.x - userPosition.x) + Mathf.Abs(targetPosition.y - userPosition.y);

        if (distance > weaponRange) return false;
        else return true;
    }

    protected int GetWeaponRange()
    {
        WeaponManager.Weapon weapon = new WeaponManager.Weapon();
        weapon = GameManager.instance.weaponScript.weaponList.Find(i => i.weaponId == equippedWeaponId);
        return weapon.range;
    }

    protected abstract void Attack<T>(T component)
        where T : Component;
}
