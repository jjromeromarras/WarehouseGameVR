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
                    SetStock(piña, peras, cantidad, reception);
                }
                else
                {
                    SetStock(platano, null, cantidad-Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "melocoton":
                if (!reception || (reception && !isfake))
                {
                    SetStock(melocoton, manzanas, cantidad, reception);
                }
                else
                {
                    SetStock(uvas, null, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "platano":
                if (!reception || (reception && !isfake))
                {
                    SetStock(platano, uvas, cantidad, reception);
                }
                else
                {
                    SetStock(melocoton, null, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "peras":
                if (!reception || (reception && !isfake))
                {
                    SetStock(peras, platano, cantidad, reception);
                }
                else
                {
                    SetStock(manzanas, null, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "manzanas":
                if (!reception || (reception && !isfake))
                {
                    SetStock(manzanas, melocoton, cantidad, reception);
                }
                else
                {
                    SetStock(fresa, null,cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "uvas":
                if (!reception || (reception && !isfake))
                {
                    SetStock(uvas, fresa, cantidad, reception);
                }
                else
                {
                    SetStock(peras, null, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
            case "fresa":
                if (!reception || (reception && !isfake))
                {
                    SetStock(fresa, platano, cantidad, reception);
                }
                else
                {
                    SetStock(melocoton, null, cantidad - Random.Range(1, cantidad - 1), reception);
                }
                break;
        }
        
    }

    private void SetStock(GameObject stocktype, GameObject stockmult, int cantidad, bool reception)
    {
        if(!reception)
            StartCoroutine(ActivarDespuesDeEsperaPicking(stocktype, stockmult, cantidad));    
        else
            StartCoroutine(ActivarDespuesDeEsperaReception(stocktype, cantidad));
    }


    IEnumerator ActivarDespuesDeEsperaPicking(GameObject stocktype, GameObject stockmulti, int cantidad)
    {
     
        yield return new WaitForSeconds(0.5f);
        // Activar el objeto después de la espera
        stocktype.SetActive(true);
        stockmulti.SetActive(true);
        var total = cantidad + 3 < 9 ? cantidad + 3 : cantidad;

        // Parte de atrás        
        for (int i = 11; i >= 0; i--)
        {
            if (total>0)
            {
                stocktype.transform.GetChild(i).gameObject.SetActive(true);
                stockmulti.transform.GetChild(i).gameObject.SetActive(false);
                var pallet = stocktype.transform.GetChild(i).GetComponent<selectedBox>();
                if (pallet != null)
                {
                    pallet.isSelected = false;
                }
                total--;
            }
            else
            {
                stocktype.transform.GetChild(i).gameObject.SetActive(false);
                stockmulti.transform.GetChild(i).gameObject.SetActive(true);
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
