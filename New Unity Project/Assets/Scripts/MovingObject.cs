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

    private APathAlgorythm movingAlgorythm;

    public int str;
    public int dex;
    public int baseArmor;
    public int healthPoints;
    public int moveRange;
    public int equippedWeaponId;

    public int id;

    // Use this for initialization
    protected virtual void Start () {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        movingAlgorythm = gameObject.AddComponent(typeof(APathAlgorythm)) as APathAlgorythm;
        isMoving = false;
    }

    protected int Move (Vector3 start, Vector3 end, int movementLeft)
    {
        List<Vector3> shortestPath = new List<Vector3>();
        List<Vector3> newShortestPath = new List<Vector3>();

        boxCollider.enabled = false;
        shortestPath = movingAlgorythm.ReturnShortestPath(start, end);
        boxCollider.enabled = true;

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
        isMoving = true;
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
        isMoving = false;
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

        if(movementLeft < movemententCost)
        if (hit.transform == null)
        {
            StartCoroutine(SimpleSmoothMovement(new Vector2(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y))));
            movementLeft -= movemententCost;
        }
        else print("There is smth in the way");
        return movementLeft;
    }

    protected IEnumerator SimpleSmoothMovement(Vector3 end)
    {
        isMoving = true;
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        isMoving = false;
    }
    
    protected bool CheckIfInRange(Vector3 userPosition, Vector3 targetPosition, int weaponRange)
    {
        float distance = Mathf.Abs(targetPosition.x - userPosition.x) + Mathf.Abs(targetPosition.y - userPosition.y);

        if (distance > weaponRange) return false;
        else return true;
    }

    protected abstract void Attack<T>(T component)
        where T : Component;
}
