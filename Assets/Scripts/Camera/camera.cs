using UnityEngine;

public class camera : MonoBehaviour {

    //THIS SCRIPT MAKES THE CAMERA MOVE WITH THE MOUVEMENT OF THE MOUSE

    public float sensibilityX;
    public float sensibilityY;

    void Update()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        float cantidadX = mouseX * sensibilityX;
        float cantidadY = mouseY * sensibilityY;

        Vector3 targetRot = transform.rotation.eulerAngles;

        targetRot.y += cantidadX;
        targetRot.x += cantidadY;
        transform.rotation = Quaternion.Euler(targetRot);
    }
}
