using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageTruck : MonoBehaviour
{

    #region Fields
    [SerializeField] public GameObject selectedCube;
    #endregion
    // Start is called before the first frame update
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("carretilla"))
        {
            // Detectamos que hemos recogido la paleta
            SetSelected(false);
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
