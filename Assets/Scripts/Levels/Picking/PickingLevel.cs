using Assets.Scripts.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class PickingLevel : Level
{

    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private pallet[] clientsPallets;
    [SerializeField] private bool variospedidos;
    public IARetosPicking retoia;
    public IAResultPicking resultia;

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
                    game = new GamePicking(warehousemanual, 1, 1, OrderType.Picking, "Tutorial Picking", 10);
                    state = StateGame.ShowBienVenido;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(300f);
                    }
                    break;
                case 2:
                    game = new GamePicking(warehousemanual, 1, 10, OrderType.Picking, "Preparar un pedido", 6);
                    showhelp = true;
                    state = StateGame.ShowTutorial2;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(900f);
                    }
                    break;
                case 3:
                    game = new GamePicking(warehousemanual, 3, 8, OrderType.Picking, "Multi pedidos", 3);
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
            NextLevelIA();
        }

    }

    private void NextLevelIA()
    {
        // Le pedimos a la IA que nos de el siguiente reto a realizar
        string prompt = $"El jugador se dispone a realizar los retos de preparación de pedidos. Su nivel general es {GameManager.Instance.player.playerClassification.GetLevel4Category("General")}." +
            $"Y el nivel en la categoria de Preparación de Pedidos es {GameManager.Instance.player.playerClassification.GetLevel4Category("PreparacionPedidos")}." +
            $"Sabiendo cual es su nivel necesito que me indiques cual seria el siguiente a realizar para alcanzar el siguiente nivel." +
            $"La estructura de la respuesta es\r\n{{\r\n  \"Ordenes\": valor,     //Indica el Número de órdenes a realizar. Valor entre 1 a 3.   \t \r\n  \"Tareas\": valor,        //indica el Número de tareas. Valor puede ser entre 1 y 20. \t\r\n  \"Multireferencia\": valor, \t//es true (sí) o false (no), según incluyan contenedores multireferencia. Si es True mas dificil es el reto, solo adapto para el nivel experto.\r\n  \"Nivel\": valor, \t//es el Nivel de dificultad. Puede ser Principiante, Medio, Avanzado, Experto según dificultad del reto.\t\t\r\n  \"Tiempo\": valor, \t//es el Tiempo en minutos que debería el jugador de completar el reto en función del nivel, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos.\r\n  \"Fallos\": valor, \t// es un entero que indica el Número máximo de fallos permitidos. Cuando mas fallos menor dificultad.\r\n  \"Explicacion\": valor // Cadena de texto con la explicación para mostrarsela al jugador del reto a realizar.\r\n   \"Ayuda\": valor // Indica en función del nivel de dificultad si se debe mostrar ayuda. Valor True o False. \"MiniMapa\": valor // Indica si en función del nivel de dificulta y nivel del jugador se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False}}\r\nA mayor número de ordenes, tareas multireferencias y menor fallos mayor nivel de dificultad." +
            $" \r\nNecesito un reto para el siguiente nivel que el jugador necesite para llegar alcanzar el nivel experto (el cual seria el ultimo reto)," +
            $"\r\nRespondeme solamente con la estructura indicada para el primer reto. La explicación del reto debe estar el idioma: {GameManager.Instance.GetLanguage()}";
        GameManager.Instance.SendIAMsg(prompt);
        state = StateGame.WaitingIAReto;

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
            case StateGame.FailIAResult:
                {
                    showMsg(resultia.Explicacion);
                    break;
                }
            case StateGame.WaitingIAResult:
                {
                    if (!GameManager.Instance.wait4IAResponse)
                    {
                        resultia = JsonConvert.DeserializeObject<IAResultPicking>(GameManager.Instance.IAResponse);
                        if (retoia != null)
                        {
                            GameManager.Instance.WriteLog($"Result AI Game {game.Name}: {retoia.ToString()} --> {GameManager.Instance.player.Data[currentData]}");
                            if (resultia.SuperarReto)
                            {
                                // reto superado. Subimos nivel jugador
                                GameManager.Instance.player.playerClassification.SetLevel4Category("PreparacionPedidos", retoia.Nivel);
                                if (GameManager.Instance.player.playerClassification.GetLevel4Category("PreparacionPedidos") == PlayerClassification.LevelCategory.Experto)
                                {
                                    state = StateGame.FinishLevel;
                                    this.setFinishLevel();
                                }
                                else
                                {
                                    // Siguiente nivel
                                    NextLevelIA();
                                }
                            }
                            else
                            {
                                GameManager.Instance.WriteLog($"***************************");
                                int numtareas = retoia.Tareas / retoia.Ordenes;
                                game = new GamePicking(warehousemanual, retoia.Ordenes, numtareas, OrderType.Picking, $"Reto IA:{retoia.Nivel}", retoia.Fallos);
                                if (timer != null)
                                {
                                    timer.SetTimeLeft(retoia.Tiempo * 60);
                                }
                                showhelp = false;
                                GameManager.Instance.WriteLog($"Iniciar game: {game.Name} - Nivel {retoia.Nivel}");
                                tasks = new Queue<Task>();
                                (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
                                currentTask = tasks.Dequeue();
                                if (txtNivel != null)
                                {
                                    txtNivel.SetPantallaTxt("level1", new object[] { });
                                }
                                GameManager.Instance.showminimap = retoia.MiniMapa;
                                tutorial = retoia.Ayuda;
                                state = StateGame.FailIAResult;
                            }
                        }
                    }
                    break;
                }
            case StateGame.WaitingIAReto:
                {
                    if (!GameManager.Instance.wait4IAResponse)
                    {
                        retoia = JsonConvert.DeserializeObject<IARetosPicking>(GameManager.Instance.IAResponse);
                        if (retoia != null)
                        {
                            GameManager.Instance.WriteLog($"***************************");
                            int numtareas = retoia.Tareas / retoia.Ordenes;
                            game = new GamePicking(warehousemanual, retoia.Ordenes, numtareas, OrderType.Picking, $"Reto IA:{retoia.Nivel}", retoia.Fallos);
                            if (timer != null)
                            {
                                timer.SetTimeLeft(retoia.Tiempo * 60);
                            }
                            showhelp = false;
                            GameManager.Instance.WriteLog($"Iniciar game: {game.Name} - Nivel {retoia.Nivel}");
                            tasks = new Queue<Task>();
                            (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
                            currentTask = tasks.Dequeue();
                            if (txtNivel != null)
                            {
                                txtNivel.SetPantallaTxt("level1", new object[] { });
                            }
                            GameManager.Instance.showminimap = retoia.MiniMapa;
                            tutorial = retoia.Ayuda;
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
                GameManager.Instance.player.Data[currentData].TotalTime += timer.TimeLeft;
                AddBonificacion(10);
                currentTask.parentOrder.ContainerClient = string.Empty;
                if (!GetTask())
                {
                    if (!GameManager.Instance.UsedIA || numberlevel == 1)
                    {
                        state = StateGame.FinishLevel;
                        this.setFinishLevel();
                    }
                    else
                    {
                        // Tenemos que analizar los resultados con la IA
                        string prompt = $"Analiza los resultados obtenidos por el jugador en el reto. El jugador tenia que realizar el siguiente reto: \"Ordenes\": {retoia.Ordenes}, \"Tareas\": {retoia.Tareas}, \"Multireferencia\": {retoia.Multireferencia}, \"Nivel\": \"{retoia.Nivel}\", \"Tiempo\": {retoia.Tiempo}, \"Fallos\": {retoia.Fallos}, \"Ayuda\": {retoia.Ayuda}, \"MiniMapa\": {retoia.MiniMapa} " +
                            $"Y los resultados han sido: Fallos: {GameManager.Instance.player.Data[currentData].Errors} - Aciertos: {GameManager.Instance.player.Data[currentData].Aciertos} - Tiempo: {GameManager.Instance.player.Data[currentData].TotalTime}\r\n " +
                            $".Sabiendo que es su nivel actual en la categoria de Preparación de Pedidos es {GameManager.Instance.player.playerClassification.GetLevel4Category("PreparacionPedidos")} necesito que me indiques si ha superado el reto. " +
                            $"La estructura de la respuesta es:" +
                            $"{{\r\n  \"SuperarReto\": valor,     //Indica que ha superado el reto o no. Valor true o false.   \t \r\n  \"Explicacion\": valor, \t// Cadena de texto con la explicación para mostrarsela al jugador del resultado del reto. Indicando si lo ha superado o lo tiene que repetir.\r\n  \"AjustarReto\": valor, \t// Indica si se debe ajustar o no la dificulta del reto que ha realizado, en caso de no superarlo. Valor true o false.\r\n  \"Ordenes\": valor,        // Número de órdenes a realizar en caso de ajustar el reto. Valor entre 1 a 3. O cero si no es necesario ajustarlo.\r\n  \"Tareas\": valor,         // Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\r\n  \"Multireferencia\": valor // true (sí) o false (no), según incluyan contenedores multireferencia en caso de ajustar el reto. Si es True mas dificil es el reto, solo adapto para el nivel experto." +
                            $"\r\n  \"Tiempo\": valor \t\t\t// Tiempo en minutos que debería el jugador de completar el reto en función del nivel en caso de ajustar el reto, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos." +
                            $"\r\n  \"Fallos\" valor \t\t\t// Número de fallos permitidos en el reto en caso de ajustar el reto. Cuando menor el valor mayor dificultad por lo que para los niveles menos avanzados no puede ser muy pequeño.\r\n}}  \"Ayuda\": valor // Indica en función del nivel de dificultad y nivel del jguador si se debe mostrar ayuda. Valor True o False.\r\n  \"MiniMapa\": valor // Indica si en función del nivel de dificulta y nivel del jugador se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\r\n" +
                            $"En caso de ajustar el reto los nuevos valores deben ser mas fáciles que el reto realizado por jugador. Respondeme solamente con la estructura indicada. \r\nLa explicación debe estar en el idioma {GameManager.Instance.GetLanguage()}";
                        ;

                        GameManager.Instance.SendIAMsg(prompt);
                        state = StateGame.WaitingIAResult;
                    }
                }

            }
            else
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
                AddPenalty(5);

            }
        }
        else if (!showerror && !waitreading)
        {
            SoundManager.SharedInstance.PlaySound(scannerError);
            showerror = true;
            infotext.SetActiveInfo(true);
            StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
            AddPenalty(5);
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
                        AddBonificacion(5);
                    }
                    else
                    {
                        if (!showerror && !waitreading)
                        {
                            SoundManager.SharedInstance.PlaySound(scannerError);
                            showerror = true;
                            infotext.SetActiveInfo(true);
                            StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { location }));
                            AddPenalty(5);

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
                    AddPenalty(5);

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
                AddPenalty(5);

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
                AddBonificacion(5);
            }
            else
            {
                if (!showerror && !waitreading)
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    showerror = true;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContainerCliente", 2f, new object[] { container }));
                    AddPenalty(5);

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
                    var container1 = string.Empty;
                    var container2 = string.Empty;
                    var container3 = string.Empty;
                    if ((game as GamePicking).Orders.Count >= 1)
                        container1 = (game as GamePicking).Orders[0].ContainerClient != null ? (game as GamePicking).Orders[0].ContainerClient : string.Empty;
                    if ((game as GamePicking).Orders.Count >= 2)
                        container2 = (game as GamePicking).Orders[1].ContainerClient != null ? (game as GamePicking).Orders[1].ContainerClient : string.Empty;
                    if ((game as GamePicking).Orders.Count >= 3)
                        container3 = (game as GamePicking).Orders[2].ContainerClient != null ? (game as GamePicking).Orders[2].ContainerClient : string.Empty;
                    setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, container1, container2, container3, currentTask.parentOrder.Level);
                    AddBonificacion(5);
                }
                else
                {
                    if (!showerror && !waitreading)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerError);
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedor", 2f, new object[] { container }));
                        AddPenalty(5);

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
                AddPenalty(5);

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
            if (picking.Container != container)
            {
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("containerpickingerror", 2f, new object[] { container }));
                AddPenalty(5);

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
        AddPenalty(5);

    }

    public override bool CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        // check
        if (currentTask is PickingTask picking)
        {
            var total = cantfresa + cantplatano + cantperas + cantmelocoton + cantmanzana + cantuvas + cantpiña;
            if (total == picking.Quantity)
            {
                if ((picking.Stock == "piña" && cantpiña != total) ||
                     (picking.Stock == "melocton" && cantmelocoton != total) ||
                     (picking.Stock == "platano" && cantplatano != total) ||
                     (picking.Stock == "fresa" && cantfresa != total) ||
                     (picking.Stock == "peras" && cantperas != total) ||
                     (picking.Stock == "manzanas" && cantmanzana != total) ||
                     (picking.Stock == "uvas" && cantuvas != total))
                {
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("errorpickingproduct", 2f, new object[] { total, picking.Stock }));
                    AddPenalty(5);

                    return false;
                }
                else
                {
                    infotext.SetActiveInfo(true);
                    setLockPlayer(true);
                    NextStep();
                    // Picking correcto
                    AddBonificacion(10);
                    return true;
                }
            }
            else
            {
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errorpickingquantity", 2f, new object[] { total, picking.Quantity }));
                AddPenalty(5);
                return false;
            }
        }
        return false;
    }

    public override void onResetTask()
    {
        if (currentTask is PickingTask picking)
        {
            AddPenalty(5);
            var container1 = string.Empty;
            var container2 = string.Empty;
            var container3 = string.Empty;
            if ((game as GamePicking).Orders.Count >= 1)
                container1 = (game as GamePicking).Orders[0].ContainerClient != null ? (game as GamePicking).Orders[0].ContainerClient : string.Empty;
            if ((game as GamePicking).Orders.Count >= 2)
                container2 = (game as GamePicking).Orders[1].ContainerClient != null ? (game as GamePicking).Orders[1].ContainerClient : string.Empty;
            if ((game as GamePicking).Orders.Count >= 3)
                container3 = (game as GamePicking).Orders[2].ContainerClient != null ? (game as GamePicking).Orders[2].ContainerClient : string.Empty;

            setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, container1, container2, container3, currentTask.parentOrder.Level);
        }
    }

    public override void onErrorPicking()
    {
        showerror = true;
        infotext.SetActiveInfo(true);
        StartCoroutine(infotext.SetMessageKey("errorpickingotherclient", 2f, new object[] { }));
        AddPenalty(5);

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
            case StateGame.FailIAResult:
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
                    rfcontroller.SetPantallaTxt("NoMasTareas", new object[] { currentTask.parentOrder.Name, currentTask.parentOrder.Dock, currentTask.parentOrder.ContainerClient });
                    break;
                }
            case StateGame.ShowFinishLevel:
                {
                    setLockPlayer(true);
                    state = StateGame.FinishLevel;
                    GameManager.Instance.player.Data[currentData].TotalTime += timer.TimeLeft;
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
        WaitingIAResult,
        FailIAResult,
        ShowMsgIA,

    }


}

