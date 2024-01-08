using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pnjwalker : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("PNJ")]
    public Transform[] waypoints;
    public Animator anim;
    public Rigidbody rb;

    
    [Header("VALUES")]
    public float currentSpeed;

    private int indexWaypointActual = 0;
    private int margenError = 1;
    private bool pnjdetenido = false;
    void Start()
    {
          anim.SetBool("IsWalk", false);
    }

    // Update is called once per frame
    void Update()
    {
         //currentSpeed = rb.velocity.magnitude;
        movePNJ();
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
        if (waypoints.Length > 0 && indexWaypointActual < waypoints.Length && !pnjdetenido)
        {
            anim.SetBool("IsWalk", true);
            
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
               transform.position = Vector3.MoveTowards(transform.position, waypoints[indexWaypointActual].position, Time.deltaTime* 1.5f);                
                
            }
            else
            {
                indexWaypointActual++;
                // Verifica si hay más waypoints, si no, reinicia el recorrido
                if (indexWaypointActual >= waypoints.Length)
                {

                    anim.SetBool("IsWalk", false);
                    // indexWaypointActual = 0; // Reinicia al primer waypoint
                    // Puedes agregar aquí lógica adicional si deseas que el camión se detenga o realice alguna acción específica al completar la ruta
                }
            }
        }
    }
}
