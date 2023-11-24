using UnityEngine;

public class fpsBody : MonoBehaviour {

    //THIS SCRIPT IS A EXAMPLE FOR THIS ASSET, YOU CAN USE YOUR OWN SCRIPT

    public string horizontalInputName;
    public string verticalInputName;
    public float Speed;
    public float runSpeedMultiplicated;

    public CharacterController charController;

    public void Start()
    {
        charController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerMovement();

        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Speed = Speed * runSpeedMultiplicated;
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Speed = Speed / runSpeedMultiplicated;
        }
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * Speed;
        float vertInput = Input.GetAxis(verticalInputName) * Speed;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);
    }

}
