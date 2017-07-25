using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour {
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
