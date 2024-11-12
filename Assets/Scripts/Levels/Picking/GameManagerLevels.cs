using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;


public class GameManagerLevels : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject rfmenu;
    [SerializeField] private infotextcontroller infotext;
    [SerializeField] private inforesultcontroller inforesult;
    [SerializeField] private rfcontroller rfcontroller;
    [SerializeField] private fpsBody playerbody;
    [SerializeField] private fpsCamera fpscamera;
    [SerializeField] private forklift forklift;
    [SerializeField] private pickingcamera pickingcamera;
    [SerializeField] private ReceptionCamera receptioncamera;
    [SerializeField] private picking picking;
    [SerializeField] private ReceptionScene reception;
    [SerializeField] public int currentGame;
    [SerializeField] public panelusercontroller paneluser;   
    [SerializeField] private Level[] levels;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject cross;
    [SerializeField] private GameObject infopicking;
    [SerializeField] private GameObject paneloptions;
    [SerializeField] private GameObject warehouse;
    [SerializeField] private pnjwalker[] pnjs;
    [SerializeField] private AudioClip warehouseAmbient, pickingOK, pickingFail;
    [SerializeField] private GameObject contgame;
    [SerializeField] private GameObject contpicking;
    [SerializeField] private Button btngame;
    [SerializeField] private Button btnpicking;
    private GameState _state;
    private GameState _backstate;
    
    
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

        currentGame = GameManager.Instance.minlevel - 1;

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

        if (forklift != null)
        {
            forklift.onScannerContainer += ScannerContainer;
            forklift.onScannerLocation += ScannerLocation;

        }
        for (int i = 0; i< levels.Length; i++)
        {
            if (GameManager.Instance != null)
            {

                if (i != GameManager.Instance.minlevel - 1)
                {
                    levels[i].gameObject.SetActive(false);
                }
                else
                {
                    levels[i].gameObject.SetActive(true);
                }
            }
            levels[i].onSetLockPlayer += SetLockPlayer;
            levels[i].onSetPickingLocation += SetPickingLocation;
            levels[i].onSetReceptionLocation += SetReceptionLocation;
            levels[i].onFinishTask += FinishTask;

        }
       
        if(pickingcamera != null)
        {       
            pickingcamera.onCheckPicking += onCheckPicking;
            pickingcamera.onResetPicking += onResetPicking;
            pickingcamera.onErrorPicking += onErrorPicking;  
            pickingcamera.onCheckContainerPicking += onCheckContainerPicking;
            pickingcamera.onErrorContainerClient += onErrorContainerClient;
        }

        if (receptioncamera != null)
        {
            receptioncamera.onCheckReception += onCheckReception;
            receptioncamera.onResetReception += onResetReception;
            receptioncamera.onCheckItem += onCheckItem;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance != null)
        {
            paneluser.SetScore(new object[] { GameManager.Instance.player.Score });
        }
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (_state != GameState.Pause)
            {
                Cursor.lockState = CursorLockMode.None;
                _backstate = _state;
                _state = GameState.Pause;
            }
            else
            {
                Cursor.lockState = CursorLockMode.Locked;
                _state = _backstate;
            }
            playerbody.setLock(_state == GameState.Pause);
            fpscamera.SetLock(_state == GameState.Pause);
            paneloptions.SetActive(_state == GameState.Pause);
        }
        if ((Input.GetKeyDown(KeyCode.C) || (Input.GetKeyDown(KeyCode.Joystick1Button6))) && _state == GameState.Pause)
        {

            CloseLevel();
        }

        if (_state == GameState.FinishTask)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && inforesult.writefulltext)
            {
                inforesult.SetActiveInfo(false);
                levels[currentGame].gameObject.SetActive(false);
                if (currentGame < 2)
                {
                    currentGame++;
                    minimap.SetActive(GameManager.Instance.showminimap);
                    cross.SetActive(true);
                    rfmenu.SetActive(true);
                    infotext.SetActiveInfo(false);
                    _state = GameState.Traveller;
                    levels[currentGame].gameObject.SetActive(true);
                }
                else
                {
                    GameManager.Instance.WriteLog($"[FinishTask] - Level: {GameManager.Instance.player.Level}");
                    if (GameManager.Instance.player.Level < 2)
                        GameManager.Instance.player.Level = 2;
                    _state = GameState.FinishLevel;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("RetoCompletado", 2f, new object[] { }));
                }
            }
        }
        if (_state == GameState.FinishLevel)
        {
            if ((Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Joystick1Button1)) && infotext.writefulltext)
            {
                CloseLevel();
            }
        }
    }

    #region Private Methods
    private void ScannerContainer(string container, string tag)
    {
        GameManager.Instance.WriteLog($"[ScannerContainer] - container: {container} - tag: {tag}");
        // Comprobar si el contenedor es correcto
        levels[currentGame].OnSetContainerScanner(container, tag);
    }

    private void SetReceptionLocation()
    {
        infopicking.SetActive(true);
        playerbody.gameObject.SetActive(false);
        minimap.SetActive(false);
        cross.SetActive(false);
        for (int i = 0; i < pnjs.Length; i++)
        {
            pnjs[i].StopPNJ();
        }
        warehouse.SetActive(false);
        _state = GameState.Reception;
    }

    private void SetPickingLocation(string stock, string containersscc, shelf shelf, string contclient1, string contclient2, string contclient3, int pedido)
    {
       
        pickingcamera.pedidopreparando = pedido;
        pickingcamera.contclient1 = "F1:"+contclient1;
        pickingcamera.contclient2 = "F2:"+contclient2;
        pickingcamera.contclient3 = "F3:"+contclient3;
        pickingcamera.selectclient = 1;
        pickingcamera.ResetScene();
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
            if (!String.IsNullOrEmpty(item.ssc))
            {
                item.gameObject.SetActive(true);
                var stockpallet = Enum.GetValues(typeof(Stock)).GetValue(UnityEngine.Random.Range(0, 7)).ToString();
                if (item.ssc == containersscc)
                {
                    stockpallet = stock;
                }

                if (item.transform.localPosition.y < 2)
                {
                    // Abajo
                    if (item.transform.localPosition.x > 1)
                    {
                        // izquierda
                        picking.setContainer(0, stockpallet, 12, item.ssc);
                    }
                    else if (item.transform.localPosition.x < 1 && item.transform.localPosition.x > -1)
                    {
                        // centrado
                        picking.setContainer(1, stockpallet, 12, item.ssc);
                    }
                    else
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
            } else
            {
                item.gameObject.SetActive(false);
            }
        }
       
    }
    private bool onCheckContainerPicking(string container)
    {
        var result = levels[currentGame].CheckContainerPicking(container);
        if(!result) {
            GameManager.Instance.WriteLog($"[onCheckContainerPicking] - container: {container} Fail");
            SoundManager.SharedInstance.PlaySound(pickingFail);
        }
        GameManager.Instance.WriteLog($"[onCheckContainerPicking] - container: {container} OK");
        return result;

    }

    private void onErrorContainerClient()
    {
        GameManager.Instance.WriteLog($"[onCheckContainerPicking] - onErrorContainerClient");
        levels[currentGame].onErrorContainerClient();
        SoundManager.SharedInstance.PlaySound(pickingFail);
    }

    private void onCheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        GameManager.Instance.WriteLog($"[onCheckPicking] - cantplatano: {cantplatano} - cantuvas: {cantuvas} " +
            $"- cantpiña: {cantpiña} - cantmelocoton: {cantmelocoton} - cantmanzana: {cantmanzana} - cantfresa: {cantfresa}");

        var result = levels[currentGame].CheckPicking(cantplatano, cantuvas, cantpiña, cantperas, cantmelocoton, cantmanzana, cantfresa);        
        if (result)
        {
            GameManager.Instance.WriteLog($"[onCheckPicking] - Picking OK");
            SoundManager.SharedInstance.PlaySound(pickingOK);
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
        else
        {
            GameManager.Instance.WriteLog($"[onCheckPicking] - Picking Fail");
            SoundManager.SharedInstance.PlaySound(pickingFail);
        }
    }

    private void onCheckItem(bool value)
    {
        GameManager.Instance.WriteLog($"[onCheckItem] - value: {value}");
        var result = (levels[currentGame] as ReceptionLevel).CheckItem(value);
        if (result)
        {
            GameManager.Instance.WriteLog($"[onCheckItem] - Reception OK");
            receptioncamera.ResetScene();
            SoundManager.SharedInstance.PlaySound(pickingOK);
            reception.gameObject.SetActive(false);
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
    private void onCheckReception(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        GameManager.Instance.WriteLog($"[onCheckReception] - cantplatano: {cantplatano} - cantuvas: {cantuvas} " +
            $"- cantpiña: {cantpiña} - cantmelocoton: {cantmelocoton} - cantmanzana: {cantmanzana} - cantfresa: {cantfresa}");

        var result = levels[currentGame].CheckPicking(cantplatano, cantuvas, cantpiña, cantperas, cantmelocoton, cantmanzana, cantfresa);
        if (result)
        {
            GameManager.Instance.WriteLog($"[onCheckPicking] - Reception OK");
            receptioncamera.ResetScene();
            SoundManager.SharedInstance.PlaySound(pickingOK);
            reception.gameObject.SetActive(false);
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
        else
        {
            GameManager.Instance.WriteLog($"[onCheckPicking] - Picking Fail");
            SoundManager.SharedInstance.PlaySound(pickingFail);
        }
    }

    private void onResetPicking()
    {
        GameManager.Instance.WriteLog($"[onResetPicking]");
        SoundManager.SharedInstance.PlaySound(pickingFail); 
        levels[currentGame].onResetTask();
        pickingcamera.ResetScene();

    }

    private void onResetReception()
    {
        GameManager.Instance.WriteLog($"[onResetReception]");
        SoundManager.SharedInstance.PlaySound(pickingFail);
        levels[currentGame].onResetTask();
        receptioncamera.ResetScene();

    }

    private void onErrorPicking()
    {
        GameManager.Instance.WriteLog($"[onErrorPicking]");
        levels[currentGame].onErrorPicking();
        SoundManager.SharedInstance.PlaySound(pickingFail);
    }


    private void ScannerLocation(string location, string tag)
    {
        GameManager.Instance.WriteLog($"[ScannerLocation] - location: {location} - tage: {tag}");

        levels[currentGame].OnSetLocationScanner(location, tag);
    }


    private void SetLockPlayer(bool value)
    {
        playerbody.setLock(value);
        fpscamera.SetLock(value);
    }

    private void FinishTask(int time, int bonificacion, int fallos)
    {
        StartCoroutine(ActiveFinish(time, bonificacion, fallos));
    }

    private IEnumerator ActiveFinish(int time, int bonificacion, int fallos)
    {
        GameManager.Instance.WriteLog($"[ActiveFinish] - Game 1: {levels[currentGame].game.Name}");

        SoundManager.SharedInstance.PlaySound(pickingOK);
        inforesult.SetActiveInfo(true);
        minimap.SetActive(false);
        cross.SetActive(false);
        rfmenu.SetActive(false);
        playerbody.setLock(true);
        fpscamera.SetLock(true);
        GameManager.Instance.player.Score += time;
        inforesult.SetResult((int)GameManager.Instance.player.Score, time, fallos, bonificacion);
        infotext.SetActiveInfo(false);
        _state = GameState.FinishTask;
        yield return inforesult.SetMessageKey(2f, new object[] { });
    }
    #endregion

    #region Public Methods
    public void CloseOption()
    {
        playerbody.setLock(false);
        fpscamera.SetLock(false);
        paneloptions.SetActive(false);
    }

    public void RestartLevel()
    {
        GameManager.Instance.WriteLog($"[RestartLevel] - Game 1: {levels[currentGame].game.Name}");
        GameManager.Instance.player.Score -= 500;
        StartCoroutine(GameManager.Instance.ResetLevel());
    }
    public void CloseLevel()
    {
        GameManager.Instance.WriteLog($"[CloseLevel] - Game 1: {levels[currentGame].game.Name}");  
        StartCoroutine(GameManager.Instance.BackMenu());
    }
    #endregion

    public void SetBtnJuego()
    {
        btngame.image.color = Color.green;
        btnpicking.image.color = Color.white;
        contgame.SetActive(true);
        contpicking.SetActive(false);
    }

    public void SetBtnPicking()
    {
        btngame.image.color = Color.white;
        btnpicking.image.color = Color.green;
        contgame.SetActive(false);
        contpicking.SetActive(true);

    }
}

