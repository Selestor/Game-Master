using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {

    public float speed = 5.0f;
    void Update()
    {
        if(Input.GetAxis("Mouse ScrollWheel") > 0f) //scroll down
        {
            if (Camera.main.orthographicSize > 5)
                Camera.main.orthographicSize--;
        }
        if (Input.GetAxis("Mouse ScrollWheel") < 0f) //scroll up
        {
            if (Camera.main.orthographicSize < 10)
                Camera.main.orthographicSize++;
        }

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
