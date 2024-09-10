using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


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
    [SerializeField] public TextMeshProUGUI txtclient1;
    [SerializeField] public TextMeshProUGUI txtclient2;
    [SerializeField] public TextMeshProUGUI txtclient3;
    [SerializeField] public Image imgclient1;
    [SerializeField] public Image imgclient2;
    [SerializeField] public Image imgclient3;
    [SerializeField] public AudioClip clipPicking;


    public event Action<int, int, int, int, int, int, int> onCheckPicking;
    public event Action onErrorPicking;
    public event Action onResetPicking;
    public event Action onErrorContainerClient;
    public event Func<string, bool> onCheckContainerPicking;

    public int pedidopreparando;
    public string contclient1, contclient2, contclient3;

    private boxType boxType;
    private bool isBoxSelected;
    // Caja actualmente seleccionada
    private Transform cajaSeleccionada;
    private Camera camara;
    private pickingclient[] pickings = new pickingclient[3];
    private float time;
    private float resetTime = 0.2f;
    public int selectclient = 1;


    // Update is called once per frame
    private void Start()
    {
        camara = Camera.main; // Obtener la cámara principal
        time = resetTime;
        ResetScene();
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

        // Teclado
        if (Input.GetKeyDown(KeyCode.F1))
        {
            selectclient = 1;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            selectclient = 2;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            selectclient = 3;
        }

        // Tecla confirmar
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKey(KeyCode.Joystick1Button2))
        {
            if (CheckClientContainerQuantity())
            {
                this.onErrorPicking();
            }
            else
            {
                this.onCheckPicking(pickings[pedidopreparando - 1].cantidadplatano, pickings[pedidopreparando - 1].cantidaduvas, pickings[pedidopreparando - 1].cantidadpiñas,
                    pickings[pedidopreparando - 1].cantidadperas, pickings[pedidopreparando - 1].cantidadmelocoton, pickings[pedidopreparando - 1].cantidadmanzanas, pickings[pedidopreparando - 1].cantidadfresas);
            }
        }
        else if (Input.GetKeyDown(KeyCode.R) || Input.GetKey(KeyCode.Joystick1Button3))
        {
            this.onResetPicking();
        }

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Joystick1Button0))
        {


            time -= Time.deltaTime;
            if (time < 0f)
            {
                if (selectclient > 0)
                {
                    if (cajaSeleccionada != null)
                    {
                        string sscc = string.Empty;
                        try
                        {
                            sscc = cajaSeleccionada.parent.parent.GetComponentsInChildren<TextMeshPro>()[0].text;
                        }
                        catch
                        {
                            sscc = string.Empty;
                        }

                        if (onCheckContainerPicking.Invoke(sscc))
                        {

                            SoundManager.SharedInstance.PlaySound(clipPicking);
                            cajaSeleccionada.gameObject.SetActive(false);
                            cajaSeleccionada = null;
                            switch (boxType)
                            {
                                case boxType.piña:
                                    {

                                        pickings[selectclient - 1].cantidadpiñas += 1;
                                        txtpiñas.text = pickings[selectclient - 1].cantidadpiñas.ToString();
                                        break;
                                    }
                                case boxType.melocoton:
                                    {
                                        pickings[selectclient - 1].cantidadmelocoton += 1;
                                        txtmelocoton.text = pickings[selectclient - 1].cantidadmelocoton.ToString();
                                        break;
                                    }
                                case boxType.platano:
                                    {
                                        pickings[selectclient - 1].cantidadplatano += 1;
                                        txtplatano.text = pickings[selectclient - 1].cantidadplatano.ToString();
                                        break;
                                    }
                                case boxType.fresa:
                                    {
                                        pickings[selectclient - 1].cantidadfresas += 1;
                                        txtfresas.text = pickings[selectclient - 1].cantidadfresas.ToString();
                                        break;
                                    }
                                case boxType.peras:
                                    {
                                        pickings[selectclient - 1].cantidadperas += 1;
                                        txtperas.text = pickings[selectclient - 1].cantidadperas.ToString();
                                        break;
                                    }
                                case boxType.manzanas:
                                    {
                                        pickings[selectclient - 1].cantidadmanzanas += 1;

                                        break;
                                    }
                                case boxType.uvas:
                                    {
                                        pickings[selectclient - 1].cantidaduvas += 1;

                                        break;
                                    }
                            }
                        }
                    }

                }
                else
                {
                    onErrorContainerClient();
                }

                time = resetTime;
            }

        }
        else
        {
            time = resetTime;
        }
        imgclient1.color = Color.black;
        imgclient2.color = Color.black;
        imgclient3.color = Color.black;
        // Select pedido
        switch (selectclient)
        {
            case 1:
                imgclient1.color = Color.blue; break;
            case 2:
                imgclient2.color = Color.blue; break;
            case 3:
                imgclient3.color = Color.blue; break;
        }
        ShowPickings();
    }
    private bool CheckClientContainerQuantity()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i != selectclient - 1)
            {
                var cant = pickings[i].cantidadpiñas > 0 ||
                            pickings[i].cantidadperas > 0 ||
                            pickings[i].cantidadmelocoton > 0 ||
                            pickings[i].cantidadmanzanas > 0 ||
                            pickings[i].cantidadfresas > 0 ||
                            pickings[i].cantidadplatano > 0 ||
                            pickings[i].cantidaduvas > 0;
                if (cant) return true;
            }
        }
        return false;
    }
    private void ShowPickings()
    {
        if (selectclient > 0)
        {
            txtpiñas.text = pickings[selectclient - 1].cantidadpiñas.ToString();
            txtmelocoton.text = pickings[selectclient - 1].cantidadmelocoton.ToString();
            txtplatano.text = pickings[selectclient - 1].cantidadplatano.ToString();
            txtfresas.text = pickings[selectclient - 1].cantidadfresas.ToString();
            txtperas.text = pickings[selectclient - 1].cantidadperas.ToString();
            txtmanzanas.text = pickings[selectclient - 1].cantidadmanzanas.ToString();
            txtuvas.text = pickings[selectclient - 1].cantidaduvas.ToString();
        }
    }

    public void ResetScene()
    {
        for (int i = 0; i < 3; i++)
        {
            pickings[i] = new pickingclient();
        }

        txtclient1.text = contclient1;
        txtclient2.text = contclient2;
        txtclient3.text = contclient3;

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
    uvas
}

record pickingclient
{
    public int cantidadplatano;
    public int cantidaduvas;
    public int cantidadpiñas;
    public int cantidadperas;
    public int cantidadmelocoton;
    public int cantidadmanzanas;
    public int cantidadfresas;
}
