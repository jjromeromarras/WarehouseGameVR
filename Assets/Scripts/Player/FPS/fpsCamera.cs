using UnityEngine;

public class fpsCamera : MonoBehaviour {


    //THIS SCRIPT IS A EXAMPLE FOR THIS ASSET, YOU CAN USE YOUR OWN SCRIPT

    public string mouseXInputName, mouseYInputName;
    public float mouseSensitivity;

    public Transform playerBody;
    private bool _lock;
    private float limit;

    private void Awake()
    {
        _lock = false;
          limit = 0.0f;
    }

    private void Update()
    {
        if (!_lock)
        {
            CameraRotation();
        }
    }

    private void CameraRotation()
    {
        
        //Vector3 mouseInput = new Vector3(Input.GetAxis(mouseXInputName), Input.GetAxis(mouseYInputName));

        //playerBody.transform.rotation = Quaternion.Euler(playerBody.transform.eulerAngles + new Vector3(0f,mouseX,0f));
        //transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(mouseY,0f,0f));
        
        float mouseX = Input.GetAxis(mouseXInputName) * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis(mouseYInputName) ;

        // Movimiento de la cámara con el joystick derecho
        float controllerX = Input.GetAxis("4th axis") * mouseSensitivity * Time.deltaTime;
        float controllerY = Input.GetAxis("5th axis");

        // Combine mouse and controller inputs
        mouseX = mouseX + controllerX;
        mouseY = mouseY + controllerY;

        limit += mouseY;

        if (limit > 90.0f)
        {
            limit = 90.0f;
            mouseY = 0.0f;
            limitRotationToValue(270.0f);
        }
        else if (limit < -90.0f)
        {
            limit = -90.0f;
            mouseY = 0.0f;
            limitRotationToValue(90.0f);
        }
        
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(-mouseY,0f,0f));
        // transform.Rotate(Vector3.left * mouseY);
        playerBody.Rotate(Vector3.up * mouseX);
    }

    private void limitRotationToValue(float value)
    {
        Vector3 eulerRotation = transform.eulerAngles;
        eulerRotation.x = value;
        transform.eulerAngles = eulerRotation;
    }

    public void SetLock(bool value)
    {
        _lock = value;
    }
}
