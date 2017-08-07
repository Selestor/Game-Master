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

    protected IEnumerator SmoothMovement(List<Vector3> path)
    {
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
