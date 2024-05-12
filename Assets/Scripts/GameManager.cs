using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using static UnityEditor.FilePathAttribute;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject rfmenu;

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

    private bool showrfmenu;
    private GameState _state;
    private Player player;
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
            minimap.SetActive(true);
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
        player = new Player();

    }
    void Start()
    {        
        rfmenu.SetActive(true);
        SetLockPlayer(true);
    
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
       /* games[0]= new Game(warehousemanual, 1, 5, 1, OrderType.Picking);
        games[1] = new Game(warehousemanual, 2, 8, 2, OrderType.Picking);
        games[2] = new Game(warehouseautomatico, 1, 3, 3, OrderType.Shipping);
        games[3] = new Game(warehouseautomatico, 2, 7, 4, OrderType.Shipping);   */     
    }

    // Update is called once per frame
    void Update()
    {
        paneluser.SetScore(new object[] { player.Score });
        if (_state == GameState.Traveller)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {

                showrfmenu = !showrfmenu;
                rfmenu.SetActive(showrfmenu);               
                playerbody.setLock(showrfmenu);                
            }
        }        
    }

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
        minimap.SetActive(true);
        infopicking.SetActive(false);
        SetLockPlayer(false);
        player.Score -= 5;
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

    private void onCheckPicking(int cantplatano, int cantuvas, int cantpi�a, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        var result = levels[currentGame].CheckPicking(cantplatano, cantuvas, cantpi�a, cantperas, cantmelocoton, cantmanzana, cantfresa);        
        if (result)
        {            
            picking.gameObject.SetActive(false);
            warehouse.SetActive(true);
            playerbody.gameObject.SetActive(true);
            cross.SetActive(true);
            minimap.SetActive(true);
            infopicking.SetActive(false);
            SetLockPlayer(false);
            _state = GameState.Traveller;
        }
    }

    private void onResetPicking()
    {
        player.Score -= 5;
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
        player.Score += (bonificacion + time - fallos);
        inforesult.SetResult((int)player.Score, time, fallos, bonificacion);
        infotext.SetActiveInfo(false);
        _state = GameState.FinishLevel;
        yield return inforesult.SetMessageKey("nivelcompletado", 2f, new object[] { });
    }


}
