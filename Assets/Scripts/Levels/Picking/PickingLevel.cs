using Assets.Scripts.Helper;
using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

public class PickingLevel : Level
{

    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private pallet[] clientsPallets;
    [SerializeField] private bool variospedidos;
    public IARetosPicking retoia= new IARetosPicking();
    public IAResultPicking resultia;
    public IARetosPickingAyuda retoayuda;

    private Task currentTask;
    private Queue<Task> tasks;
    private StateGame state;
    private StateGame stateHelp;
    private bool isFirtsAI = false;
    private bool AIHelp = false;
        
    #region Public Methods  

    public void Start()
    {
        InitLevel();

        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }

        if (!GameManager.Instance.UsedIA)
        {
            // El nivel 1 siempre es tutorial del juego
            switch (numberlevel)
            {
                case 1:
                    game = new GamePicking(warehousemanual, 1, 1, OrderType.Picking, "Tutorial Picking", 10, false);
                    state = StateGame.ShowBienVenido;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(300f);
                    }
                    break;
                case 2:
                    game = new GamePicking(warehousemanual, 1, 10, OrderType.Picking, "Preparar un pedido", 6, false);
                    showhelp = true;
                    state = StateGame.ShowTutorial2;
                    if (timer != null)
                    {
                        timer.SetTimeLeft(900f);
                    }
                    break;
                case 3:
                    game = new GamePicking(warehousemanual, 3, 8, OrderType.Picking, "Multi pedidos", 3, true);
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
        string prompt = $"El jugador se dispone a realizar los retos de preparación de pedidos. Su nivel en la categoria de Preparación de Pedidos es {GameManager.Instance.player.playerClassification.GetLevel4Category("PreparacionPedidos")}." +
            $"Necesito un reto para su nivel para poder avanzar y alcanzar el nivel experto." +
            $"La estructura de la respuesta es\r\n{{\r\n  \"Ordenes\": valor,     //Indica el Número de órdenes a realizar. Valor entre 1 a 3.   \t \r\n  \"Tareas\": valor,        //indica el Número de tareas por cada reto. Valor puede ser entre 1 y 20. \t\r\n  \"Multireferencia\": valor, \t//es true (sí) o false (no), según incluyan contenedores multireferencia. Si es True mas dificil es el reto, solo adapto para el nivel avanzado o experto.\r\n  \"Nivel\": valor, \t//es el Nivel de dificultad. Puede ser Principiante, Medio, Avanzado, Experto según dificultad del reto.\t\t\r\n  \"Tiempo\": valor, \t//es el Tiempo en minutos que debería el jugador de completar el reto en función del nivel, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos.\r\n  \"Fallos\": valor, \t// es un entero que indica el Número máximo de fallos permitidos por cada reto. Cuando mas fallos menor dificultad.\r\n  \"Explicacion\": valor // Cadena de texto con la explicación para mostrarsela al jugador del reto a realizar.\r\n   \"Ayuda\": valor // Indica en función del nivel de dificultad si se debe mostrar ayuda. Valor True o False. \"MiniMapa\": valor // Indica si en función del nivel de dificulta y nivel del jugador se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False}}\r\nA mayor número de ordenes, tareas multireferencias y menor fallos mayor nivel de dificultad." +
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
            case StateGame.AdjustIAChallenge:
            case StateGame.AdjustIALevelInLocation:
            case StateGame.AdjustIALevelInContainerClient:
                {
                    showMsg(retoayuda.Explicacion);
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
                                game = new GamePicking(warehousemanual, retoia.Ordenes, numtareas, OrderType.Picking, $"Reto IA:{retoia.Nivel}", retoia.Fallos * retoia.Ordenes, retoia.Multireferencia);
                                if (timer != null)
                                {
                                    timer.SetTimeLeft(retoia.Tiempo * 60);
                                }
                               
                                GameManager.Instance.WriteLog($"Iniciar game: {game.Name} - Nivel {retoia.Nivel}");
                                tasks = new Queue<Task>();
                                (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
                                currentTask = tasks.Dequeue();
                                if (txtNivel != null)
                                {
                                    txtNivel.SetPantallaTxt("level1", new object[] { });
                                }
                                GameManager.Instance.showminimap = retoia.MiniMapa;
                                showhelp = AIHelp = retoia.Ayuda;
                                state = StateGame.AdjustIAChallenge;
                                setMiniMap();
                            }
                        }
                    }
                    break;
                }
            case StateGame.WaitingIAHelpLocation:
                {
                    if (!GameManager.Instance.wait4IAResponse)
                    {
                        retoayuda = JsonConvert.DeserializeObject<IARetosPickingAyuda>(GameManager.Instance.IAResponse);
                        if (retoayuda != null)
                        {
                            switch (retoayuda.OpcionAyuda.ToUpper())
                            {
                                case "A":
                                    {
                                        // Solamente es añadir mas ayuda al jugador
                                        GameManager.Instance.showminimap = retoayuda.MiniMapa;
                                        AIHelp = retoayuda.Ayuda;
                                        state = stateHelp;
                                        setLockPlayer(true);
                                        setMiniMap();
                                        break;
                                    }
                                case "B":
                                    {
                                        // Caso B. En este caso la IA acaba de ajustar el reto. Creamos un nuevo reto
                                        GameManager.Instance.WriteLog($"***************************");
                                        int numtareas = retoayuda.Tareas / retoayuda.Ordenes;
                                        game = new GamePicking(warehousemanual, retoayuda.Ordenes, numtareas, OrderType.Picking, $"Reto IA:{retoayuda.Nivel}", retoayuda.Fallos * retoayuda.Ordenes, retoayuda.Multireferencia);
                                        if (timer != null)
                                        {
                                            timer.SetTimeLeft(retoayuda.Tiempo * 60);
                                        }
                         
                                        GameManager.Instance.WriteLog($"Iniciar Reajustado game: {game.Name} - Nivel {retoayuda.Nivel}");
                                        tasks = new Queue<Task>();
                                        (game as GamePicking).AllTask.ForEach(task => tasks.Enqueue(task));
                                        currentTask = tasks.Dequeue();
                                        if (txtNivel != null)
                                        {
                                            txtNivel.SetPantallaTxt("level1", new object[] { });
                                        }
                                        GameManager.Instance.showminimap = retoayuda.MiniMapa;
                                        showhelp = AIHelp = retoayuda.Ayuda;
                                        state = StateGame.AdjustIAChallenge;
                                        setLockPlayer(true);
                                        ResetErrors();
                                        setMiniMap();
                                        currentTask.LocationRef.UnSelectionShelf();
                                        for (int i = 0; i < clientsPallets.Length; i++)
                                        {
                                                clientsPallets[i].gameObject.SetActive(true);
                                        }
                                        break;
                                    }
                               
                            }
                        }
                       
                    }
                    break;
                }
            case StateGame.WaitingIAHelpContainerClient:
                {
                    if (!GameManager.Instance.wait4IAResponse)
                    {
                        retoayuda = JsonConvert.DeserializeObject<IARetosPickingAyuda>(GameManager.Instance.IAResponse);
                        if (retoayuda != null)
                        {
                            switch (retoayuda.OpcionAyuda.ToUpper())
                            {
                                case "A":
                                    {
                                        // Solamente es añadir mas ayuda al jugador
                                        GameManager.Instance.showminimap = retoayuda.MiniMapa;
                                        setMiniMap();
                                        AIHelp = retoayuda.Ayuda;
                                        state = stateHelp;
                                        setLockPlayer(true);
                                        break;
                                    }
                                default:
                                    state = stateHelp;
                                    break;
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
                            game = new GamePicking(warehousemanual, retoia.Ordenes, numtareas, OrderType.Picking, $"Reto IA:{retoia.Nivel}", retoia.Fallos * retoia.Ordenes, retoia.Multireferencia);
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
                            AIHelp = tutorial = retoia.Ayuda;
                            state = StateGame.ShowMsgIA;
                            if (isFirtsAI)
                            {
                                showhelp = true;
                                isFirtsAI = false;
                            }
                            else
                            {
                                showhelp = false;
                            }
                            ResetErrors();
                            setMiniMap();
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
                            $"{{\r\n  \"SuperarReto\": valor,     //Indica que ha superado el reto o no. Valor true o false.   \t \r\n  \"Explicacion\": valor, \t// Cadena de texto con la explicación para mostrarsela al jugador del resultado del reto. Indicando si lo ha superado o lo tiene que repetir.\r\n  \"AjustarReto\": valor, \t// Indica si se debe ajustar o no la dificulta del reto que ha realizado, en caso de no superarlo. Valor true o false.\r\n  \"Ordenes\": valor,        // Número de órdenes a realizar en caso de ajustar el reto. Valor entre 1 a 3. O cero si no es necesario ajustarlo.\r\n  \"Tareas\": valor,         // Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\r\n  \"Multireferencia\": valor // true (sí) o false (no), según incluyan contenedores multireferencia en caso de ajustar el reto. Si es True mas dificil es el reto, solo adapto para el nivel avanzado o experto." +
                            $"\r\n  \"Tiempo\": valor \t\t\t// Tiempo en minutos que debería el jugador de completar el reto en función del nivel en caso de ajustar el reto, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos." +
                            $"\r\n  \"Fallos\" valor \t\t\t// Número de fallos permitidos en el reto en caso de ajustar el reto. Cuando menor el valor mayor dificultad por lo que para los niveles menos avanzados no puede ser muy pequeño.\r\n  \"Ayuda\": valor // Indica en función del nivel de dificultad y nivel del jguador si se debe mostrar ayuda. Valor True o False.\r\n  \"MiniMapa\": valor // Indica si en función del nivel de dificulta y nivel del jugador se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\r\n }}" +
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
                            AddPenalty(5);
                            if (!GameManager.Instance.UsedIA)
                            {
                                showerror = true;
                                infotext.SetActiveInfo(true);
                                StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { location }));
                            }
                            else
                            {
                                // Informamos a la IA. Del fallo
                                string prompt = $"El jugador acaba de cometer un error al escanear la ubicación donde se encuentra el contenedor. La etiqueta leida {location} es incorrecta. " +
                                    $"Quiero que analices si es necesario ayudar al jugador. La ayuda que les puedes proporcionar es de tres tipos: A) Añadir más ayuda al juego. En este caso se podría activar el minimapa y marcar la ubicación para que se oriente mejor. " +
                                    $"B) Además de lo anterior, si el jugador ha cometido muchos fallos reducir la dificulta del reto. En este caso deberas de ajustar la dificulta el reto a nivel menor. " +
                                    $"Por ejemplo si estaba realizando un reto para el nivel medio el nuevo reto sería para el nivel principante. En caso de tener un nivel principante se podría reducir las tareas a realizar y aumentar la ayuda.\r\n" +
                                    $" C) No hacer nada, ya que el número de fallos no es suficientemente elevador para el reto a realizar versus el nivel del jugador. " +
                                    $"A continuación te proporcionaré los datos el reto que esta realizando el jugador.  El jugador tenia que realizar el siguiente reto: \"Ordenes\": {retoia.Ordenes}, \"Tareas\": {retoia.Tareas}, " +
                                    $"\"Multireferencia\": {retoia.Multireferencia}, \"Nivel\": \"{retoia.Nivel}\", \"Tiempo\": {retoia.Tiempo}, \"Fallos\": {retoia.Fallos * retoia.Ordenes}, \"Ayuda\": {retoia.Ayuda}, " +
                                    $"\"MiniMapa\": {retoia.MiniMapa}. Hasta estos son sus Fallos y Aciertos. Fallos: {GameManager.Instance.player.Data[currentData].Errors} " +
                                    $"- Aciertos: {GameManager.Instance.player.Data[currentData].Aciertos}. La estructura la respuesta debe ser la siguiente:\r\n" +
                                    $" {{\\r\\n  \"OpcionAyuda\": valor,     //Indica la opción de ayuda. Puede ser A,B,C.   \\t \\r\\n  " +
                                    $" \"Explicacion\": valor, \\t// Cadena de texto con la explicación con la ayuda a mostrar al jugador en caso de A y B.\\r\\n " +
                                    $" \"AjustarReto\": valor, \\t// Indica si se debe ajustar o no la dificulta del reto que ha realizado, en caso de la opcion B. Valor true o false.\\r\\n " +
                                    $" \"Ordenes\": valor, \\t// Número de órdenes a realizar en caso de ajustar el reto. Valor entre 1 a 3. O cero si no es necesario ajustarlo.\\r\\n " +
                                    $" \"Tareas\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                                    $" \"Multireferencia\": valor, \\t// true (sí) o false (no), según incluyan contenedores multireferencia en caso de ajustar el reto. Si es True mas dificil es el reto, solo adapto para el nivel experto o avanzado\\r\\n " +
                                    $" \"Tiempo\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                                    $" \"Tareas\": valor, \\t// Tiempo en minutos que debería el jugador de completar el reto en función del nivel en caso de ajustar el reto, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos.\\r\\n " +
                                    $" \"Fallos\": valor, \\t// Número de fallos permitidos en el reto en caso de ajustar el reto. Cuando menor el valor mayor dificultad por lo que para los niveles menos avanzados no puede ser muy pequeño.\\r\\n " +
                                    $" \"Ayuda\": valor, \\t// En caso de opcion A o B indica si se debe mostrar la ayuda al jugador. Valor True o False.\\r\\n " +
                                    $" \"MiniMapa\": valor, \\t// En caso de A o B se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\\r\\n " +
                                    $" En caso de ajustar el reto los nuevos valores deben ser mas fáciles que el reto realizado por jugador. Respondeme solamente con la estructura indicada. La explicación debe estar en el idioma {GameManager.Instance.GetLanguage()}\r\n";

                                ;
                                setLockPlayer(true);
                                GameManager.Instance.SendIAMsg(prompt);
                                state = StateGame.WaitingIAHelpLocation;
                                stateHelp = StateGame.AdjustIALevelInLocation;

                            }

                        }

                    }
                }
            }
            else
            {
                if (!showerror && !waitreading)
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    AddPenalty(5);
                    if (!GameManager.Instance.UsedIA)
                    {
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirUbicacion", 2f, new object[] { location }));
                    }
                    else
                    {
                        // Informamos a la IA. Del fallo
                        string prompt = $"El jugador acaba de cometer un error al escanear la ubicación donde se encuentra el contenedor. La etiqueta leida {location} es incorrecta. " +
                            $"Quiero que analices si es necesario ayudar al jugador. La ayuda que les puedes proporcionar es de tres tipos: A) Añadir más ayuda al juego. En este caso se podría activar el minimapa y marcar la ubicación para que se oriente mejor. " +
                            $"B) Además de lo anterior, si el jugador ha cometido muchos fallos reducir la dificulta del reto. En este caso deberas de ajustar la dificulta el reto a nivel menor. " +
                            $"Por ejemplo si estaba realizando un reto para el nivel medio el nuevo reto sería para el nivel principante. En caso de tener un nivel principante se podría reducir las tareas a realizar y aumentar la ayuda.\r\n" +
                            $" C) No hacer nada, ya que le número de fallos no es suficientemente elevador para el reto a realizar versus el nivel del jugador. " +
                            $"A continuación te proporcionaré los datos el reto que esta realizando el jugador.  El jugador tenia que realizar el siguiente reto: \"Ordenes\": {retoia.Ordenes}, \"Tareas\": {retoia.Tareas}, " +
                            $"\"Multireferencia\": {retoia.Multireferencia}, \"Nivel\": \"{retoia.Nivel}\", \"Tiempo\": {retoia.Tiempo}, \"Fallos\": {retoia.Fallos * retoia.Ordenes}, \"Ayuda\": {retoia.Ayuda}, " +
                            $"\"MiniMapa\": {retoia.MiniMapa}. Hasta estos son sus Fallos y Aciertos. Fallos: {GameManager.Instance.player.Data[currentData].Errors} " +
                            $"- Aciertos: {GameManager.Instance.player.Data[currentData].Aciertos}. La estructura la respuesta debe ser la siguiente:\r\n" +
                            $" {{\\r\\n  \"OpcionAyuda\": valor,     //Indica la opción de ayuda. Puede ser A,B,C.   \\t \\r\\n  " +
                            $" \"Explicacion\": valor, \\t// Cadena de texto con la explicación con la ayuda a mostrar al jugador en caso de A y B.\\r\\n " +
                            $" \"AjustarReto\": valor, \\t// Indica si se debe ajustar o no la dificulta del reto que ha realizado, en caso de la opcion B. Valor true o false.\\r\\n " +
                            $" \"Ordenes\": valor, \\t// Número de órdenes a realizar en caso de ajustar el reto. Valor entre 1 a 3. O cero si no es necesario ajustarlo.\\r\\n " +
                            $" \"Tareas\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                            $" \"Multireferencia\": valor, \\t// true (sí) o false (no), según incluyan contenedores multireferencia en caso de ajustar el reto. Si es True mas dificil es el reto, solo adapto para el nivel experto o avanzado\\r\\n " +
                            $" \"Tiempo\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                            $" \"Tareas\": valor, \\t// Tiempo en minutos que debería el jugador de completar el reto en función del nivel en caso de ajustar el reto, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos.\\r\\n " +
                            $" \"Fallos\": valor, \\t// Número de fallos permitidos en el reto en caso de ajustar el reto. Cuando menor el valor mayor dificultad por lo que para los niveles menos avanzados no puede ser muy pequeño.\\r\\n " +
                            $" \"Ayuda\": valor, \\t// En caso de opcion A o B indica si se debe mostrar la ayuda al jugador. Valor True o False.\\r\\n " +
                            $" \"MiniMapa\": valor, \\t// En caso de A o B se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\\r\\n " +
                            $" En caso de ajustar el reto los nuevos valores deben ser mas fáciles que el reto realizado por jugador. Respondeme solamente con la estructura indicada. La explicación debe estar en el idioma {GameManager.Instance.GetLanguage()}\r\n";

                        ;
                        setLockPlayer(true);
                        GameManager.Instance.SendIAMsg(prompt);
                        state = StateGame.WaitingIAHelpLocation;
                        stateHelp = StateGame.AdjustIALevelInLocation;
                    }

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
                    AddPenalty(5);
                    if (!GameManager.Instance.UsedIA)
                    {
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContainerCliente", 2f, new object[] { container }));
                    }
                    else
                    {
                        // Informamos a la IA. Del fallo
                        string prompt = $"El jugador acaba de cometer un error al escanear el contenedor cliente, primer paso para la preparación de pedidos. El contenedor leido: {container}, es incorrecto. " +
                            $"Quiero que analices si es necesario ayudar al jugador. La ayuda que les puedes proporcionar son dos tipos: A) Añadir más ayuda al juego. En este caso se podría activar el minimapa y marcar la ubicación para que se oriente mejor. " +
                            $" B) No hacer nada, ya que le número de fallos no es suficientemente elevado para el reto a realizar versus el nivel del jugador. " +
                            $"A continuación te proporcionaré los datos el reto que esta realizando el jugador.  El jugador tenia que realizar el siguiente reto: \"Ordenes\": {retoia.Ordenes}, \"Tareas\": {retoia.Tareas}, " +
                            $"\"Multireferencia\": {retoia.Multireferencia}, \"Nivel\": \"{retoia.Nivel}\", \"Tiempo\": {retoia.Tiempo}, \"Fallos\": {retoia.Fallos * retoia.Ordenes}, \"Ayuda\": {retoia.Ayuda}, " +
                            $"\"MiniMapa\": {retoia.MiniMapa}. Hasta estos son sus Fallos y Aciertos. Fallos: {GameManager.Instance.player.Data[currentData].Errors} " +
                            $"- Aciertos: {GameManager.Instance.player.Data[currentData].Aciertos}. La estructura la respuesta debe ser la siguiente:\r\n" +
                            $" {{\\r\\n  \"OpcionAyuda\": valor,     //Indica la opción de ayuda. Puede ser A,B.   \\t \\r\\n  " +
                            $" \"Explicacion\": valor, \\t// Cadena de texto con la explicación con la ayuda a mostrar al jugador en caso de A y B.\\r\\n " +
                            $" \"Ayuda\": valor, \\t// En caso de opcion A  indica si se debe mostrar la ayuda al jugador. Valor True o False.\\r\\n " +
                            $" \"MiniMapa\": valor, \\t// En caso de A se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\\r\\n " +
                            $"  Respondeme solamente con la estructura indicada, ajusta la explicación al número de fallos. La explicación debe estar en el idioma {GameManager.Instance.GetLanguage()}\r\n";

                        ;
                        setLockPlayer(true);
                        GameManager.Instance.SendIAMsg(prompt);
                        state = StateGame.WaitingIAHelpContainerClient;
                        stateHelp = StateGame.AdjustIALevelInContainerClient;
                    }
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
                    setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, container1, container2, container3, currentTask.parentOrder.Level, picking.isMulti ? picking.Quantity : 12);
                    AddBonificacion(5);
                }
                else
                {
                    if (!showerror && !waitreading)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerError);
                        AddPenalty(5);
                        if (!GameManager.Instance.UsedIA)
                        {
                            showerror = true;
                            infotext.SetActiveInfo(true);
                            StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedor", 2f, new object[] { container }));
                        }
                        else
                        {
                            // Informamos a la IA. Del fallo
                            string prompt = $"El jugador acaba de cometer un error al escanear el contenedor donde se encuentra el producto a picar. La etiqueta leida {container} es incorrecta. " +
                                $"Quiero que analices si es necesario ayudar al jugador. La ayuda que les puedes proporcionar es de tres tipos: A) Añadir más ayuda al juego. En este caso se podría activar el minimapa y marcar la ubicación para que se oriente mejor. " +
                                $"B) Además de lo anterior, si el jugador ha cometido muchos fallos reducir la dificulta del reto. En este caso deberas de ajustar la dificulta el reto a nivel menor. " +
                                $"Por ejemplo si estaba realizando un reto para el nivel medio el nuevo reto sería para el nivel principante. En caso de tener un nivel principante se podría reducir las tareas a realizar y aumentar la ayuda.\r\n" +
                                $" C) No hacer nada, ya que le número de fallos no es suficientemente elevador para el reto a realizar versus el nivel del jugador. " +
                                $"A continuación te proporcionaré los datos el reto que esta realizando el jugador.  El jugador tenia que realizar el siguiente reto: \"Ordenes\": {retoia.Ordenes}, \"Tareas\": {retoia.Tareas}, " +
                                $"\"Multireferencia\": {retoia.Multireferencia}, \"Nivel\": \"{retoia.Nivel}\", \"Tiempo\": {retoia.Tiempo}, \"Fallos\": {retoia.Fallos * retoia.Ordenes}, \"Ayuda\": {retoia.Ayuda}, " +
                                $"\"MiniMapa\": {retoia.MiniMapa}. Hasta estos son sus Fallos y Aciertos. Fallos: {GameManager.Instance.player.Data[currentData].Errors} " +
                                $"- Aciertos: {GameManager.Instance.player.Data[currentData].Aciertos}. La estructura la respuesta debe ser la siguiente:\r\n" +
                                $" {{\\r\\n  \"OpcionAyuda\": valor,     //Indica la opción de ayuda. Puede ser A,B,C.   \\t \\r\\n  " +
                                $" \"Explicacion\": valor, \\t// Cadena de texto con la explicación con la ayuda a mostrar al jugador en caso de A y B.\\r\\n " +
                                $" \"AjustarReto\": valor, \\t// Indica si se debe ajustar o no la dificulta del reto que ha realizado, en caso de la opcion B. Valor true o false.\\r\\n " +
                                $" \"Ordenes\": valor, \\t// Número de órdenes a realizar en caso de ajustar el reto. Valor entre 1 a 3. O cero si no es necesario ajustarlo.\\r\\n " +
                                $" \"Tareas\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                                $" \"Multireferencia\": valor, \\t// true (sí) o false (no), según incluyan contenedores multireferencia en caso de ajustar el reto. Si es True mas dificil es el reto, solo adapto para el nivel experto o avanzado\\r\\n " +
                                $" \"Tiempo\": valor, \\t// Número de tareas por orden en caso de ajustar el reto. Valor puede ser entre 1 y 20.\\r\\n " +
                                $" \"Tareas\": valor, \\t// Tiempo en minutos que debería el jugador de completar el reto en función del nivel en caso de ajustar el reto, ordenes y número de tareas. Tiempo máximo para un reto de nivel experto 30 minutos.\\r\\n " +
                                $" \"Fallos\": valor, \\t// Número de fallos permitidos en el reto en caso de ajustar el reto. Cuando menor el valor mayor dificultad por lo que para los niveles menos avanzados no puede ser muy pequeño.\\r\\n " +
                                $" \"Ayuda\": valor, \\t// En caso de opcion A o B indica si se debe mostrar la ayuda al jugador. Valor True o False.\\r\\n " +
                                $" \"MiniMapa\": valor, \\t// En caso de A o B se debe mostrar o no un minimapa del almacén para que el jugador se pueda orientar. Valor True o False\\r\\n " +
                                $" En caso de ajustar el reto los nuevos valores deben ser mas fáciles que el reto realizado por jugador, y la explicación al número de fallos. Respondeme solamente con la estructura indicada. La explicación debe estar en el idioma {GameManager.Instance.GetLanguage()}\r\n";

                            setLockPlayer(true);
                            GameManager.Instance.SendIAMsg(prompt);
                            state = StateGame.WaitingIAHelpLocation;
                            stateHelp = StateGame.AdjustIALevelInContainer;

                        }
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
                    onResetTask();
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
                onResetTask();
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

            setPickingLocation(picking.Stock, picking.Container, picking.LocationRef, container1, container2, container3, currentTask.parentOrder.Level, picking.isMulti ? picking.Quantity : 12);
        }
    }

    public override void onErrorPicking()
    {
        showerror = true;
        infotext.SetActiveInfo(true);
        StartCoroutine(infotext.SetMessageKey("errorpickingotherclient", 2f, new object[] { }));
        onResetTask();

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
            case StateGame.AdjustIAChallenge:
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
            case StateGame.AdjustIALevelInContainerClient:
            case StateGame.ShowScannerContainer:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerContainerClient;
                    if (tutorial || AIHelp)
                    {
                        for (int i = 0; i < clientsPallets.Length; i++)
                        {
                            clientsPallets[i].SetSelected(true);
                        }
                    }
                }
                break;
            case StateGame.ShowLocationPicking:
            case StateGame.AdjustIALevelInLocation:
                {
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.ScannerLocation;
                    if (tutorial || AIHelp)
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
                    if (tutorial || AIHelp)
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
            if (tutorial || AIHelp)
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
        WaitingIAHelpLocation,
        WaitingIAHelpContainer,
        WaitingIAHelpContainerClient,
        WaitingIAHelpDock,
        FailIAResult,
        AdjustIAChallenge,
        AdjustIALevelInLocation,
        AdjustIALevelInContainer,
        AdjustIALevelInContainerClient,
        AdjustIALevelInDock,
        ShowMsgIA,

    }


}

