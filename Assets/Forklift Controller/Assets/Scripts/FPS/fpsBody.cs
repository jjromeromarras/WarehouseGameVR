using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class fpsBody : MonoBehaviour {

    public static fpsBody instance;

    public string horizontalInputName;
    public string verticalInputName;
    public float Speed;
    public float runSpeedMultiplicated;
    public Animator anim;
    public CharacterController charController;
    public GameObject crossHair;
    public float resetTime;
    public Color scannColor;

    private float timer;
    private Color currentColorCrossHair;
    private bool isScanning;
    private bool islocked;

    public void Awake()
    {
        instance = this;
    }

    
    public void Start()
    {
        charController = GetComponent<CharacterController>();
        timer = resetTime;
        currentColorCrossHair = Color.white;
        islocked = isScanning = false;
    }

    private void Update()
    {
        if (!islocked)
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

            if (Input.GetMouseButton(0))
            {
                isScanning = true;
                timer -= Time.deltaTime;
                if (timer < 0f)
                {
                    if (currentColorCrossHair == Color.white) currentColorCrossHair = Color.red; else currentColorCrossHair = Color.white;
                    this.ScannerAtPoint(currentColorCrossHair);
                    timer = resetTime;
                }
            }
            else
            {
                currentColorCrossHair = Color.white;
                timer = resetTime;
                isScanning = false;
                this.ScannerAtPoint(currentColorCrossHair);
            }
        }
    }

    private void ScannerAtPoint(Color color){
        if (crossHair != null)
        {
            Image[] images = crossHair.GetComponentsInChildren<Image>();
            for (int i = 0; i < images.Length; i++)
            {
                images[i].color = color;
            }
            if (isScanning)
            {


                RaycastHit rayHit;
                Ray currentRay = Camera.allCameras[0].ScreenPointToRay(crossHair.transform.position);
                if (Physics.Raycast(currentRay, out rayHit))
                {
                    if (rayHit.collider.tag == "Ubicacion" && rayHit.distance <= 3)
                    {
                        TextMeshPro ubicacion = rayHit.transform.GetChild(0).gameObject.GetComponents<TextMeshPro>()[0];
                        Debug.Log("Colision con ubicación: " + ubicacion.text);
                    }
                }
            }
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

    public void setLock(bool value)
    {
        islocked = value;
    }
}
