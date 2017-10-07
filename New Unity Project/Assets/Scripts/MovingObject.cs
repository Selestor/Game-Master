using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour {

    public float moveTime = 0.1f;
    public LayerMask blockingLayer;

    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private float inverseMoveTime;

    private APathAlgorythm movingAlgorythm;

    // Use this for initialization
    protected virtual void Start () {

        boxCollider = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
        movingAlgorythm = gameObject.AddComponent(typeof(APathAlgorythm)) as APathAlgorythm;

    }

    protected void Move (Vector3 start, Vector3 end)
    {
        List<Vector3> shortestPath = new List<Vector3>();

        boxCollider.enabled = false;
        shortestPath = movingAlgorythm.ReturnShortestPath(start, end);
        boxCollider.enabled = true;
        
        StartCoroutine(SmoothMovement(shortestPath));
    }

    protected virtual void SimpleMove(int xDir, int yDir)
    {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        int layerMask = 1 << 8;
        boxCollider.enabled = false;
        RaycastHit2D hit = Physics2D.Linecast(start, end, layerMask);
        boxCollider.enabled = true;

        if (hit.transform == null)
        {
            StartCoroutine(SimpleSmoothMovement(new Vector2(Mathf.RoundToInt(end.x), Mathf.RoundToInt(end.y))));
        }
        else print("There is smth in the way");
    }

    protected IEnumerator SimpleSmoothMovement(Vector3 end)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected IEnumerator SmoothMovement(List<Vector3> path)
    {
        path.RemoveAt(0);
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
    }

    protected abstract void Attack<T>(T component)
        where T : Component;
}
