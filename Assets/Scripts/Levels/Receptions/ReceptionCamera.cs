using System;
using TMPro;
using UnityEngine;
using Time = UnityEngine.Time;

public class ReceptionCamera : MonoBehaviour
{
    // Velocidad de movimiento de la cruceta
    [SerializeField] public float velocidad = 3f;
    [SerializeField] public float distanciaMaxima = 5f;
    [SerializeField] public TextMeshProUGUI txtplatano;
    [SerializeField] public TextMeshProUGUI txtuvas;
    [SerializeField] public TextMeshProUGUI txtpiñas;
    [SerializeField] public TextMeshProUGUI txtperas;
    [SerializeField] public TextMeshProUGUI txtmelocoton;
    [SerializeField] public TextMeshProUGUI txtmanzanas;
    [SerializeField] public TextMeshProUGUI txtfresas;
    [SerializeField] public AudioClip clipPicking;

    public event Action<int, int, int, int, int, int, int> onCheckReception;
    public event Action<bool> onCheckItem;
   
    private bool enableActions;
    private StateGame state;
    private boxType boxType;
    private bool isBoxSelected;
    // Caja actualmente seleccionada
    private Transform cajaSeleccionada;
    private Camera camara;
    private float time;
    private float resetTime = 0.2f;
    private pickingclient datareception = new pickingclient();
    // Start is called before the first frame update
    void Start()
    {
        camara = Camera.main; // Obtener la cámara principal
        time = resetTime;
        ResetScene();
        state = StateGame.EnterItem;
        enableActions = false;
    }

    // Update is called once per frame
    void Update()
    {
        
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            // Calcular el desplazamiento de la mano
            Vector3 desplazamiento = new Vector3(horizontal, vertical, 0) * velocidad * UnityEngine.Time.deltaTime;

            // Convertir la posición del objeto a coordenadas de vista
            Vector3 viewportPos = camara.WorldToViewportPoint(transform.position + desplazamiento);

        if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
        {
            // Aplicar el desplazamiento a la posición de la mano
            transform.position += desplazamiento;
            Console.WriteLine(transform.position);
        }
        if (state == StateGame.EnterQuantity)
        {
            // Lanzar un rayo desde la posición de la mano hacia adelante
            Ray rayo = new Ray(transform.position, transform.forward);
            RaycastHit hit;

            isBoxSelected = false;
            if (Physics.Raycast(rayo, out hit))
            {
                switch (hit.collider.tag)
                {
                    case "piña":
                        {
                            boxType = boxType.piña;
                            isBoxSelected = true;
                            break;
                        }
                    case "melocoton":
                        {
                            boxType = boxType.melocoton;
                            isBoxSelected = true;
                            break;
                        }
                    case "platano":
                        {
                            boxType = boxType.platano;
                            isBoxSelected = true;
                            break;
                        }
                    case "fresas":
                        {
                            boxType = boxType.fresa;
                            isBoxSelected = true;
                            break;
                        }
                    case "peras":
                        {
                            boxType = boxType.peras;
                            isBoxSelected = true;
                            break;
                        }
                    case "manzanas":
                        {
                            boxType = boxType.manzanas;
                            isBoxSelected = true;
                            break;
                        }
                    case "uvas":
                        {
                            boxType = boxType.uvas;
                            isBoxSelected = true;
                            break;
                        }

                }
                if (isBoxSelected)
                {
                    // Obtener la caja seleccionada

                    Transform nuevaCajaSeleccionada = hit.transform;

                    if (nuevaCajaSeleccionada != cajaSeleccionada && cajaSeleccionada != null)
                    {
                        // Realizar cualquier acción necesaria para deseleccionar la caja anterior
                        var _selectedbox = cajaSeleccionada.gameObject.GetComponent<selectedBox>();
                        if (_selectedbox != null)
                        {
                            _selectedbox.isSelected = false;
                        }
                    }
                    // Guardar la nueva caja seleccionada
                    cajaSeleccionada = nuevaCajaSeleccionada;
                    if (cajaSeleccionada != null)
                    {
                        // Realizar cualquier acción necesaria para deseleccionar la caja anterior
                        var _selectedbox = cajaSeleccionada.gameObject.GetComponent<selectedBox>();
                        if (_selectedbox != null)
                        {
                            _selectedbox.isSelected = true;
                        }
                    }
                }
                else
                {
                    // Si no se detecta ninguna caja, deseleccionar la caja actual si hay alguna
                    if (cajaSeleccionada != null)
                    {
                        // Realizar cualquier acción necesaria para deseleccionar la caja actual
                        var _selectedbox = cajaSeleccionada.gameObject.GetComponent<selectedBox>();
                        if (_selectedbox != null)
                        {
                            _selectedbox.isSelected = false;
                        }
                        cajaSeleccionada = null;
                    }
                }
            }
            else
            {
                // Si no se detecta ninguna caja, deseleccionar la caja actual si hay alguna
                if (cajaSeleccionada != null)
                {
                    // Realizar cualquier acción necesaria para deseleccionar la caja actual
                    var _selectedbox = cajaSeleccionada.gameObject.GetComponent<selectedBox>();
                    if (_selectedbox != null)
                    {
                        _selectedbox.isSelected = false;
                    }
                    cajaSeleccionada = null;
                }
            }


            if (enableActions)
            {
                // Tecla confirmar
                if (Input.GetKeyDown(KeyCode.C) || Input.GetKey(KeyCode.Joystick1Button2))
                {
                    
                    var totalcantidad = datareception.cantidadplatano + datareception.cantidaduvas + datareception.cantidadpiñas+
                        datareception.cantidadperas+ datareception.cantidadmelocoton+ datareception.cantidadmanzanas+
                        datareception.cantidadfresas;
                    if (totalcantidad > 0)
                    {
                        enableActions = false;
                        this.onCheckReception(datareception.cantidadplatano, datareception.cantidaduvas, datareception.cantidadpiñas,
                        datareception.cantidadperas, datareception.cantidadmelocoton, datareception.cantidadmanzanas,
                        datareception.cantidadfresas);
                    }
                }

               
                if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Joystick1Button0))
                {

                    time -= Time.deltaTime;
                    if (time < 0f)
                    {

                        if (cajaSeleccionada != null)
                        {


                            SoundManager.SharedInstance.PlaySound(clipPicking);
                            cajaSeleccionada.gameObject.SetActive(false);
                            cajaSeleccionada = null;
                            switch (boxType)
                            {
                                case boxType.piña:
                                    {

                                        datareception.cantidadpiñas += 1;
                                        txtpiñas.text = datareception.cantidadpiñas.ToString();
                                        break;
                                    }
                                case boxType.melocoton:
                                    {
                                        datareception.cantidadmelocoton += 1;
                                        txtmelocoton.text = datareception.cantidadmelocoton.ToString();
                                        break;
                                    }
                                case boxType.platano:
                                    {
                                        datareception.cantidadplatano += 1;
                                        txtplatano.text = datareception.cantidadplatano.ToString();
                                        break;
                                    }
                                case boxType.fresa:
                                    {
                                        datareception.cantidadfresas += 1;
                                        txtfresas.text = datareception.cantidadfresas.ToString();
                                        break;
                                    }
                                case boxType.peras:
                                    {
                                        datareception.cantidadperas += 1;
                                        txtperas.text = datareception.cantidadperas.ToString();
                                        break;
                                    }
                                case boxType.manzanas:
                                    {
                                        datareception.cantidadmanzanas += 1;

                                        break;
                                    }
                                case boxType.uvas:
                                    {
                                        datareception.cantidaduvas += 1;

                                        break;
                                    }
                            }

                        }

                        time = resetTime;
                    }

                }
            }
            else
            {
                time = resetTime;
            }
        }
        else
        {
            if (enableActions)
            {
                //estamo comprobando la paleta
                if (Input.GetKeyDown(KeyCode.C) || Input.GetKey(KeyCode.Joystick1Button2))
                {
                    time -= Time.deltaTime;
                    if (time < 0f)
                    {
                        enableActions = false;
                        onCheckItem(true);
                        time = resetTime;
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKey(KeyCode.Joystick1Button3))
                {
                    time -= Time.deltaTime;
                    if (time < 0f)
                    {
                        enableActions = false;
                        onCheckItem(false);
                        time = resetTime;
                    }
                }
            }
            else
            {
                time = resetTime;
            }
            
        }
        ShowReception();
    }

    public void setenableActions(bool enable) { 
        enableActions = enable; 
    }

    public void setStateItem()
    {
        state = StateGame.EnterItem;
    }

    public void setStateQuantity()
    {
        state = StateGame.EnterQuantity;
    }

    private void ShowReception()
    {
        txtpiñas.text = datareception.cantidadpiñas.ToString();
        txtmelocoton.text = datareception.cantidadmelocoton.ToString();
        txtplatano.text = datareception.cantidadplatano.ToString();
        txtfresas.text = datareception.cantidadfresas.ToString();
        txtperas.text = datareception.cantidadperas.ToString();
        txtmanzanas.text = datareception.cantidadmanzanas.ToString();
        txtuvas.text = datareception.cantidaduvas.ToString();
    }

    public void ResetScene()
    {
        datareception = new pickingclient();
        txtfresas.text = "0";
        txtmelocoton.text = "0";
        txtmanzanas.text = "0";
        txtperas.text = "0";
        txtpiñas.text = "0";
        txtplatano.text = "0";
        txtuvas.text = "0";

        cajaSeleccionada = null;
    }

    internal enum StateGame
    {
        EnterItem,
        EnterQuantity,
    }
}


