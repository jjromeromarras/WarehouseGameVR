using System;
using System.Collections;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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

    public void SetStock(string stock, int cantidad, string ssc, bool reception, bool isfake)
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
   

        switch (stock.ToLower())
        {
            case "piña":
                if (!reception || (reception && !isfake))
                {
                    SetStock(piña, cantidad, reception);
                }
                else
                {
                    SetStock(platano, cantidad-Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "melocoton":
                if (!reception || (reception && !isfake))
                {
                    SetStock(melocoton, cantidad, reception);
                }
                else
                {
                    SetStock(uvas, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "platano":
                if (!reception || (reception && !isfake))
                {
                    SetStock(platano, cantidad, reception);
                }
                else
                {
                    SetStock(melocoton, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "peras":
                if (!reception || (reception && !isfake))
                {
                    SetStock(peras, cantidad, reception);
                }
                else
                {
                    SetStock(manzanas, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "manzanas":
                if (!reception || (reception && !isfake))
                {
                    SetStock(manzanas, cantidad, reception);
                }
                else
                {
                    SetStock(fresa, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "uvas":
                if (!reception || (reception && !isfake))
                {
                    SetStock(uvas, cantidad, reception);
                }
                else
                {
                    SetStock(peras, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "fresa":
                if (!reception || (reception && !isfake))
                {
                    SetStock(fresa, cantidad, reception);
                }
                else
                {
                    SetStock(melocoton, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
        }
        
    }

    private void SetStock(GameObject stocktype, int cantidad, bool reception)
    {
        if(!reception)
            StartCoroutine(ActivarDespuesDeEsperaPicking(stocktype));    
        else
            StartCoroutine(ActivarDespuesDeEsperaReception(stocktype, cantidad));
    }

    IEnumerator ActivarDespuesDeEsperaPicking(GameObject stocktype)
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

    IEnumerator ActivarDespuesDeEsperaReception(GameObject stocktype, int cantidad)
    {

        yield return new WaitForSeconds(0.5f);
        // Activar el objeto después de la espera
        stocktype.SetActive(true);
        for (int i = 0; i < stocktype.transform.childCount; i++)
        {
            if(i<cantidad)
                stocktype.transform.GetChild(i).gameObject.SetActive(true);
            else
                stocktype.transform.GetChild(i).gameObject.SetActive(false);
            var pallet = stocktype.transform.GetChild(i).GetComponent<selectedBox>();
            if (pallet != null)
            {
                pallet.isSelected = false;
            }
        }

        yield return new WaitForSeconds(0.5f);
    }


}
