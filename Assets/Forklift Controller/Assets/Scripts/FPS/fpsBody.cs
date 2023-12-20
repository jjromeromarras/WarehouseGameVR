using UnityEngine;

public class fpsBody : MonoBehaviour {

    //THIS SCRIPT IS A EXAMPLE FOR THIS ASSET, YOU CAN USE YOUR OWN SCRIPT

    public string horizontalInputName;
    public string verticalInputName;
    public float Speed;
    public float runSpeedMultiplicated;

    public Animator anim;

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
             anim.SetBool("IsRunning", true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            Speed = Speed / runSpeedMultiplicated;
            anim.SetBool("IsRunning", false);
        }

       
    }

    private void PlayerMovement()
    {
        float horizInput = Input.GetAxis(horizontalInputName) * Speed;
        float vertInput = Input.GetAxis(verticalInputName) * Speed;

        Vector3 forwardMovement = transform.forward * vertInput;
        Vector3 rightMovement = transform.right * horizInput;

        charController.SimpleMove(forwardMovement + rightMovement);
        anim.SetBool("IsWalker", forwardMovement.magnitude + rightMovement.magnitude>0);
    }

}
