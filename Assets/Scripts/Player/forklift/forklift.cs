using UnityEngine;
using TMPro;
using Assets.Scripts.Helper;
using UnityEngine.AI;
using System.Collections;

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
    public GameObject box;

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
    public orderwalker[] waypoints;
    public Transform loaderpos1;
    
    [Range(-1, 1)]
    int currentGear = 0;
    bool canEnter = false;
    bool enter = false;
    float maxPositionY = 3.5f;

    #region Private Fields
    private int indexWaypointActual = 0;
    private int margenError = 1;
    private bool pnjdetenido = false;
    private ForkLiftState state;
    
    private float distanciaRecorrida = 0f;
    private float velocidad = 5f; // Velocidad de avance
    private GameObject palletloadpos = null;
    private Vector3 positionorigpallet;
    private Quaternion rotacionorigpallet;
    private NavMeshAgent agente;

    #endregion
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
    private void Awake()
    {
        state = ForkLiftState.eMove;
        agente = GetComponent<NavMeshAgent>();
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
                    currentGear = 1;
                }
                else
                {
                    currentGear = -1;                   
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
        }
        else
        {
            // PNJ
            UpdatePNJ();
        }
    }

    private void MovePNJ()
    {
        if (waypoints.Length > 0 && indexWaypointActual < waypoints.Length && !pnjdetenido)
        {           
            float distancia = Vector3.Distance(transform.position, waypoints[indexWaypointActual].waypoint.position);

            if (distancia > margenError)
            {

                frontL.motorTorque = 0;
                frontR.motorTorque = 0;
                rearL.motorTorque = 0;
                rearR.motorTorque = 0;
                agente.enabled = true;
                agente.destination = waypoints[indexWaypointActual].waypoint.position;

            }
            else
            {
              
                agente.enabled = false;
                // Comprobamos si un movimiento de carga o descarga
                if (waypoints[indexWaypointActual].isloading)
                {
                    palletloadpos = waypoints[indexWaypointActual].pallettoload;
                    rotacionorigpallet = palletloadpos.transform.rotation;
                    positionorigpallet = palletloadpos.transform.position;
                    state = ForkLiftState.eRotate;
                    rb.velocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
                    // Obtenemos la dirección hacia el waypoint actual
                    Vector3 direccion = positionorigpallet - transform.position;

                    // Rotamos la carretilla hacía el punto
                    Quaternion rotacion = Quaternion.LookRotation(direccion);
                             
                    StartCoroutine(RotarHaciaCaja(rotacion));
                }
                else
                {
                    if (waypoints[indexWaypointActual].isunloading)
                        state = ForkLiftState.eUnLoadingPrepareElevator;
                    else
                        CheckNextOrden();
                }
                
            }            
        }
    }
    IEnumerator RotarHaciaCaja(Quaternion rotacionObjetivo)
    {
        float duracionRotacion = 30f; // Ajusta según sea necesario
        float tiempoPasado = 0f;           
        while (tiempoPasado < duracionRotacion)
        {
            // Usa Quaternion.RotateTowards para suavizar la rotación
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionObjetivo, tiempoPasado / duracionRotacion);// Time.deltaTime * 3f);
            // Controla la inclinación del objeto
            Vector3 eulerAngles = transform.eulerAngles;
            eulerAngles.x = 0f;
            eulerAngles.z = 0f;
            transform.eulerAngles = eulerAngles;

            tiempoPasado += Time.deltaTime;
        }

        // Esperar un tiempo antes de continuar el movimiento
        yield return new WaitForSeconds(1f); // Ajusta según sea necesario

        // Reanudar el movimiento hacia la posición de la caja
       state = ForkLiftState.eLoadingPrepareElevator;
    }

    private void LoadingPallet()
    {
        palletloadpos.GetComponent<Rigidbody>().isKinematic = true;
        palletloadpos.SetActive(false);
        box.SetActive(true);
        currentGear = 0;
        state = ForkLiftState.eLoadingSavePosition;        
    }

    private  void LoadingSavePosition()
    {
        // La carretilla baja las palas con la carga a posición segura para moverse
        frontL.motorTorque = 0;
        frontR.motorTorque = 0;
        rearL.motorTorque = 0;
        rearR.motorTorque = 0;
        frontL.brakeTorque = brakeTorque;
        frontR.brakeTorque = brakeTorque;
        rearL.brakeTorque = brakeTorque;
        rearR.brakeTorque = brakeTorque;
        UpdateWheelPoses();
        if (loader.position.y > 1)
        {
            loader.Translate(new Vector3(0f, -1f, 0f) * Time.deltaTime);
        }
        else
        {
            frontL.brakeTorque = 0;
            frontR.brakeTorque = 0;
            rearL.brakeTorque = 0;
            rearR.brakeTorque = 0;
            state = ForkLiftState.eMove;
            indexWaypointActual++;
            // Verifica si hay más waypoints, si no, reinicia el recorrido
            if (indexWaypointActual >= waypoints.Length)
            {
                indexWaypointActual = 0; // Reinicia al primer waypoint
                                         // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
            }
        }
    }

    private void LoadingPrepareElevator ()
    {
        // carretilla posiciona la palas a la altura de la paleta
        frontL.motorTorque = 0;
        frontR.motorTorque = 0;
        rearL.motorTorque = 0;
        rearR.motorTorque = 0;
        frontL.brakeTorque = brakeTorque;
        frontR.brakeTorque = brakeTorque;
        rearL.brakeTorque = brakeTorque;
        rearR.brakeTorque = brakeTorque;
        UpdateWheelPoses();
        if (loader.position.y < (waypoints[indexWaypointActual].pallettoload.transform.position.y-0.05f) && loader.position.y < maxPositionY)
        {
            loader.Translate(new Vector3(0f, 1f, 0f) * Time.deltaTime);
        }
        else
        {
            frontL.brakeTorque = 0;
            frontR.brakeTorque = 0;
            rearL.brakeTorque = 0;
            rearR.brakeTorque = 0;
            state = ForkLiftState.eLoadingPallet;
        }
    }

    private void LoadingElevatorPallet ()
    {
        // carretilla eleva la carga de la estanteria
        if (loader.position.y < maxPositionY)
        {
            loader.Translate(new Vector3(0f, 1f, 0f) * Time.deltaTime);
        }
        else
        {
            frontL.brakeTorque = 0;
            frontR.brakeTorque = 0;
            rearL.brakeTorque = 0;
            rearR.brakeTorque = 0;
            state = ForkLiftState.eLoadingRetirePosition;
        }
    }


    private void UnLoadingPrepareElevator()
    {
        // La carretilla baja la carga al suelo
        rb.velocity = Vector3.zero;
        StopForkList();
        UpdateWheelPoses();
        if (loader.position.y > 0f)
        {
            loader.Translate(new Vector3(0f, -1f, 0f) * Time.deltaTime);
        }
        else
        {
            frontL.brakeTorque = 0;
            frontR.brakeTorque = 0;
            rearL.brakeTorque = 0;
            rearR.brakeTorque = 0;
            distanciaRecorrida = 0;
            //state = ForkLiftState.eUnLoadingRetirePosition;
            if (palletloadpos != null)
            {
                palletloadpos.transform.SetParent(waypoints[indexWaypointActual].waypoint);                
                palletloadpos.GetComponent<Rigidbody>().isKinematic = true;
                palletloadpos.SetActive(true);
            }
            box.SetActive(false);
            state = ForkLiftState.eMove;
            indexWaypointActual++;
            // Verifica si hay más waypoints, si no, reinicia el recorrido
            if (indexWaypointActual >= waypoints.Length)
            {
                indexWaypointActual = 0; // Reinicia al primer waypoint
                                         // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
            }

        }
    }


    private void UnLoadingRetirePosition()
    {
        // Carretilla avanza para cargar la paleta en la estantería

        frontL.motorTorque = currentGear * velocidad;
        frontR.motorTorque = currentGear * velocidad;
        rearL.motorTorque = currentGear * velocidad;
        rearR.motorTorque = currentGear * velocidad;

        // Calcula la distancia recorrida
        distanciaRecorrida += velocidad * Time.deltaTime;

        // Si la distancia recorrida es mayor o igual a la distancia máxima, detén el objeto
        if (distanciaRecorrida >= 50)
        {
            StopForkList();
            UpdateWheelPoses();
            StartCoroutine(DevolverCaja());
            CheckNextOrden();        
        }
        else
        {
            UpdateWheelPoses();
        }
    }
    
    private void CheckNextOrden()
    {
        if (!waypoints[indexWaypointActual].isfinish)
        {
            frontL.brakeTorque = 0;
            frontR.brakeTorque = 0;
            rearL.brakeTorque = 0;
            rearR.brakeTorque = 0;
            state = ForkLiftState.eMove;
            indexWaypointActual++;
            // Verifica si hay más waypoints, si no, reinicia el recorrido
            if (indexWaypointActual >= waypoints.Length)
            {
                indexWaypointActual = 0; // Reinicia al primer waypoint
            }
        }
        else
        {
            state = ForkLiftState.eNothing;
        }

    }

    private void StopForkList()
    {
        frontL.motorTorque = 0;
        frontR.motorTorque = 0;
        rearL.motorTorque = 0;
        rearR.motorTorque = 0;
        frontL.brakeTorque = brakeTorque;
        frontR.brakeTorque = brakeTorque;
        rearL.brakeTorque = brakeTorque;
        rearR.brakeTorque = brakeTorque;      
    }

    private void UpdatePNJ()
    {
        switch (state)
        {
            case ForkLiftState.eMove:
                {
                    // La carretilla se mueve a la siguiente posición
                    distanciaRecorrida = 0;
                    MovePNJ(); 
                    break;
                }
            case ForkLiftState.eLoadingPrepareElevator:
                {
                    // Elevamos las palas a la posición de la paleta a cargar
                    distanciaRecorrida = 0;
                    LoadingPrepareElevator();
                    break;
                }
            case ForkLiftState.eLoadingPallet:
                {                    
                    // Avanzamos hacia la paleta a cargar
                    currentGear = 1;
                    LoadingPallet();
                    break;
                }
            case ForkLiftState.eLoadingElevatorPallet:
                {
                    // Con la paleta cargada, la elevamos para poder retirarla de la estanteria/suelo
                    distanciaRecorrida = 0;
                    LoadingElevatorPallet();
                    break;
                }
            case ForkLiftState.eLoadingRetirePosition:
                {
                    // Retiramos hacía atrás para poder sacar la paleta de su posición
                    currentGear = -1;
                    LoadingPallet();
                    break;
                }
            case ForkLiftState.eLoadingSavePosition:
                {
                    // Bajamos/subimos la palas a posición segura para poder movernos con la paleta encima
                    LoadingSavePosition();                    
                    break;
                }
            case ForkLiftState.eUnLoadingPrepareElevator:
                {
                    // Bajamos las palas a la posición de descarga
                    UnLoadingPrepareElevator();
                    break;
                }
            case ForkLiftState.eUnLoadingRetirePosition:
                {
                    // Nos retiramos hacia atrás para dejar la paleta
                    currentGear = -1;                 
                    UnLoadingRetirePosition();
                    break;
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

    IEnumerator DevolverCaja()
    {
        yield return new WaitForSeconds(2f); // Ajusta el tiempo según sea necesario

        // Devuelve la caja a su posición y orientación originales
        palletloadpos.transform.SetParent(null);
        palletloadpos.GetComponent<Rigidbody>().isKinematic = false;
        palletloadpos.transform.position = positionorigpallet;
        palletloadpos.transform.rotation = rotacionorigpallet;
        palletloadpos = null;
    }
}
