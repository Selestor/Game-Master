using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour {
    public Vector3 target;

    public void Shoot()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, 10 * Time.deltaTime);
    }

    private void Update()
    {
        Shoot();
        if (transform.position == target)
        {
            Destroy(gameObject);
        }
    }
}
