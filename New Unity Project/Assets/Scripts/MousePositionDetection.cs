using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MousePositionDetection : MonoBehaviour {
    private void OnMouseEnter()
    {
        GameManager.instance.mousePosition = gameObject.transform.position;
        //print(gameObject.transform.position);
    }
}
