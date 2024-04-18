using System;
using TMPro;
using UnityEngine;

public class pickingcamera : MonoBehaviour
{
    
    // Velocidad de movimiento de la cruceta
    [SerializeField] public float velocidad = 3f;

    // Distancia máxima de selección
    [SerializeField] public float distanciaMaxima = 5f;
    [SerializeField] public TextMeshProUGUI txtplatano;
    [SerializeField] public TextMeshProUGUI txtuvas;
    [SerializeField] public TextMeshProUGUI txtpiñas;
    [SerializeField] public TextMeshProUGUI txtperas;
    [SerializeField] public TextMeshProUGUI txtmelocoton;
    [SerializeField] public TextMeshProUGUI txtmanzanas;
    [SerializeField] public TextMeshProUGUI txtfresas;

    public event Action onCancelPickingLocation;
    public event Action<int, int, int, int, int, int, int> onCheckPicking;
    public event Action onResetPicking;
    
    private boxType boxType;
    private bool isBoxSelected;
    // Caja actualmente seleccionada
    private Transform cajaSeleccionada;
    private Camera camara;
    private int cantidadplatano;
    private int cantidaduvas;
    private int cantidadpiñas;
    private int cantidadperas;
    private int cantidadmelocoton;
    private int cantidadmanzanas;
    private int cantidadfresas;
    // Update is called once per frame
    private void Start()
    {
        ResetScene();
        camara = Camera.main; // Obtener la cámara principal

    }

    void Update()
    {

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Calcular el desplazamiento de la mano
        Vector3 desplazamiento = new Vector3(horizontal, vertical, 0) * velocidad * Time.deltaTime;

        // Convertir la posición del objeto a coordenadas de vista
        Vector3 viewportPos = camara.WorldToViewportPoint(transform.position + desplazamiento);

        if (viewportPos.x >= 0 && viewportPos.x <= 1 && viewportPos.y >= 0 && viewportPos.y <= 1)
        {
            // Aplicar el desplazamiento a la posición de la mano
            transform.position += desplazamiento;
            Console.WriteLine(transform.position);
        }
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
                case "botonconfirmar":
                    {
                        boxType = boxType.bottonok;
                        isBoxSelected = true;
                        break;
                    }
                case "bontoncancelar":
                    {
                        boxType = boxType.bottoncancel;
                        isBoxSelected = true;
                        break;
                    }
                case "botonresetear":
                    {
                        boxType = boxType.bottonreset;
                        isBoxSelected = true;
                        break;
                    }

            }
            if (isBoxSelected)
            {
                // Obtener la caja seleccionada

                Transform nuevaCajaSeleccionada = hit.transform;
                                
                if(nuevaCajaSeleccionada!=cajaSeleccionada && cajaSeleccionada != null)
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

        // Teclado
        if (Input.GetKeyDown(KeyCode.Space))
        {

            if(boxType == boxType.bottoncancel)
            {
                this.onCancelPickingLocation();
            } else if (boxType == boxType.bottonok)
            {
                this.onCheckPicking(cantidadplatano, cantidaduvas, cantidadpiñas, cantidadperas , cantidadmelocoton, cantidadmanzanas, cantidadfresas);
            }
            else if (boxType == boxType.bottonreset)
            {
                this.onResetPicking();
            }
            else
            {
                if(cajaSeleccionada != null) {
                    cajaSeleccionada.gameObject.SetActive(false);
                    cajaSeleccionada = null;
                    switch (boxType)
                    {
                        case boxType.piña:
                            {
                                cantidadpiñas += 1;
                                txtpiñas.text = cantidadpiñas.ToString();
                                break;
                            }
                        case boxType.melocoton:
                            {
                                cantidadmelocoton += 1;
                                txtmelocoton.text = cantidadmelocoton.ToString();
                                break;
                            }
                        case boxType.platano:
                            {
                                cantidadplatano += 1;
                                txtplatano.text = cantidadplatano.ToString();
                                break;
                            }
                        case boxType.fresa:
                            {
                                cantidadfresas += 1;
                                txtfresas.text = cantidadfresas.ToString();
                                break;
                            }
                        case boxType.peras:
                            {
                                cantidadperas += 1;
                                txtperas.text = cantidadperas.ToString();
                                break;
                            }
                        case boxType.manzanas:
                            {
                                cantidadmanzanas += 1;
                                txtmanzanas.text = cantidadmanzanas.ToString();
                                break;
                            }
                        case boxType.uvas:
                            {
                                cantidaduvas += 1;
                                txtuvas.text = cantidaduvas.ToString();
                                break;
                            }
                    }
                }
            }
        }
    }

    public void ResetScene()
    {
        cantidadfresas = 0;
        cantidadmelocoton = 0;
        cantidadmanzanas = 0;
        cantidadperas = 0;
        cantidadpiñas = 0;
        cantidadplatano = 0;
        cantidaduvas = 0;

        txtfresas.text = "0";
        txtmelocoton.text = "0";
        txtmanzanas.text = "0";
        txtperas.text = "0";
        txtpiñas.text = "0";
        txtplatano.text = "0";
        txtuvas.text = "0";

        cajaSeleccionada = null;
    }
}

enum boxType
{
    piña,
    melocoton,
    platano,
    fresa,
    peras,
    manzanas,
    uvas,
    bottonok,
    bottoncancel,
    bottonreset
}
