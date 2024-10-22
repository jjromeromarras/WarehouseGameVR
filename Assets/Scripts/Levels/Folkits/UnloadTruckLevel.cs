using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.FilePathAttribute;

public class UnloadTruckLevel : Level
{

    #region Fields
    [SerializeField] private pallet[] pallets;
    [SerializeField] private StageTruck pulmon;
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
        game = new Game("UnloadTruckLevel", "UnloadTruckLevel");
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
            timer.SetTimeLeft(300f);
        }
        forkliftPickup.onUnloadPallet += onUnloadPallet;


        GameManager.Instance.WriteLog($"Iniciar game: UnloadTruckLevel");
        currentTask = tasks.Dequeue();
        rfcontroller.SetPantallaTxt("UnloadTruckTask", new object[] { "M2", currentTask.ContainerRef.ssc, "R 01", pallets.Length.ToString(), pallets.Length.ToString() });

    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTexto("BienvenidaForklift");
                    break;
                }
            case StateGame.ShowTutorial1:
                {
                    showTexto("Tutorial1UnloadTruck");
                    break;
                }
            case StateGame.ShowTutorial2:
                {
                    showTexto("Tutorial2UnloadTruck");
                    break;
                }
            case StateGame.ShowFinishTutorial:
                {
                    showTexto("FinishTutorialUnloadTruck");
                    break;
                }
            case StateGame.ShowFirstContainer:
                {
                    showTexto("FirstContainerUnload");
                    break;
                }
            case StateGame.ShowErrorUnloadContainer:
                {
                    showTexto("ErrorContainerUnload");
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

                }
                break;
            case StateGame.ShowFinishTutorial:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.LoadContainer;
                    currentTask.ContainerRef.gameObject.SetActive(true);
                    currentTask.ContainerRef.SetSelected(true);
                    pulmon.SetSelected(true);
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
    private void onUnloadPallet(string container)
    {
        if (container == currentTask.Container)
        {
            bonificacion += 5;
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
            penalizacion += 10;
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

