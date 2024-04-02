using Assets.Scripts.Helper;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private GameObject rfmenu;

    [SerializeField] private infotextcontroller infotext;

    [SerializeField] private rfcontroller rfcontroller;

    [SerializeField] private fpsBody playerbody;

    [SerializeField] private picking picking;

    [SerializeField] private pnjwalker[] pnjwalkers;
    [SerializeField] private forklift[] forklifts;
    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private GameObject warehouseautomatico;

    [SerializeField] private Level[] levels;

    private bool showrfmenu;
    private GameState _state; 
    public int currentGame;
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _state = GameState.Traveller;
        currentGame = 0;
       

    }
    void Start()
    {        
        rfmenu.SetActive(true);
        SetLockPlayer(true);
        if (playerbody != null )
        {
            playerbody.onScannerContainer += ScannerContainer;
            playerbody.onScannerLocation += ScannerLocation;
        }

        for(int i = 0; i< levels.Length; i++)
        {
            levels[i].onSetLockPlayer += SetLockPlayer;
        }
       
       /* games[0]= new Game(warehousemanual, 1, 5, 1, OrderType.Picking);
        games[1] = new Game(warehousemanual, 2, 8, 2, OrderType.Picking);
        games[2] = new Game(warehouseautomatico, 1, 3, 3, OrderType.Shipping);
        games[3] = new Game(warehouseautomatico, 2, 7, 4, OrderType.Shipping);   */     
    }

    // Update is called once per frame
    void Update()
    {
        if (_state == GameState.Traveller)
        {
            if (Input.GetKeyDown(KeyCode.T))
            {

                showrfmenu = !showrfmenu;
                rfmenu.SetActive(showrfmenu);               
                playerbody.setLock(showrfmenu);                
            }
        }
        else if (_state == GameState.Picking)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                picking.gameObject.SetActive(false);
                playerbody.gameObject.SetActive(true);
                _state = GameState.Traveller;
                for(int i = 0; i < pnjwalkers.Length; i++) {
                    pnjwalkers[i].ResumePNJ();
                }
            }
        }
    }

    private void ScannerContainer(string container)
    {
        // Comprobar si el contenedor es correcto

        for (int i = 0; i < pnjwalkers.Length; i++)
        {
            pnjwalkers[i].StopPNJ();
        }

        picking.gameObject.SetActive(true);
        playerbody.gameObject.SetActive(false);
        var numcontainer = Random.Range(1, 7);
        Debug.Log("Contenedor: " + numcontainer.ToString());
        picking.setContainer(numcontainer, Stock.manzanas.ToString(), 12);
        _state = GameState.Picking;
    }

    private void ScannerLocation(string location)
    {
        
    }

    private void SetLockPlayer(bool value)
    {
        playerbody.setLock(value);
    }
    
}
