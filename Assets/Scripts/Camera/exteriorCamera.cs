using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class exteriorCamera : MonoBehaviour
{

    public Transform lookAt;
    public float distance = 10.0f;

    private float X = 0.0f;
    private float Y = 45.0f;
    public float sensitivityX = 4.0f;
    public float sensitivityY = 1.0f;

    private void Update()
    {
        X += Input.GetAxis("Mouse X");
        Y += Input.GetAxis("Mouse Y");

        //change the camera distance with the mouse scroll wheel
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            distance++;
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && distance > 1)
        {
            distance--;
        }
    }

    private void LateUpdate()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(-Y, X, 0);

        //makes the camera look the target
        transform.position = lookAt.position + rotation * dir;
        transform.LookAt(lookAt.position);
    }
}