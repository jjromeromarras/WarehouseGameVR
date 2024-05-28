using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerLevel1 : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rfmenu;
    [SerializeField] private infotextcontroller infotext;
    [SerializeField] private inforesultcontroller inforesult;
    [SerializeField] private rfcontroller rfcontroller;
    [SerializeField] private fpsBody playerbody;
    [SerializeField] private pickingcamera pickingcamera;
    [SerializeField] private picking picking;
    [SerializeField] public int currentGame;
    [SerializeField] public panelusercontroller paneluser;   
    [SerializeField] private Level[] levels;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject cross;
    [SerializeField] private GameObject infopicking;
    [SerializeField] private GameObject warehouse;
    [SerializeField] private pnjwalker[] pnjs;
    [SerializeField] private AudioClip warehouseAmbient;
    private bool showrfmenu;
    private GameState _state;
    
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _state = GameState.Traveller;
        if(warehouse != null)
        {
            warehouse.SetActive(true);
        }

        if(minimap != null)
        {
           minimap.SetActive(GameManager.Instance.showminimap);
        }

        if(cross != null)
        {
            cross.SetActive(true);
        }
        if(infopicking != null)
        {
            infopicking.SetActive(false);
        }

        if(picking != null)
        {
            picking.gameObject.SetActive(false);    
        }
        currentGame = 0;
       

    }
    void Start()
    {        
        rfmenu.SetActive(true);
        SetLockPlayer(true);
        SoundManager.SharedInstance.PlayMusic(warehouseAmbient);
        if (playerbody != null)
        {
            playerbody.onScannerContainer += ScannerContainer;
            playerbody.onScannerLocation += ScannerLocation;
       
        }

        for(int i = 0; i< levels.Length; i++)
        {
            levels[i].onSetLockPlayer += SetLockPlayer;
            levels[i].onSetPickingLocation += SetPickingLocation;
            levels[i].onFinishLevel += FinishLevel;

        }
       
        if(pickingcamera != null)
        {
            pickingcamera.onCancelPickingLocation += CancelPickingLocation;
            pickingcamera.onCheckPicking += onCheckPicking;
            pickingcamera.onResetPicking += onResetPicking;
        }
         
    }

    // Update is called once per frame
    void Update()
    {
        paneluser.SetScore(new object[] { GameManager.Instance.player.Score });
        if (_state == GameState.Traveller)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {

                showrfmenu = !showrfmenu;
                rfmenu.SetActive(showrfmenu);
                playerbody.setLock(showrfmenu);
            }
        }
        else if (_state == GameState.FinishLevel)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (currentGame < 2)
                {
                    levels[currentGame].gameObject.SetActive(false);
                    currentGame++;
                    inforesult.SetActiveInfo(false);
                    minimap.SetActive(GameManager.Instance.showminimap);
                    cross.SetActive(true);
                    rfmenu.SetActive(true);
                    playerbody.setLock(false);
                    infotext.SetActiveInfo(false);
                    _state = GameState.Traveller;
                    levels[currentGame].gameObject.SetActive(true);

                }
            }
        }
    }

    #region Private Methods
    private void ScannerContainer(string container, string tag)
    {
        // Comprobar si el contenedor es correcto
       levels[currentGame].OnSetContainerScanner(container, tag);
    }

    private void CancelPickingLocation()
    {
        picking.gameObject.SetActive(false);
        warehouse.SetActive(true);
        playerbody.gameObject.SetActive(true);
        cross.SetActive(true);
        for (int i = 0; i < pnjs.Length; i++)
        {
            pnjs[i].ResumePNJ();
        }

        minimap.SetActive(GameManager.Instance.showminimap);
        infopicking.SetActive(false);
        SetLockPlayer(false);
        GameManager.Instance.player.Score -= 5;
        levels[currentGame].OnExistPickingScene();
        _state = GameState.Traveller;
    }

    private void SetPickingLocation(string stock, string containersscc, shelf shelf)
    {
        picking.gameObject.SetActive(true);
        infopicking.SetActive(true);
        playerbody.gameObject.SetActive(false);
        minimap.SetActive(false);
        cross.SetActive(false);
        for(int i=0; i<pnjs.Length; i++)
        {
            pnjs[i].StopPNJ();
        }
        warehouse.SetActive(false);
        
        pickingcamera.ResetScene();
        _state = GameState.Picking;
        List<pallet> containers = new List<pallet>();
        for (int i=0; i<6; i++)
        {
            var container = shelf.transform.GetChild(i).GetComponent<pallet>();
            if (container != null)
            {
                containers.Add(container);
            }
        }

        picking.ResetSetSSC();

        foreach (var item in containers)
        {
            var stockpallet = Enum.GetValues(typeof(Stock)).GetValue(UnityEngine.Random.Range(0, 7)).ToString();
            if (item.ssc == containersscc)
            {
                stockpallet = stock;
            }
           
            if (item.transform.localPosition.y < 2)
            {
                // Abajo
                if(item.transform.localPosition.x > 1)
                {
                    // izquierda
                    picking.setContainer(0, stockpallet, 12, item.ssc);
                } else if (item.transform.localPosition.x < 1 && item.transform.localPosition.x > -1)
                {
                    // centrado
                    picking.setContainer(1, stockpallet, 12, item.ssc);
                } else
                {
                    // derecha
                    picking.setContainer(2, stockpallet, 12, item.ssc);
                }
            }
            else
            {
                // Arriba
                if (item.transform.localPosition.x > 1)
                {
                    // izquierda
                    picking.setContainer(3, stockpallet, 12, item.ssc);
                }
                else if (item.transform.localPosition.x < 1 && item.transform.localPosition.x > -1)
                {
                    // centrado
                    picking.setContainer(4, stockpallet, 12, item.ssc);
                }
                else
                {
                    // derecha
                    picking.setContainer(5, stockpallet, 12, item.ssc);
                }
            }
        }
       
    }

    private void onCheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        var result = levels[currentGame].CheckPicking(cantplatano, cantuvas, cantpiña, cantperas, cantmelocoton, cantmanzana, cantfresa);        
        if (result)
        {            
            picking.gameObject.SetActive(false);
            warehouse.SetActive(true);
            for (int i = 0; i < pnjs.Length; i++)
            {
                pnjs[i].ResumePNJ();
            }

            playerbody.gameObject.SetActive(true);
            cross.SetActive(true);
            minimap.SetActive(GameManager.Instance.showminimap);
            infopicking.SetActive(false);
            SetLockPlayer(false);
            _state = GameState.Traveller;
        }
    }

    private void onResetPicking()
    {
        GameManager.Instance.player.Score -= 5;
        levels[currentGame].onResetTask();
        pickingcamera.ResetScene();

    }

    private void ScannerLocation(string location, string tag)
    {
        levels[currentGame].OnSetLocationScanner(location, tag);
    }


    private void SetLockPlayer(bool value)
    {
        playerbody.setLock(value);
    }

    private void FinishLevel(int time, int bonificacion, int fallos)
    {
        StartCoroutine(ActiveFinish(time, bonificacion, fallos));
    }

    private IEnumerator ActiveFinish(int time, int bonificacion, int fallos)
    {
        inforesult.SetActiveInfo(true);
        minimap.SetActive(false);
        cross.SetActive(false);
        rfmenu.SetActive(false);
        playerbody.setLock(true);
        GameManager.Instance.player.Score += (bonificacion + time - fallos);
        inforesult.SetResult((int)GameManager.Instance.player.Score, time, fallos, bonificacion);
        infotext.SetActiveInfo(false);
        _state = GameState.FinishLevel;
        yield return inforesult.SetMessageKey(2f, new object[] { });
    }
    #endregion

}
