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
    private ForkLiftState state;
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
            case ForkLiftState.eMove:
                {
                    movePNJ();
                    break;
                }
            case ForkLiftState.eNothing: {
                    anim.SetBool("IsWalk", false);
                    anim.SetBool("IsPicking", false);                                        
                    break;
                }
            case ForkLiftState.eLoadingPallet:
                {
                    DoPicking();
                    break;
                }
        }
       
    }
    private void Awake()
    {
        state = ForkLiftState.eMove;
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
                    state = ForkLiftState.eLoadingPallet;
                } 
                else
                {
                    if (!orderwalkers[indexWaypointActual].isfinish)
                    {
                        state = ForkLiftState.eMove;
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
                        state = ForkLiftState.eNothing;
                    }
                }
            }
            /*
            if (!orderwalkers[indexWaypointActual].ispicking)
            {
                anim.SetBool("IsWalk", true);
                anim.SetBool("IsPicking", false);
                anim.Play("walking", 0);

                // Calcula la distancia entre el camión y el waypoint actual
                float distancia = Vector3.Distance(transform.position, orderwalkers[indexWaypointActual].waypoint.position);

                if (distancia > margenError)
                {
                    // Obtenemos la dirección hacia el waypoint actual
                    Vector3 direccion = orderwalkers[indexWaypointActual].waypoint.position - transform.position;

                    // Rotamos la carretilla hacía el punto
                    Quaternion rotacion = Quaternion.LookRotation(direccion);
                    transform.rotation = Quaternion.Lerp(transform.rotation, rotacion, Time.deltaTime * 3f);

                    // Mueve el camión hacia el waypoint actual
                    transform.position = Vector3.MoveTowards(transform.position, orderwalkers[indexWaypointActual].waypoint.position, Time.deltaTime * 1.5f);

                }
                else
                {
                    if(orderwalkers[indexWaypointActual].isfinish)
                    {
                        indexWaypointActual = orderwalkers.Length + 1;
                    }
                    else 
                        indexWaypointActual++;
                    // Verifica si hay más waypoints, si no, reinicia el recorrido
                    if (indexWaypointActual >= orderwalkers.Length && !orderwalkers[indexWaypointActual].isfinish)
                    {
                        indexWaypointActual = 0; // Reinicia al primer waypoint
                                                 // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
                    }
                    else
                    {
                        anim.SetBool("IsWalk", false);
                        anim.SetBool("IsPicking", false);
                    }
                }
            }
            else // picking
            {
                anim.SetBool("IsWalk", false);
                anim.SetBool("IsPicking", true);
                StartCoroutine(WaitPicking(10));
                
                if (!orderwalkers[indexWaypointActual].isfinish)
                {
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
                    anim.SetBool("IsWalk", true);
                    anim.SetBool("IsPicking", false);
                }
            }*/
         }
    }

    public void DoPicking()
    {
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsPicking", true);
        state = ForkLiftState.eLoadingSavePosition;
        StartCoroutine(WaitPicking(10));
    }
    public IEnumerator WaitPicking(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        anim.SetBool("IsWalk", false);
        anim.SetBool("IsPicking", false);
        if (!orderwalkers[indexWaypointActual].isfinish)
        {
            state = ForkLiftState.eMove;
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
            state = ForkLiftState.eNothing;
        }
    }
}
