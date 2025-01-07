using Assets.Scripts.Helper;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PickingLevel : Level
{

    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private pallet[] clientsPallets;   
    [SerializeField] private bool variospedidos;
    public IARetosPicking retoia;

    private Task currentTask; 
    private Queue<Task> tasks;
    private StateGame state;
 



    #region Public Methods  

    public void Start()
    {

        InitLevel();

        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }

        if (!GameManager.Instance.UsedIA || numberlevel == 1)
        {
            // El nivel 1 siempre es tutorial del juego
            switch (numberlevel)
            {
                case 1:
                    game = new GamePicking(warehousemanual, 1, 1, OrderType.Picking, "Tutorial Picking");
                    state = StateGame.ShowBienVenido;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(300f);
                    }
                    break;
                case 2:
                    game = new GamePicking(warehousemanual, 1, 10, OrderType.Picking, "Preparar un pedido");
                    showhelp = true;
                    state = StateGame.ShowTutorial2;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(900f);
                    }
                    break;
                case 3:
                    game = new GamePicking(warehousemanual, 3, 8, OrderType.Picking, "Multi pedidos");
                    showhelp = true;
                    state = StateGame.ShowTutorial3;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(1800f);
                    }
                    break;
            }
            GameManager.Instance.WriteLog($"Iniciar game: {game.Name}");
            tasks = new Queue<Task>();
            (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
            currentTask = tasks.Dequeue();
            if (txtNivel != null)
            {
                txtNivel.SetPantallaTxt("level1", new object[] { });
            }
        } 
        else
        {
            // En este punto estamos trabajando la IA no estamos en el tutorial
            // Le pedimos a la IA que nos de el siguiente reto a realizar
            string prompt = $"El jugador se dispone a realizar los retos de preparaci�n de pedidos. Su nivel general es {GameManager.Instance.player.playerClassification.GetLevel4Category("General")}." +
                $"Y el nivel en la categoria de Preparaci�n de Pedidos es {GameManager.Instance.player.playerClassification.GetLevel4Category("PreparacionPedidos")}." +
                $"Sabiendo cual es su nivel necesito que me indiques cual seria el siguiente a realizar para alcanzar el siguiente nivel." +
                $"La estructura de la respuesta es\r\n{{\r\n  \"Ordenes\": valor,     //Indica el N�mero de �rdenes a realizar. Valor entre 1 a 3.   \t \r\n  \"Tareas\": valor,        //indica el N�mero de tareas. Valor puede ser entre 1 y 20. \t\r\n  \"Multireferencia\": valor, \t//es true (s�) o false (no), seg�n incluyan contenedores multireferencia. Si es True mas dificil es el reto, solo adapto para el nivel experto.\r\n  \"Nivel\": valor, \t//es el Nivel de dificultad. Puede ser Principiante, Medio, Avanzado, Experto seg�n dificultad del reto.\t\t\r\n  \"Tiempo\": valor, \t//es el Tiempo en minutos que deber�a el jugador de completar el reto en funci�n del nivel, ordenes y n�mero de tareas. Tiempo m�ximo para un reto de nivel experto 30 minutos.\r\n  \"Fallos\": valor, \t// es un entero que indica el N�mero m�ximo de fallos permitidos. Cuando mas fallos menor dificultad.\r\n  \"Explicacion\": valor // Cadena de texto con la explicaci�n para mostrarsela al jugador del reto a realizar.\r\n}}\r\nA mayor n�mero de ordenes, tareas multireferencias y menor fallos mayor nivel de dificultad. \r\nNecesito un reto para el siguiente nivel que el jugador necesite para llegar alcanzar el nivel experto (el cual seria el ultimo reto),\r\nRespondeme solamente con la estructura indicada para el primer reto.";
            GameManager.Instance.SendIAMsg(prompt);
            state = StateGame.WaitingIAReto;
        }
        
    }

    public void Update()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    showTextoKey("PrimerBienvenida");
                    break;
                }
            case StateGame.ShowTutorial1:
                {
                    showTextoKey("Reto1Tutorial1");
                    break;
                }
            case StateGame.ShowTutorial2:
                {
                    showTextoKey("Reto1Level2");
                    break;
                }
            case StateGame.ShowTutorial3:
                {
                    showTextoKey("Reto2Level3");
                    break;
                }
            case StateGame.ShowClientContainer:
                {
                    showTextoKey("ContainerCliente");
                }
                break;
            case StateGame.ShowIntroducirContainerCliente:
                {
                    setLockPlayer(true);
                    showTextoKey("IntroducirContainerCliente");
                }
                break;
            case StateGame.ShowScannerContainer:
                {
                    setLockPlayer(true);
                    showTextoKey("ScannerContainer");
                }
                break;
            case StateGame.ShowLocationPicking:
                {
                    setLockPlayer(true);
                    showTextoKey("IntroducirUbicacion");
                    break;
                }
            case StateGame.ShowContainerPicking:
                {
                    setLockPlayer(true);
                    showTextoKey("IntroducirContenedor");
                    break;
                }
            case StateGame.ShowIntroducirArticulo:
                {                    
                    showTextoKey("IntroducirArticulo");
                    break;
                }           
            case StateGame.ShowDockConfirmation:
                {
                    showTextoKey("confirmdock");
                    break;
                }
            case StateGame.ShowFinishLevel:
                {
                    timer.SetTimerOn(false);
                    infotext.SetActiveInfo(true);
                    waitreading = true;
                    StartCoroutine(infotext.SetMessageKey("nivelcompletado", 2f));                    
                    break;
                }
            case StateGame.ShowMsgIA:
                {
                    showMsg(retoia.Explicacion);
                    break;
                }
            case StateGame.WaitingIAReto:
                {
                    if (!GameManager.Instance.wait4IAResponse)
                    {
                        retoia = JsonConvert.DeserializeObject<IARetosPicking>(GameManager.Instance.IAResponse);
                        if (retoia != null)
                        {
                            game = new GamePicking(warehousemanual, retoia.Ordenes, retoia.Tareas, OrderType.Picking, $"Reto IA:{retoia.Nivel}");
                            if (timer != null)
                            {
                                timer.SetTimeLeft(retoia.Tiempo * 60);
                            }
                            showhelp = false;
                            GameManager.Instance.WriteLog($"Iniciar game: {game.Name}");
                            tasks = new Queue<Task>();
                            (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
                            currentTask = tasks.Dequeue();
                            if (txtNivel != null)
                            {
                                txtNivel.SetPantallaTxt("level1", new object[] { });
                            }
                            state = StateGame.ShowMsgIA;
                        }
                    }
                    break;
                }

        }
    }
   

    public override void OnSetDockScanner(string dock, string tag)
    {

        if (tag == "dock")
        {
            if (dock == currentTask.parentOrder.Dock)
            {
                SoundManager.SharedInstance.PlaySound(scannerOK);                
                timer.SetTimerOn(false);
                GameManager.Instance.player.Data[0].TotalTime += timer.TimeLeft;
                bonificacion += 10;
                GameManager.Instance.player.Score += 10;
                currentTask.parentOrder.ContainerClient = string.Empty;
                if (!GetTask())
                {
                    state = StateGame.FinishLevel;
                    this.setFinishLevel();
                }
        
            }
            else
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;

            }
        }
        else if (!showerror && !waitreading)
        {
            SoundManager.SharedInstance.PlaySound(scannerError);
            showerror = true;
            infotext.SetActiveInfo(true);
            StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
            penalizacion += 5;
            GameManager.Instance.player.Score -= 5;
        }
      

    }

    public override void OnSetLocationScanner(string location, string tag)
    {
        if (state == StateGame.ScannerLocation)
        {
            if (tag == "Ubicacion")
            {
                if (currentTask is PickingTask picking)
                {
                    if (location == picking.Location)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerOK);
                        picking.locationScan = true;
                        state = StateGame.ShowContainerPicking;
                        rfcontroller.SetPantallaTxt("EnterContainer", new object[] { picking.Stock, picking.Container,
                        currentTask.parentOrder.ContainerClient, picking.Quantity});
                        bonificacion += 5;
                        GameManager.Instance.player.Score += 5;
                        GameManager.Instance.player.Data[0].Aciertos += 1;
                    }
                    else
                    {
                        if (!showerror && !waitreading)
                        {
                            SoundManager.SharedInstance.PlaySound(scannerError);
                            showerror = true;
                            infotext.SetActiveInfo(true);
                            StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { location }));
                            penalizacion += 5;
                            GameManager.Instance.player.Score -= 5;
                            GameManager.Instance.player.Data[0].Errors += 1;
                        }

                    }
                }

            }
            else
            {
                if (!showerror && !waitreading)
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    showerror = true;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { location }));
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;
                    GameManager.Instance.player.Data[0].Errors += 1;
                }

            }
        }
        else if (state == StateGame.ScannerDock)
        {
            OnSetDockScanner(location, tag);
        }
        else
        {
            if (!showerror && !waitreading)
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContainerCliente", 2f, new object[] { location }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
                GameManager.Instance.player.Data[0].Errors += 1;
            }

        }
    }

    public override void OnSetContainerScanner(string container, string tag)
    {
        if (state == StateGame.ScannerContainerClient)
        {
            if (tag == "ContainerClient")
            {
                SoundManager.SharedInstance.PlaySound(scannerOK);
                state = StateGame.ShowLocationPicking;
                for (int i = 0; i < clientsPallets.Length; i++)
                {
                    clientsPallets[i].SetSelected(false);
                    if (clientsPallets[i].ssc == container)
                    {
                        clientsPallets[i].gameObject.SetActive(false);
                        currentTask.parentOrder.ContainerClient = container;
                    }
                }
                if (currentTask is PickingTask picking)
                {
                    rfcontroller.SetPantallaTxt("EnterLocation", new object[] { picking.Location, currentTask.parentOrder.Name,
                        picking.Stock, currentTask.parentOrder.ContainerClient});
                }
                bonificacion += 5;
                GameManager.Instance.player.Score += 5;
                GameManager.Instance.player.Data[0].Aciertos += 1;
            }
            else
            {
                if (!showerror && !waitreading)
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    showerror = true;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContainerCliente", 2f, new object[] { container }));
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;
                    GameManager.Instance.player.Data[0].Errors += 1;
                }

            }
        }
        else if (state == StateGame.ScannerContainer)
        {
            if (currentTask is PickingTask picking)
            {
                if (picking.Container == container)
                {
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    state = StateGame.ShowIntroducirArticulo;
                    rfcontroller.SetPantallaTxt("EnterArticulo", new object[] { picking.Stock, picking.Container,
                        currentTask.parentOrder.ContainerClient, picking.Quantity});
                    setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, (game as GamePicking).Orders[0].ContainerClient,
                        (game as GamePicking).Orders.Count > 1 ? (game as GamePicking).Orders[1].ContainerClient : string.Empty, (game as GamePicking).Orders.Count > 2 ? (game as GamePicking).Orders[2].ContainerClient : string.Empty, currentTask.parentOrder.Level);
                    bonificacion += 5;
                    GameManager.Instance.player.Score += 5;
                    GameManager.Instance.player.Data[0].Aciertos+= 1;
                }
                else
                {
                    if (!showerror && !waitreading)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerError);
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedor", 2f, new object[] { container }));
                        penalizacion += 5;
                        GameManager.Instance.player.Score -= 5;
                        GameManager.Instance.player.Data[0].Errors += 1;
                    }

                }
            }

        }
        else
        {
            if (!showerror && !waitreading)
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { container }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
                GameManager.Instance.player.Data[0].Errors += 1;
            }

        }

    }

    public override void OnExistPickingScene()
    {
        state = StateGame.ScannerContainer;
        if (currentTask is PickingTask picking)
        {
            rfcontroller.SetPantallaTxt("EnterContainer", new object[] { picking.Stock, picking.Container,
                        currentTask.parentOrder.ContainerClient, picking.Quantity});
        }
    }

    public override bool CheckContainerPicking(string container)
    {
        if (currentTask is PickingTask picking)
        {
            if(picking.Container != container)
            {
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("containerpickingerror", 2f, new object[] { container }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
                GameManager.Instance.player.Data[0].Errors += 1;
                return false;
            }
            else
            {
                return true;
            }
        }
        return false;
    }

    public override void onErrorContainerClient() 
    {
        showerror = true;
        infotext.SetActiveInfo(true);
        StartCoroutine(infotext.SetMessageKey("errorcontainerclient", 2f, new object[] { }));
        penalizacion += 5;
        GameManager.Instance.player.Data[0].Errors += 1;
    }

    public override bool CheckPicking(int cantplatano, int cantuvas, int cantpi�a, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        // check
        if (currentTask is PickingTask picking)
        {
            var total = cantfresa + cantplatano + cantperas + cantmelocoton + cantmanzana + cantuvas + cantpi�a;
            if (total == picking.Quantity)
            {
                if ((picking.Stock == "pi�a" && cantpi�a != total) ||
                     (picking.Stock == "melocton" && cantmelocoton != total) ||
                     (picking.Stock == "platano" && cantplatano != total) ||
                     (picking.Stock == "fresa" && cantfresa != total) ||
                     (picking.Stock == "peras" && cantperas != total) ||
                     (picking.Stock == "manzanas" && cantmanzana != total) ||
                     (picking.Stock == "uvas" && cantuvas != total))
                {
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("errorpickingproduct", 2f, new object[] { total, picking.Stock }));
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;
                    GameManager.Instance.player.Data[0].Errors += 1;
                    return false;
                }
                else
                {
                    infotext.SetActiveInfo(true);
                    setLockPlayer(true);
                    NextStep();
                    // Picking correcto
                    bonificacion += 10;
                    GameManager.Instance.player.Score += 10;
                    GameManager.Instance.player.Data[0].Aciertos += 1;
                    return true;
                }
            }
            else
            {
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errorpickingquantity", 2f, new object[] { total, picking.Quantity }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
                GameManager.Instance.player.Data[0].Errors += 1;
                return false;
            }
        }
        return false;
    }

    public override void onResetTask()
    {
        if (currentTask is PickingTask picking)
        {
            penalizacion += 5;
            GameManager.Instance.player.Score -= 5;
            GameManager.Instance.player.Data[0].Errors += 1;
            var container1 = currentTask.parentOrder.ContainerClient != null ? currentTask.parentOrder.ContainerClient : string.Empty;
            var container2 = string.Empty;
            var container3 = string.Empty;
            if ((game as GamePicking).Orders.Count > 1)
                container2 = (game as GamePicking).Orders[1].ContainerClient != null ? (game as GamePicking).Orders[1].ContainerClient : string.Empty;
            if ((game as GamePicking).Orders.Count > 2)
                container3 = (game as GamePicking).Orders[2].ContainerClient != null ? (game as GamePicking).Orders[1].ContainerClient : string.Empty;

            setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, container1,container2,container3, currentTask.parentOrder.Level);
        }
    }

    public override void onErrorPicking()
    {
        showerror = true;
        infotext.SetActiveInfo(true);
        StartCoroutine(infotext.SetMessageKey("errorpickingotherclient", 2f, new object[] { }));
        penalizacion += 5;
        GameManager.Instance.player.Data[0].Errors += 1;  
    }


    #endregion

    #region Private Methods
    
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
            case StateGame.ShowTutorial2:
            case StateGame.ShowTutorial3:
            case StateGame.ShowMsgIA:
            {
                    if (state != StateGame.ShowTutorial1)
                    {
                        showhelp = false;
                    }
                    if (currentTask != null)
                    {
                        rfcontroller.SetPantallaTxt("Picking", new object[] { currentTask.parentOrder.Name });
                    }
                    setLockPlayer(true);
                    state = StateGame.ShowClientContainer;
                }
                break;
            case StateGame.ShowClientContainer:
                {
                    setLockPlayer(true);
                    state = StateGame.ShowScannerContainer;
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
                    if (tutorial)
                    {
                        for (int i = 0; i < clientsPallets.Length; i++)
                        {
                            clientsPallets[i].SetSelected(true);
                        }
                    }
                }
                break;
            case StateGame.ShowLocationPicking:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerLocation;
                    if (tutorial)
                    {
                        currentTask.LocationRef.SetSelectLevel(currentTask.Location);
                    }
                }
                break;
            case StateGame.ShowContainerPicking:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerContainer;
                    if (tutorial)
                    {
                        currentTask.ContainerRef.SetSelected(true);
                    }
                    currentTask.LocationRef.UnSelectionShelf();

                }
                break;
            case StateGame.ShowIntroducirArticulo:
                {
                    setLockPlayer(true);
                    infotext.SetActiveInfo(false);
                    timer.SetTimerOn(true);
                    state = StateGame.PickingQuantity;
                }
                break;
            case StateGame.PickingQuantity:
                {
                    setLockPlayer(false);
                    NexTask();
                    break;
                }
            case StateGame.ShowDockConfirmation:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerDock;
                    rfcontroller.SetPantallaTxt("NoMasTareas", new object[] {currentTask.parentOrder.Name, currentTask.parentOrder.Dock, currentTask.parentOrder.ContainerClient });              
                    break;
                }
            case StateGame.ShowFinishLevel:
                {
                    setLockPlayer(true);
                    state = StateGame.FinishLevel; 
                    GameManager.Instance.player.Data[0].TotalTime += timer.TimeLeft;
                    this.setFinishLevel();
                    break;
                }
        }
    }

    private void NexTask()
    {
        infotext.SetActiveInfo(false);
        currentTask.ContainerRef.SetSelected(false);
        if (!GetTask())
        {
            state = StateGame.ShowDockConfirmation;
        }
    }

    private bool GetTask()
    {
        if (tasks.Count > 0)
        {
            currentTask = tasks.Dequeue();
            state = StateGame.ScannerLocation;
            if (tutorial)
            {
                currentTask.ContainerRef.SetSelected(true);
            }
            if (currentTask is PickingTask picking)
            {
                if (currentTask.parentOrder.ContainerClient == null)
                {
                    state = StateGame.ScannerContainerClient;
                    rfcontroller.SetPantallaTxt("Picking", new object[] { currentTask.parentOrder.Name });
                }
                else
                {
                    rfcontroller.SetPantallaTxt("EnterLocation", new object[] { picking.Location, currentTask.parentOrder.Name,
                                picking.Stock, currentTask.parentOrder.ContainerClient});
                }
            }
            return true;
        }
        return false;
    }

    #endregion
    internal enum StateGame
    {
        ShowBienVenido,
        ShowTutorial1,
        ShowTutorial2,
        ShowTutorial3,
        ShowClientContainer,
        ShowIntroducirContainerCliente,
        ShowScannerContainer,
        ScannerContainerClient,
        ShowLocationPicking,
        ShowContainerPicking,
        ScannerLocation,
        ScannerContainer,
        ShowIntroducirArticulo,
        PickingQuantity,
        ShowDockConfirmation,
        ScannerDock,
        WaitingReading,
        ShowFinishLevel,
        FinishLevel,
        WaitingIAReto,
        ShowMsgIA,
            
    }


}

 