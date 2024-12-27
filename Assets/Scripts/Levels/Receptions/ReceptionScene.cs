using UnityEngine;

public class ReceptionScene: MonoBehaviour
{   
    [SerializeField] public containerpicking stock;
    [SerializeField] public GameObject pallet;
    public float rotationSpeed = 100f; // Velocidad de rotación ajustable
  

    private void Update()
    {
        // Rotación hacia la izquierda (Q) y hacia la derecha (E)
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.Joystick1Button4))
        {
            pallet.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime, Space.World);
        }
        else if (Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.Joystick1Button5))
        {
            pallet.transform.Rotate(Vector3.up, -rotationSpeed * Time.deltaTime, Space.World);
        }
    }
}
