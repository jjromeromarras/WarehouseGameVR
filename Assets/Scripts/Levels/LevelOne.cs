using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOne : Level
{

    [SerializeField] private GameObject warehousemanual;

    private int currentTask;
    private Game game;
    private StateGame state;
    private bool waitreading;
    public void Awake()
    {
        currentTask = 0;
    }

    public void Start()
    {
        waitreading = false;
        game = new Game(warehousemanual, 1, 5, 1, OrderType.Picking);
        state = StateGame.ShowBienVenido;
        if (infotext != null)
        {
            infotext.onFinishInfoText += FinishInfoText;
            infotext.SetActiveInfo(true);
        }
        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }
    }

    public void Update()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTexto("PrimerBienvenida");
                    break;
                }
            case StateGame.ShowTutorial1:
                {
                    showTexto("SegundaBienvenida");
                    break;
                }
            case StateGame.ShowTutorial2:
                {
                    showTexto("ExplicacionInterfaz1");
                    break;
                }
            case StateGame.ShowTutorial3:
                {
                    showTexto("ExplicacionInterfaz2");
                    break;
                }
            case StateGame.ShowTutorial4:
                {
                    showTexto("ExplicacionInterfaz3");
                    break;
                }
            case StateGame.ShowTutorial5:
                {
                    showTexto("Reto1Tutorial1");
                    break;
                }
            case StateGame.ShowClientContainer:
                {
                    showTexto("ContainerCliente");
                }
                break;
            case StateGame.ShowIntroducirContainerCliente:
                {
                    showTexto("IntroducirContainerCliente");
                }
                break;
            case StateGame.ShowScannerContainer:
                {
                    showTexto("ScannerContainer");
                }
                break;
        }
    }


    private void showTexto(string key)
    {
        if (!waitreading)
        {
            infotext.SetActiveInfo(true);
            waitreading = true;
            StartCoroutine(infotext.SetMessageKey(key, 2f));
        }
    }


    private void FinishInfoText()
    {
        waitreading = false;
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
                    state = StateGame.ShowTutorial3;
                }
                break;
            case StateGame.ShowTutorial3:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowTutorial4;
                }
                break;
            case StateGame.ShowTutorial4:
                {
                    setLockPlayer(true);
                    rfcontroller.SetPantallaTxt("Picking", new object[] { "OS1" });
                    state = StateGame.ShowTutorial5;
                }
                break;
            case StateGame.ShowTutorial5:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowClientContainer;
                }
                break;

            case StateGame.ShowClientContainer:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowIntroducirContainerCliente;
                }
                break;
            case StateGame.ShowIntroducirContainerCliente:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowScannerContainer;
                }
                break;
            case StateGame.ShowScannerContainer:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerContainerClient;
                }
                break;
        }
    }

}

internal enum StateGame
{
    ShowBienVenido,
    ShowTutorial1,
    ShowTutorial2, 
    ShowTutorial3,
    ShowTutorial4,
    ShowTutorial5,
    ShowClientContainer,
    ShowIntroducirContainerCliente,
    ShowScannerContainer,
    ScannerContainerClient,
    ScannerLocation,
    ScannerContainer,
    PickingQuantity,
    WaitingReading
}