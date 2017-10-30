using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
    public static LayerMask blockingLayer;

    private void Update()
    {
        if (GameManager.instance.mousePosition == transform.position)
            GetComponent<SpriteRenderer>().enabled = true;
        else GetComponent<SpriteRenderer>().enabled = false;
    }
    /*
    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
    */
}
