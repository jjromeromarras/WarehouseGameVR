using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject rfmenu;

    public infotextcontroller infotext;

    public rfcontroller rfcontroller;

    public fpsBody playerbody;

    private bool showrfmenu;
    void Start()
    {        
        rfmenu.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            
            showrfmenu = !showrfmenu;
            rfmenu.SetActive(showrfmenu);
            infotext.SetActiveInfo(showrfmenu);
            playerbody.setLock(showrfmenu);
            if (showrfmenu)
            {
                StartCoroutine(infotext.SetMessage($"prueba de concepto de texto de ayuda cuando se muestra la rf", 2f));
            }
        }
    }
}
