using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float speed = 5.0f;
    void Update()
    {
        /* scrolling
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) //scroll down
        {
            if (transform.position.z < -5)
                transform.position += new Vector3(0, 0, 1);
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) //scroll up
        {
            if (transform.position.z > -15)
                transform.position -= new Vector3(0, 0, 1);
        }
        */

        if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position -= new Vector3(speed * Time.deltaTime, 0, 0);
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            transform.position -= new Vector3(0, speed * Time.deltaTime, 0);
        }
        if (Input.GetKey(KeyCode.UpArrow))
        {
            transform.position += new Vector3(0, speed * Time.deltaTime, 0);
        }
    }
}
