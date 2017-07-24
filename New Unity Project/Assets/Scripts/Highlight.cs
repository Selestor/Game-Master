using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {

    public Vector3 positionOnGrid;

    void OnMouseEnter()
    {
        GetComponent<SpriteRenderer>().enabled = true;
    }
    void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().enabled = false;
    }

    void GetMousePosition()
    {
        Vector3 mousePosition = gameObject.transform.position;
        GameManager.instance.mousePosition = mousePosition;
    }
}
