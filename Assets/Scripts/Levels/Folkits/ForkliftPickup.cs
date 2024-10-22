using System;
using UnityEngine;

public class ForkliftPickup : MonoBehaviour
{
    [SerializeField] public Transform loader;
    [SerializeField] public Transform pulmonRecepciones;

    private bool isCarryingPallet = false;  // Verifica si ya tienes una paleta recogida
    private GameObject palletobj = null;
    private bool isUnloadLoader = false;
    private bool isOnPulmon = false;
    public event Action<string> onUnloadPallet;

    // Cuando las palas de la carretilla entran en contacto con la paleta
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pallet") && !isCarryingPallet)
        {
            // Detectamos que hemos recogido la paleta
            palletobj = other.gameObject;
            isCarryingPallet = true;

            // Hacer que la paleta se "pegue" a las palas
            //palletobj.transform.SetParent(transform);
            //palletobj.GetComponent<Rigidbody>().isKinematic = true; // Detener la física de la paleta
            palletobj.GetComponent<pallet>().SetSelected(false);
            Debug.Log("Paleta recogida");
        }

        if (isCarryingPallet && other.CompareTag("pulmonrecepciones"))
        {
            isOnPulmon = true;
            // Comprobar si las palas están bajadas
            
        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("pulmonrecepciones"))
        {
            isOnPulmon = false;
        }
    }
    private bool AreForksLowered()
    {
        // Aquí puedes incluir la lógica para verificar si las palas están bajadas.
        // Por ejemplo, comprueba la altura de las palas en relación a la posición de la carretilla.
        return loader.transform.position.y <= 0; // Asegúrate de ajustar esto a tu modelo
    }
    // Si necesitas liberar la paleta, por ejemplo al bajarla en el punto de entrega
    public void ReleasePallet()
    {
        if (isCarryingPallet && palletobj != null)
        {
            //palletobj.transform.SetParent(null);
            //palletobj.GetComponent<Rigidbody>().isKinematic = false;  // Restaurar la física
            isCarryingPallet = false;
            onUnloadPallet(palletobj.GetComponent<pallet>().ssc);
            palletobj.SetActive(false);
            palletobj = null;
            isOnPulmon=false;
            isUnloadLoader=false;
            Debug.Log("Paleta liberada");
        }
    }

    private void Update()
    {
        if (isCarryingPallet)
        {
            // Verificamos si estamos dentro del pulmon
            if (isOnPulmon && !isUnloadLoader)
            {
                // Estamos dentro del pulmon y no hemos descargado
                if (AreForksLowered())
                {
                    isUnloadLoader = true;
                    ReleasePallet();
                    Debug.Log("Preparándose para depositar la paleta en el pulmón de recepciones");
                }
                else
                {
                    // Hemos descargo la paleta, Tenemos que salir
                    isUnloadLoader = false;
                }
            }
            //// Verifica si la paleta ha sido colocada correctamente y si el jugador se aleja
            //float distanceToPulmon = Vector3.Distance(transform.position, pulmonRecepciones.position);

            //if (distanceToPulmon > 1.0f) // Ajusta la distancia mínima según tus necesidades
            //{

            //    if (isOnPulmon && isUnloadLoader)
            //    {
            //        ReleasePallet(); // Lógica para liberar la paleta si se aleja
            //    }
            //}
        }
    }
}

