using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
    public LayerMask blockingLayer;

    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().enabled = true;
        GameManager.instance.mousePosition = gameObject.transform.position;
    }
    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
