using UnityEngine;

public class camera : MonoBehaviour {

    //THIS SCRIPT MAKES THE CAMERA MOVE WITH THE MOUVEMENT OF THE MOUSE

    public float sensibilityX;
    public float sensibilityY;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float controllerX = Input.GetAxis("4th axis");
        float controllerY = Input.GetAxis("5th axis");

        // Combine mouse and controller inputs
        mouseX = mouseX + controllerX;
        mouseY = mouseY + controllerY;

        float cantidadX = mouseX * sensibilityX;
        float cantidadY = mouseY * sensibilityY;

        Vector3 targetRot = transform.rotation.eulerAngles;

        targetRot.y += cantidadX;
        targetRot.x += cantidadY;
        transform.rotation = Quaternion.Euler(targetRot);
    }
}
