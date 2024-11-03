using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DockFolkit : MonoBehaviour
{
    [SerializeField] private GameObject selectObject;
    [SerializeField] private GameObject dock;
    // Start is called before the first frame update
   

    public void SetSelected(bool value)
    {
        selectObject.SetActive(value);
    }
}
