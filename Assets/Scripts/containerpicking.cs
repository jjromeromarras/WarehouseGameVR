using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
    public TextMeshPro SSC1;

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
   
    public void SetSSCC(string sscc)
    {
        SSC1.text = sscc;
    }

    public void SetStock(string stock, int cantidad, string ssc)
    {
        SSC1.text = ssc;
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
            case "fresa":
                SetStock(fresa, cantidad);
                break;
        }
    }

    private void SetStock(GameObject stocktype, int cantidad)
    {
        StartCoroutine(ActivarDespuesDeEspera(stocktype));      
    }

    IEnumerator ActivarDespuesDeEspera(GameObject stocktype)
    {
     
        yield return new WaitForSeconds(0.5f);
        // Activar el objeto después de la espera
        stocktype.SetActive(true);        
        for (int i = 0; i < stocktype.transform.childCount; i++)
        {
            stocktype.transform.GetChild(i).gameObject.SetActive(true);
            var pallet = stocktype.transform.GetChild(i).GetComponent<selectedBox>();
            if( pallet != null )
            {
                pallet.isSelected = false;
            }
        }

        yield return new WaitForSeconds(0.5f);
    }


}
