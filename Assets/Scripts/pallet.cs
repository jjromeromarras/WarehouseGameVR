using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class pallet : MonoBehaviour
{
    // Start is called before the first frame update
     [Header("UI elements")]
    public TextMeshPro SSC1;

    public TextMeshPro SSC2;

    public Transform centerOfMass;
    public GameObject selectedCube;

    [Header("VALUES")]
    public string ssc;


    private bool caninteractue = false;
    private void Start()
    {
        SSC1.text = ssc;
        SSC2.text = ssc;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //it can not enter
    
            caninteractue = false;
        }
    }

     private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //it can enter
     
            caninteractue = true;
        }
    }

    void Update()
    {
        if (caninteractue == true && Input.GetKeyDown(KeyCode.Space))
        {
            centerOfMass.rotation = Quaternion.Euler(0,90,0);
        }
    }

    public void SetSelected(bool selected)
    {
        if (selectedCube != null)
        {
            selectedCube.SetActive(selected);
        }
    }
}
