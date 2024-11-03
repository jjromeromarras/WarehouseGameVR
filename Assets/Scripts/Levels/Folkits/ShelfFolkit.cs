using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShelfFolkit : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject selectObject;
    [SerializeField] public TextMeshPro shelf;
    // Start is called before the first frame update


    public void SetSelected(bool value)
    {
        selectObject.SetActive(value);
    }
}
