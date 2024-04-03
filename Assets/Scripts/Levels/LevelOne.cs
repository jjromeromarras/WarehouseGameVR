using Assets.Scripts.Helper;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelOne : Level
{

    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private pallet[] clientsPallets;

    private int currentTask;
    private Game game;
    private StateGame state;
    private bool waitreading;
    private bool showerror;
    public void Awake()
    {
        currentTask = 0;
    }

    public void Start()
    {
        waitreading = false;
        showerror = false;
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
            case StateGame.ShowLocationPicking:
                {
                    showTexto("IntroducirUbicacion");
                    break;
                }
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
        if (showerror)
        {
            showerror = false;
            infotext.SetActiveInfo(false);
            setLockPlayer(false);
        }
        else
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
                        for (int i = 0; i < clientsPallets.Length; i++)
                        {
                            clientsPallets[i].SetSelected(true);
                        }
                    }
                    break;
            }
        }
    }
    public override int OnSetLocationScanner(string location)
    {
        return 0;
    }

    public override int OnSetContainerScanner(string container, string tag)
    {
        switch (state)
        {
            case StateGame.ScannerContainerClient:
                {
                    if (tag == "ContainerClient")
                    {

                        state = StateGame.ShowLocationPicking;
                        for (int i = 0; i < clientsPallets.Length; i++)
                        {
                            clientsPallets[i].SetSelected(false);
                            if (clientsPallets[i].ssc == container)
                            {
                                clientsPallets[i].gameObject.SetActive(false);
                            }
                        }
                        return 10;
                    }
                    else
                    {
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContainerCliente", 2f, new object[] { container }));
                        return -5;
                    }
                    break;
                }
        }
        return 0;
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
    ShowLocationPicking,
    ScannerLocation,
    ScannerContainer,
    PickingQuantity,
    WaitingReading
}