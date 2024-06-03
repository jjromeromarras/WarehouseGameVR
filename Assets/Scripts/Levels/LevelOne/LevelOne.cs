using Assets.Scripts.Helper;
using System.Linq;
using UnityEngine;

public class LevelOne : Level
{

    [SerializeField] private GameObject warehousemanual;
    [SerializeField] private pallet[] clientsPallets;
    [SerializeField] private bool tutorial;
    [SerializeField] private bool variospedidos;

    private int currentTask;
    private string currentContainerClient;

    private Game game;
    private Order order;
    private StateGame state;
    private bool waitreading;
    private bool showerror;
    private bool showhelp;

    #region Public Methods
    public void Awake()
    {
        currentTask = 0;
    }

    public void Start()
    {
        bonificacion = 0;
        penalizacion = 0;
        showhelp = tutorial ? GameManager.Instance.showayuda: false;
        
        waitreading = false;
        showerror = false;
    
        
        if (infotext != null)
        {
            infotext.onFinishInfoText += FinishInfoText;
            infotext.SetActiveInfo(true);
        }
        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }
        switch (numberlevel)
        {
            case 1:
                game = new Game(warehousemanual, 1 , 1 , 1, OrderType.Picking);
                state = StateGame.ShowBienVenido;
                if (timer != null)
                {
                    timer.SetTimeLeft(300f);
                }
                break;
            case 2:
                game = new Game(warehousemanual, 1, 15, 1, OrderType.Picking);
                showhelp = true;
                state = StateGame.ShowTutorial2;
                if (timer != null)
                {
                    timer.SetTimeLeft(900f);
                }
                break;   
        }
        order = game.Orders.FirstOrDefault();
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
                    showTexto("Reto1Tutorial1");
                    break;
                }
            case StateGame.ShowTutorial2:
                {
                    showTexto("Reto1Level2");
                    break;
                }
            case StateGame.ShowTutorial3:
                {
                    showTexto("Reto2Level3");
                    break;
                }
            case StateGame.ShowClientContainer:
                {
                    showTexto("ContainerCliente");
                }
                break;
            case StateGame.ShowIntroducirContainerCliente:
                {
                    setLockPlayer(true);
                    showTexto("IntroducirContainerCliente");
                }
                break;
            case StateGame.ShowScannerContainer:
                {
                    setLockPlayer(true);
                    showTexto("ScannerContainer");
                }
                break;
            case StateGame.ShowLocationPicking:
                {
                    setLockPlayer(true);
                    showTexto("IntroducirUbicacion");
                    break;
                }
            case StateGame.ShowContainerPicking:
                {
                    setLockPlayer(true);
                    showTexto("IntroducirContenedor");
                    break;
                }
            case StateGame.ShowIntroducirArticulo:
                {                    
                    showTexto("IntroducirArticulo");
                    break;
                }           
            case StateGame.ShowDockConfirmation:
                {
                    showTexto("confirmdock");
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

        }
    }
    public int OnSetDockScanner(string dock, string tag)
    {

        if (tag == "dock")
        {
            if (dock == order.Dock)
            {
                state = StateGame.FinishLevel;
                timer.SetTimerOn(false);
                GameManager.Instance.player.Data[0].TotalTime += timer.TimeLeft;
                bonificacion += 10;
                this.setFinishLevel();
                return 1;
            }
            else
            {
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
                penalizacion -= 5;
                return -5;
            }
        }
        else if (!showerror && !waitreading)
        {
            showerror = true;
            infotext.SetActiveInfo(true);
            StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { dock }));
            return -5;
        }
        return 0;

    }

    public override void OnSetLocationScanner(string location, string tag)
    {
        if (state == StateGame.ScannerLocation)
        {
            if (tag == "Ubicacion")
            {
                if (order.Tasks[currentTask] is PickingTask picking)
                {
                    if (location == picking.Location)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerOK);
                        picking.locationScan = true;
                        state = StateGame.ShowContainerPicking;
                        rfcontroller.SetPantallaTxt("EnterContainer", new object[] { picking.Stock, picking.Container,
                        currentContainerClient, picking.Quantity});
                        bonificacion += 5;
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
                        currentContainerClient = container;
                    }
                }
                if (order.Tasks[currentTask] is PickingTask picking)
                {
                    rfcontroller.SetPantallaTxt("EnterLocation", new object[] { picking.Location, order.Name,
                        picking.Stock, currentContainerClient});
                }
                bonificacion += 5;
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
                    GameManager.Instance.player.Data[0].Errors += 1;
                }

            }
        }
        else if (state == StateGame.ScannerContainer)
        {
            if (order.Tasks[currentTask] is PickingTask picking)
            {
                if (picking.Container == container)
                {
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    state = StateGame.ShowIntroducirArticulo;
                    rfcontroller.SetPantallaTxt("EnterArticulo", new object[] { picking.Stock, picking.Container,
                        currentContainerClient, picking.Quantity});
                    setPickingLocation(picking.Stock, picking.Container, picking.LocationRef);
                    bonificacion += 5;
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
                GameManager.Instance.player.Data[0].Errors += 1;
            }

        }

    }

    public override void OnExistPickingScene()
    {
        state = StateGame.ScannerContainer;
        if (order.Tasks[currentTask] is PickingTask picking)
        {
            rfcontroller.SetPantallaTxt("EnterContainer", new object[] { picking.Stock, picking.Container,
                        currentContainerClient, picking.Quantity});
        }
    }

    public override bool CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        // check
        if (order.Tasks[currentTask] is PickingTask picking)
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
                    penalizacion += 5;
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
                GameManager.Instance.player.Data[0].Errors += 1;
                return false;
            }
        }
        return false;
    }

    public override void onResetTask()
    {
        if (order.Tasks[currentTask] is PickingTask picking)
        {
            setPickingLocation(picking.Stock, picking.Container, picking.LocationRef);
        }
    }

   
    #endregion

    #region Private Methods
    private void showTexto(string key)
    {        
        if (!waitreading)
        {
            if (showhelp)
            {
                timer.SetTimerOn(false);
                infotext.SetActiveInfo(true);
                waitreading = true;
                StartCoroutine(infotext.SetMessageKey(key, 2f));
            }
            else
            {
                NextStep();
            }
        }
    }

    private void NextStep()
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
                    rfcontroller.SetPantallaTxt("Picking", new object[] { order.Name });
                    setLockPlayer(true);
                    state = StateGame.ShowClientContainer;
                }
                break;
            case StateGame.ShowTutorial2:
            case StateGame.ShowTutorial3:
                {
                    showhelp = false;
                    rfcontroller.SetPantallaTxt("Picking", new object[] { order.Name });
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
                        order.Tasks[currentTask].LocationRef.SetSelectLevel(order.Tasks[currentTask].Location);
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
                        order.Tasks[currentTask].ContainerRef.SetSelected(true);
                    }
                    order.Tasks[currentTask].LocationRef.UnSelectionShelf();

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
                    rfcontroller.SetPantallaTxt("NoMasTareas", new object[] {order.Name, order.Dock, currentContainerClient });              
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
        order.Tasks[currentTask].ContainerRef.SetSelected(false);
        currentTask += 1;

        if (currentTask < order.Tasks.Count)
        {
            state = StateGame.ScannerLocation;
            if (tutorial)
            {
                order.Tasks[currentTask].ContainerRef.SetSelected(true);
            }
            if (order.Tasks[currentTask] is PickingTask picking)
            {
                rfcontroller.SetPantallaTxt("EnterLocation", new object[] { picking.Location, order.Name,
                                picking.Stock, currentContainerClient});
            }
        }
        else
        {
               // No mas tareas
                // ayuda llevar a muelle
                state = StateGame.ShowDockConfirmation;
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
            NextStep();
        }
    }
    #endregion

}

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
    FinishLevel
}