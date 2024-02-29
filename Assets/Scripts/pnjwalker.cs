using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class pnjwalker : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("PNJ")]
    public orderwalker[] orderwalkers;
    public Animator anim;

    private int indexWaypointActual = 0;
    private int margenError = 2;
    private bool pnjdetenido = false;
    private NavMeshAgent agente;
    private PNJRFState state;
    void Start()
    {
          anim.SetBool("IsWalk", false);
          anim.SetBool("IsPicking", false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case PNJRFState.eMove:
                {
                    movePNJ();
                    break;
                }
            case PNJRFState.eNothing: {
                    anim.SetBool("IsWalk", false);
                    anim.SetBool("IsPicking", false);                                        
                    break;
                }
            case PNJRFState.eDoPicking:
                {
                    DoPicking();
                    break;
                }
            case PNJRFState.eRotateToPallet:
                {

                    break;
                }
        }
       
    }
    private void Awake()
    {
        state = PNJRFState.eMove;
        agente = GetComponent<NavMeshAgent>();
    }

    private void OnTriggerEnter(Collider other)
    {
            if (other.CompareTag("Player") || other.CompareTag("PNJ"))
            {
                pnjdetenido = true;
                anim.SetBool("IsWalk", false);
            }
    }

    private void OnTriggerExit(Collider other)
    {
        pnjdetenido = false;

    }

    private void movePNJ()
    {
        if (orderwalkers.Length > 0 && indexWaypointActual < orderwalkers.Length && !pnjdetenido)
        {

            // Calcula la distancia entre el camión y el waypoint actual
            float distancia = Vector3.Distance(transform.position, orderwalkers[indexWaypointActual].waypoint.position);

            if (distancia > margenError)
            {

                anim.SetBool("IsWalk", true);
                anim.SetBool("IsPicking", false);
                anim.Play("walking", 0);
                agente.enabled = true;
                agente.destination = orderwalkers[indexWaypointActual].waypoint.position;
            }
            else
            {
                anim.SetBool("IsWalk", false);
                agente.enabled = false;
                if (orderwalkers[indexWaypointActual].ispicking)
                {
                    var shelf = orderwalkers[indexWaypointActual].waypoint.parent;
                    if (shelf != null)
                    {
                        var positionorigpallet = shelf.transform.position;

                        // Obtenemos la dirección hacia el waypoint actual
                        Vector3 direccion = positionorigpallet - transform.position;

                        // Rotamos la carretilla hacía el punto
                        Quaternion rotacion = Quaternion.LookRotation(direccion);

                        StartCoroutine(RotarHaciaCaja(rotacion));
                        state = PNJRFState.eRotateToPallet;
                    } else
                        state = PNJRFState.eDoPicking;
                } 
                else
                {
                    if (!orderwalkers[indexWaypointActual].isfinish)
                    {
                        state = PNJRFState.eMove;
                        indexWaypointActual++;
                        // Verifica si hay más waypoints, si no, reinicia el recorrido
                        if (indexWaypointActual >= orderwalkers.Length)
                        {
                            indexWaypointActual = 0; // Reinicia al primer waypoint
                                                     // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
                        }
                    }
                    else
                    {
                        state = PNJRFState.eNothing;
                    }
                }
            }           
         }
    }

    public void DoPicking()
    {
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsPicking", true);
        state = PNJRFState.eWaiting;
        StartCoroutine(WaitPicking(10));
    }
    public IEnumerator WaitPicking(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsPicking", false);
        if (!orderwalkers[indexWaypointActual].isfinish)
        {
            state = PNJRFState.eMove;
            indexWaypointActual++;
            // Verifica si hay más waypoints, si no, reinicia el recorrido
            if (indexWaypointActual >= orderwalkers.Length)
            {
                indexWaypointActual = 0; // Reinicia al primer waypoint
                                         // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
            }
        }
        else
        {
            state = PNJRFState.eNothing;
        }
    }

    IEnumerator RotarHaciaCaja(Quaternion rotacionObjetivo)
    {
        float duracionRotacion = 40f; // Ajusta según sea necesario
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
        state = PNJRFState.eDoPicking;
    }
}
