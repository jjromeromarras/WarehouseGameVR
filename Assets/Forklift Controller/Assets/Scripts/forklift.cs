using UnityEngine;
using TMPro;

public class forklift : MonoBehaviour
{
    [Header("UI elements")]
    public TextMeshProUGUI gearText;
    public TextMeshProUGUI speedText;
    public GameObject canEnterText;
    public GameObject inForkliftMenu;

    [Header("WHEEL COLLIDERS")]
    public WheelCollider frontR;
    public WheelCollider frontL;
    public WheelCollider rearR;
    public WheelCollider rearL;

    [Header("WHEEL TRANSFORMS")]
    public Transform frontRightT;
    public Transform frontLeftT;
    public Transform rearRightT;
    public Transform rearLeftT;

    public Transform exitPosition;
    public Transform loader;
    public Transform centerOfMass;
    public GameObject steeringWheel;
    public Rigidbody rb;
    public GameObject cameraInteriorForklift;
    public GameObject cameraExteriorForklift;
    public GameObject FPS;

    [Header("VALUES")]
    public float torque;
    public float brakeTorque;
    public float maxSteerAngle;
    public float currentSpeed;
    public float maxSpeed;

    [Header("KeyCodes")]
    public KeyCode upGearKey = KeyCode.E;
    public KeyCode downGearKey = KeyCode.Q;
    public KeyCode changeCameraKey = KeyCode.C;
    public KeyCode upLoaderKey = KeyCode.O;
    public KeyCode downLoaderKey = KeyCode.L;

    [Header("PNJ")]
    public bool isPnj;
    public Transform[] waypoints;

    [Range(-1, 1)]
    int currentGear = 0;
    bool canEnter = false;
    bool enter = false;
    float maxPositionY = 3.5f;

    private int indexWaypointActual = 0;
    private int margenError = 1;
    private bool pnjdetenido = false;
    //when the player is close to the forklift
    private void OnTriggerEnter(Collider other)
    {
        if (!isPnj)
        {
            if (other.CompareTag("Player"))
            {
                //it can enter
                canEnterText.SetActive(true);
                canEnter = true;
            }
        } else {
            if (other.CompareTag("Player") || other.CompareTag("PNJ"))
            {
                pnjdetenido = true;
                frontL.brakeTorque = brakeTorque;
                frontR.brakeTorque = brakeTorque;
                rearL.brakeTorque = brakeTorque;
                rearR.brakeTorque = brakeTorque;
            }
        }
    }

    //when the player is far away of the forklift
    private void OnTriggerExit(Collider other)
    {
        if (!isPnj)
        {
            if (other.CompareTag("Player"))
            {
                //it can not enter
                canEnterText.SetActive(false);
                canEnter = false;
            }
        } else{
                pnjdetenido = false;
                frontL.brakeTorque = 0;
                frontR.brakeTorque = 0;
                rearL.brakeTorque = 0;
                rearR.brakeTorque = 0;
        }
    }

    private void Start()
    {
        //ignore that
        Application.targetFrameRate = 60;

        //aply the center of mass
        rb.centerOfMass = new Vector3(
            rb.centerOfMass.x,
            centerOfMass.position.y,
            rb.centerOfMass.z
        );
    }

    void Update()
    {
        currentSpeed = rb.velocity.magnitude;
        if (!isPnj)
        {
            //if can enter and press F key and is not in
            if (canEnter == true && Input.GetKeyDown(KeyCode.F) && enter == false)
            {
                //then enter the forklift
                FPS.SetActive(false);
                cameraInteriorForklift.SetActive(true);
                cameraExteriorForklift.SetActive(false);
                canEnterText.SetActive(false);
                enter = true;
                inForkliftMenu.SetActive(true);
            }

            //if is not enter, execute again the update method
            if (enter == false)
                return;

           

            if (Input.GetAxis("Vertical") != 0)
            {
                if (Input.GetAxis("Vertical") > 0)
                {
                    //aply brake torque
                    //frontL.brakeTorque = brakeTorque;
                    //frontR.brakeTorque = brakeTorque;
                    //rearL.brakeTorque = brakeTorque;
                    //rearR.brakeTorque = brakeTorque;
                    currentGear = 1;
                }
                else
                {
                    currentGear = -1;
                    //frontL.brakeTorque = 0;
                    //frontR.brakeTorque = 0;
                    //rearL.brakeTorque = 0;
                    //rearR.brakeTorque = 0;
                }
                frontL.brakeTorque = 0;
                frontR.brakeTorque = 0;
                rearL.brakeTorque = 0;
                rearR.brakeTorque = 0;

                if (currentSpeed < maxSpeed)
                {
                    //aply motor torque
                    frontL.motorTorque = Input.GetAxis("Vertical") * torque;
                    frontR.motorTorque = Input.GetAxis("Vertical") * torque;
                    rearL.motorTorque = Input.GetAxis("Vertical") * torque;
                    rearR.motorTorque = Input.GetAxis("Vertical") * torque;
                }
                else
                {
                    frontL.motorTorque = 0;
                    frontR.motorTorque = 0;
                    rearL.motorTorque = 0;
                    rearR.motorTorque = 0;
                }
            }
            else
            {
                currentGear = 0;
                frontL.brakeTorque = brakeTorque;
                frontR.brakeTorque = brakeTorque;
                rearL.brakeTorque = brakeTorque;
                rearR.brakeTorque = brakeTorque;
            }

            //make the wheels turn
            rearL.steerAngle = -maxSteerAngle * Input.GetAxis("Horizontal");
            rearR.steerAngle = -maxSteerAngle * Input.GetAxis("Horizontal");
            steeringWheel.transform.localEulerAngles = new Vector3(53f, 0f, rearL.steerAngle * 6);

            //up the loader
            if (Input.GetKey(upLoaderKey) && loader.position.y < maxPositionY)
            {
                loader.Translate(new Vector3(0f, 1f, 0f) * Time.deltaTime);
            }

            //down the loader
            if (Input.GetKey(downLoaderKey) && loader.position.y > 0)
            {
                loader.Translate(new Vector3(0f, -1f, 0f) * Time.deltaTime);
            }

            //update wheel poses
            UpdateWheelPoses();

            //if press Z key
            if (Input.GetKeyDown(KeyCode.Z))
            {
                //exit the forklift
                inForkliftMenu.SetActive(false);
                enter = false;
                FPS.transform.position = exitPosition.position;
                FPS.SetActive(true);
                cameraInteriorForklift.SetActive(false);
                cameraExteriorForklift.SetActive(false);
                canEnterText.SetActive(false);
            }

            if (Input.GetKeyDown(upGearKey))
            {
                if (currentGear < 1)
                {
                    currentGear++;
                }
            }

            if (Input.GetKeyDown(downGearKey))
            {
                if (currentGear > -1)
                {
                    currentGear--;
                }
            }

           /* if (Input.GetKeyDown(changeCameraKey))
            {
                if (cameraInteriorForklift.activeSelf)
                {
                    cameraInteriorForklift.SetActive(false);
                    cameraExteriorForklift.SetActive(true);
                }
                else
                {
                    cameraInteriorForklift.SetActive(true);
                    cameraExteriorForklift.SetActive(false);
                }
            }*/

            //update texts
            gearText.text = "Gear: " + currentGear;
            speedText.text = "Speed: " + currentSpeed.ToString("f2") + "Km/h";
        }
        else
        {
            // PNJ
            movePNJ();
        }
    }

    private void movePNJ()
    {
        if (waypoints.Length > 0 && indexWaypointActual < waypoints.Length && !pnjdetenido)
        {
            // Calcula la distancia entre el camión y el waypoint actual
            float distancia = Vector3.Distance(transform.position, waypoints[indexWaypointActual].position);

            if (distancia > margenError)
            {
                // Obtenemos la dirección hacia el waypoint actual
                Vector3 direccion = waypoints[indexWaypointActual].position - transform.position;

                // Rotamos la carretilla hacía el punto
                Quaternion rotacion = Quaternion.LookRotation(direccion);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotacion, Time.deltaTime * 3f);

                // Mueve el camión hacia el waypoint actual
                // transform.position = Vector3.MoveTowards(transform.position, waypoints[indexWaypointActual].position, currentSpeed * Time.deltaTime);

                if (currentSpeed < 2)
                {
                    //aply motor torque
                    frontL.motorTorque = torque;
                    frontR.motorTorque = torque;
                    rearL.motorTorque = torque;
                    rearR.motorTorque = torque;
                }
                else
                {
                    frontL.motorTorque = 0;
                    frontR.motorTorque = 0;
                    rearL.motorTorque = 0;
                    rearR.motorTorque = 0;
                }

                //update wheel poses
                UpdateWheelPoses();
                // Si el camión ha alcanzado el waypoint actual, pasa al siguiente
            }
            else
            {
                indexWaypointActual++;
                // Verifica si hay más waypoints, si no, reinicia el recorrido
                if (indexWaypointActual >= waypoints.Length)
                {
                    indexWaypointActual = 0; // Reinicia al primer waypoint
                    // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
                }
            }
        }
    }

    private void UpdateWheelPoses()
    {
        //update wheels position
        UpdateWheelPose(frontR, frontRightT);
        UpdateWheelPose(frontL, frontLeftT);
        UpdateWheelPose(rearL, rearLeftT);
        UpdateWheelPose(rearR, rearRightT);
    }

    private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
    {
        //makes the whhels turn to the same speed as the wheel collider
        Vector3 _pos = _transform.position;
        Quaternion _quat = _transform.rotation;

        _collider.GetWorldPose(out _pos, out _quat);

        _transform.rotation = _quat.normalized;
    }
}
