using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class containerpicking : MonoBehaviour
{
    public GameObject piña;
    public GameObject melocoton;
    public GameObject platano;
    public GameObject fresa;
    public GameObject peras;
    public GameObject manzanas;
    public GameObject uvas;

    public string stock;
    public int cantidad;

    // Start is called before the first frame update
    void Start()
    {
        piña.SetActive(false);
        melocoton.SetActive(false);
        platano.SetActive(false);
        fresa.SetActive(false);
        peras.SetActive(false);
        manzanas.SetActive(false);
        uvas.SetActive(false);
        stock = string.Empty;
    }
   

    public void SetStock(string stock, int cantidad)
    {
        this.stock = stock;
        piña.SetActive(false);
        melocoton.SetActive(false);
        platano.SetActive(false);
        fresa.SetActive(false);
        peras.SetActive(false);
        manzanas.SetActive(false);
        uvas.SetActive(false);        

        switch(stock.ToLower())
        {
            case "piña":
                SetStock(piña, cantidad);
                break;
            case "melocoton":
                SetStock(melocoton, cantidad);                
                break;
            case "platano":
                SetStock(platano, cantidad);
                break;
            case "peras":
                SetStock(peras, cantidad);
                break;
            case "manzanas":
                SetStock(manzanas, cantidad);
                break;
            case "uvas":
                SetStock(uvas, cantidad);
                break;
        }
    }

    private void SetStock(GameObject stocktype, int cantidad)
    {
        StartCoroutine(ActivarDespuesDeEspera(stocktype));
       // stocktype.SetActive(true);
        //var products = stocktype.GetComponentsInChildren<selectedBox>();

        /*for (int i = 0; i < stocktype.transform.childCount; i++)
        {
            if (i < cantidad)
            {
                stocktype.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                stocktype.transform.GetChild(i).gameObject.SetActive(false);
            }
        }*/

    }

    IEnumerator ActivarDespuesDeEspera(GameObject stocktype)
    {
        yield return new WaitForSeconds(0.25f);

        // Activar el objeto después de la espera
        stocktype.SetActive(true);
        }


}
