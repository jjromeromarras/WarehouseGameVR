using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UnloadTruckLevel : Level
{

    #region Fields
    [SerializeField] private pallet[] pallets;
    [SerializeField] private StageTruck pulmon;
    [SerializeField] private GameObject selectForklift;
    [SerializeField] private ForkliftPickup forkliftPickup;
    [SerializeField] private AudioClip pickingOK, pickingFail;

    private StateGame state;
    private Task currentTask;
    private Queue<Task> tasks;
    private bool isfirstContainer;
    #endregion

    #region Public Methods  
    // Start is called before the first frame update
    void Start()
    {
        InitLevel();
        game = new Game("UnloadTruckLevel", "UnloadTruckLevel", 5);
        isfirstContainer = true;
        tasks = new Queue<Task>();

        foreach(var pallet in pallets)
        {
            Task t = new Task();
            t.ContainerRef = pallet;
            t.Container = pallet.ssc;
            tasks.Enqueue(t);
        }

        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("ForkliftTask");
        }

        state = StateGame.ShowBienVenido;
        if (timer != null)
        {
            timer.SetTimeLeft(900f);
        }
        forkliftPickup.onUnloadPallet += onUnloadPallet;
        forkliftPickup.loadPallet += onloadPallet;
        forkliftPickup.destiny = "R01";

        GameManager.Instance.WriteLog($"Iniciar game: UnloadTruckLevel");
        currentTask = tasks.Dequeue();
        rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { "M2", currentTask.ContainerRef.ssc, "R 01", pallets.Length.ToString(), pallets.Length.ToString() });
        if (txtNivel != null)
        {
            txtNivel.SetPantallaTxt("niveldescargarcamion", new object[] { });
        }
        GameManager.Instance.WriteLog($"[UnloadTruckTask] - Next Task: Pallet: {currentTask.ContainerRef.ssc}");

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTextoKey("BienvenidaForklift");
                    break;
                }
            case StateGame.ShowTutorial1:
                {
                    showTextoKey("Tutorial1UnloadTruck");
                    break;
                }
            case StateGame.ShowTutorial2:
                {
                    showTextoKey("Tutorial2UnloadTruck");
                    break;
                }
            case StateGame.ShowFinishTutorial:
                {
                    showTextoKey("FinishTutorialUnloadTruck");
                    break;
                }
            case StateGame.ShowFirstContainer:
                {
                    showTextoKey("FirstContainerUnload");
                    break;
                }
            case StateGame.ShowErrorUnloadContainer:
                {
                    showTextoKey("ErrorContainerUnload");
                    break;
                }
        }
    }

    public override void NextStep()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowTutorial1;
                }
                break;
            case StateGame.ShowTutorial1:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowTutorial2;
                }
                break;
            case StateGame.ShowTutorial2:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowFinishTutorial;
                }
                break;
            case StateGame.ShowFirstContainer:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    currentTask = tasks.Dequeue();
                    currentTask.ContainerRef.gameObject.SetActive(true);
                    currentTask.ContainerRef.SetSelected(true);
                    rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { "M2", currentTask.ContainerRef.ssc, "R 01", tasks.Count.ToString(), pallets.Length.ToString() });
                    GameManager.Instance.WriteLog($"[UnloadTruckTask] - Next Task: Pallet: {currentTask.ContainerRef.ssc}");

                }
                break;
            case StateGame.ShowFinishTutorial:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    currentTask.ContainerRef.gameObject.SetActive(true);
                    currentTask.ContainerRef.SetSelected(true);
                    selectForklift.SetActive(true);
                }
                break;
            case StateGame.ShowErrorUnloadContainer:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    break;
                }
        }



    }


    #endregion

    #region Private
    private void onloadPallet(string container)
    {
        if (isfirstContainer)
        {
            pulmon.SetSelected(true);
        }
    }
    private void onUnloadPallet(string container)
    {
        if (container == currentTask.Container)
        {
            GameManager.Instance.WriteLog($"[UnloadTruckTask] - onUnloadPallet: {container} OK");
            AddBonificacion(5);
            SoundManager.SharedInstance.PlaySound(pickingOK);
            if (isfirstContainer)
            {
                state = StateGame.ShowFirstContainer;
                isfirstContainer = false;
            }
            else
            {
                if (tasks.Count > 0)
                {
                    currentTask = tasks.Dequeue();
                    currentTask.ContainerRef.gameObject.SetActive(true);
                    currentTask.ContainerRef.SetSelected(true);
                    rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { "M2", currentTask.ContainerRef.ssc, "STG01", tasks.Count+1, pallets.Length });
                    GameManager.Instance.WriteLog($"[UnloadTruckTask] - Next Task: Pallet: {currentTask.ContainerRef.ssc}");

                }
                else
                {
                    this.setFinishLevel();
                }
            }
        }
        else
        {
            state = StateGame.ShowErrorUnloadContainer;
            SoundManager.SharedInstance.PlaySound(pickingFail);
            AddPenalty(10);
            GameManager.Instance.WriteLog($"[UnloadTruckTask] - onUnloadPallet: {container} ERROR");
        }
    }
    #endregion

    internal enum StateGame
    {
        ShowBienVenido,
        ShowTutorial1,
        ShowTutorial2,
        LoadContainer,
        UnloadContainer,
        ShowFinishTutorial,
        ShowFinishLevel,
        ShowFirstContainer,
        ShowErrorUnloadContainer,
        FinishLevel
    }
}

