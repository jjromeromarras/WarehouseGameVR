using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rfmenu;

    private bool showrfmenu;
    void Start()
    {
        showrfmenu = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            showrfmenu = !showrfmenu;
            rfmenu.SetActive(showrfmenu);
        }
    }
}
