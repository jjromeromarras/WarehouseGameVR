using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PalletMovement : MonoBehaviour
{
    // Lista de puntos de referencia (puedes asignarlos en el inspector)
    public Transform[] waypoints;

    // Velocidad de movimiento de la paleta
    public float speed = 5f;

    // Índice para el punto actual
    private int currentWaypointIndex = 0;

    // Tolerancia para considerar que ha llegado al punto
    public float tolerance = 0.1f;

    void Update()
    {
        // Si hay waypoints definidos
        if (waypoints.Length > 0)
        {
            MoveTowardsWaypoint();
        }
    }

    void MoveTowardsWaypoint()
    {
        // Obtén el waypoint actual
        Transform targetWaypoint = waypoints[currentWaypointIndex];

        // Calcula la dirección de movimiento (solo en ejes X y Z)
        Vector3 targetPosition = new Vector3(targetWaypoint.position.x, transform.position.y, targetWaypoint.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Mueve la paleta hacia el punto de referencia
        transform.position += direction * speed * Time.deltaTime;

        // Si la paleta está cerca del punto de referencia
        if (Vector3.Distance(transform.position, targetPosition) < tolerance)
        {
            // Cambia al siguiente punto de referencia
            currentWaypointIndex++;

            // Si llega al último punto, puede regresar o detenerse
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0; // Para hacer que vuelva a empezar
            }
        }
    }
}

