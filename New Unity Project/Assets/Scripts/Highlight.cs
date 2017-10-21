using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
    public static LayerMask blockingLayer;

    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }
}
