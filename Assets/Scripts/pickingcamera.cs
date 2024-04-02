using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickingcamera : MonoBehaviour
{
    
    // Velocidad de movimiento de la cruceta
    public float velocidad = 3f;

    // Distancia máxima de selección
    public float distanciaMaxima = 5f;

    private boxType boxType;
    private bool isBoxSelected;
    // Caja actualmente seleccionada
    private Transform cajaSeleccionada;
    private Camera camara;
    // Update is called once per frame
    private void Start()
    {
        cajaSeleccionada = null;
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
