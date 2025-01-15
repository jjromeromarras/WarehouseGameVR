using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ReceptionLevel : Level
{
    [SerializeField] public ReceptionScene receptionpallet;
    [SerializeField] public ReceptionData[] data;
    [SerializeField] public GameObject pncliente;
    [SerializeField] public ReceptionCamera receptionCamera;
    private StateGame state;
    private ReceptionTask currentTask;
    private Queue<ReceptionTask> tasks;
    private bool isFirt;
    // Start is called before the first frame update
    void Start()
    {
        InitLevel();
        isFirt = true;
        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }
        switch (numberlevel)
        {
            case 1:
                game = new GameReception(data, 1, 3, "Tutorial Reception", 10);                
                state = StateGame.ShowReceptionInformada;

                if (timer != null)
                {
                    timer.SetTimeLeft(300f);
                }
                break;
        }
        GameManager.Instance.WriteLog($"Iniciar game: {game.Name}");
        tasks = new Queue<ReceptionTask>();
        (game as GameReception).Orders.SelectMany(p => p.Tasks).ToList().ForEach(task => tasks.Enqueue(task as ReceptionTask));
        currentTask = tasks.Dequeue();
        if(numberlevel == 1){
            currentTask.parentOrder.DockRef.gameObject.SetActive(true);
        }
        if (txtNivel != null)
        {
            txtNivel.SetPantallaTxt("Receplevel1", new object[] { });
        }

    }

    // Update is called once per frame
    void Update()
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
                    showTextoKey("Tutorial1Recep");
                    break;
                }
            case StateGame.ShowTutorialInvenCont:
                {
                    showTextoKey("TutorialInveCont");
                    break;
                }
            case StateGame.ShowPrimerIventario:
                {
                    showTextoKey("PrimerInventario");
                    break;
                }
            case StateGame.ShowReceptionInformada:
                {
                    showTextoKey("showrecepcioninformada");
                    break;
                }
            case StateGame.ShowConfirmStockContainerRecep:
                {
                    showTextoKey("showconfirmstockcontainerrecp");
                    break;
                }
            case StateGame.ShowKeyConfirmStockRecep:
                {
                    showTextoKey("showkeyconfirmstockrecep");
                    break;
                }
            case StateGame.ShowConfirmCantidadRecep:
                {
                    showTextoKey("showconfirmcantidadrecep");
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

    public bool CheckItem(bool check)
    {
        GameManager.Instance.WriteLog($"[CheckItem] - check: {check} - isFake: {currentTask.isFake}");
        if (check && !currentTask.isFake)
        {
            SoundManager.SharedInstance.PlaySound(scannerOK);
            AddBonificacion(10);
            if (isFirt)
            {
                state = StateGame.ShowConfirmCantidadRecep;
                receptionCamera.setenableActions(false);
            } 
            else
            {
                receptionCamera.setenableActions(true);
                state = StateGame.EnterQuantity;
            }
            rfcontroller.SetPantallaTxt("EnterCantidadRecep", new object[] { currentTask.parentOrder.Name, currentTask.Container, currentTask.Stock, currentTask.Quantity });
            receptionCamera.setStateQuantity();
            
            return false;
        }
        else if (!check && currentTask.isFake)
        {
            SoundManager.SharedInstance.PlaySound(scannerOK);
            AddBonificacion(10);
            receptionCamera.setenableActions(true);
            setLockPlayer(false);
            NexTask();
            return true;
        } 
        else
        {
            SoundManager.SharedInstance.PlaySound(scannerError);
            showerror = true;
            infotext.SetActiveInfo(true);
            StartCoroutine(infotext.SetMessageKey("ErrorReceptionItem", 2f, new object[] { }));
            AddPenalty(5);
            return false;
        }
    }
    public override void onResetTask()
    {

        AddPenalty(5);
        //setPickingLocation(currentTask.Stock, currentTask.Container, currentTask.LocationRef, currentTask.parentOrder.ContainerClient,
        //    (game as GamePicking).Orders.Count > 1 ? (game as GamePicking).Orders[1].ContainerClient : string.Empty, (game as GamePicking).Orders.Count > 2 ? (game as GamePicking).Orders[2].ContainerClient : string.Empty,
        //    currentTask.parentOrder.Level);

    }
    public override bool CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        // check
            var total = cantfresa + cantplatano + cantperas + cantmelocoton + cantmanzana + cantuvas + cantpiña;
            if (total == currentTask.Quantity)
            {
            if ((currentTask.Stock == "piña" && cantpiña != total) ||
                 (currentTask.Stock == "melocton" && cantmelocoton != total) ||
                 (currentTask.Stock == "platano" && cantplatano != total) ||
                 (currentTask.Stock == "fresa" && cantfresa != total) ||
                 (currentTask.Stock == "peras" && cantperas != total) ||
                 (currentTask.Stock == "manzanas" && cantmanzana != total) ||
                 (currentTask.Stock == "uvas" && cantuvas != total))
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errorreceptionproduct", 2f, new object[] { currentTask.Stock }));
                AddPenalty(5);
                return false;
            }
            else
            {
                SoundManager.SharedInstance.PlaySound(scannerOK);
                infotext.SetActiveInfo(true);
                setLockPlayer(true);
                NextStep();
                // Reception correcto
                AddBonificacion(10);
                return true;
            }
            }
            else
            {
                showerror = true;
                SoundManager.SharedInstance.PlaySound(scannerError);
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errorreceptionquantity", 2f, new object[] { currentTask.Quantity }));
            AddPenalty(5);

            return false;
            }
        
    }

  
    public override void NextStep()
    {
        switch (state)
        {
            case StateGame.ShowBienVenido:
                {
                    receptionCamera.setenableActions(false);
                    setLockPlayer(true);
                    state = StateGame.ShowTutorial1;
                    break;
                }

            case StateGame.ShowReceptionInformada:
                {
                    receptionCamera.setenableActions(false);
                    setLockPlayer(true);
                    state = StateGame.ShowTutorial1;
                }
                break;

            case StateGame.ShowTutorial1:
                {
                    if (currentTask != null)
                    {
                        rfcontroller.SetPantallaTxt("Reception", new object[] { currentTask.parentOrder.Name, currentTask.Location });                        
                    }
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.EnterStage;
                    break;
                }
            case StateGame.ShowConfirmStockContainerRecep:
                {                    
                    state = StateGame.ShowKeyConfirmStockRecep;
                    break;
                }
            case StateGame.ShowKeyConfirmStockRecep:
                {
                    receptionCamera.setenableActions(true);
                    infotext.SetActiveInfo(false);
                    state = StateGame.EnterItem;
                    break;
                }
            case StateGame.ShowConfirmCantidadRecep:
                {
                    receptionCamera.setenableActions(true);
                    infotext.SetActiveInfo(false);
                    state = StateGame.EnterQuantity;
                    break;
                }
            case StateGame.EnterQuantity:
                {
                    setLockPlayer(false);
                    NexTask();
                    break;
                }
            case StateGame.ShowFinishLevel:
                {
                    setLockPlayer(true);
                    state = StateGame.FinishLevel;
                    this.setFinishLevel();
                    break;
                }
        }
    }

    private void NexTask()
    {
        isFirt = false;
        infotext.SetActiveInfo(false);
        if (!GetTask())
        {
            timer.SetTimerOn(false);
            state = StateGame.FinishLevel;
            this.setFinishLevel();
        }
        
    }
    private bool GetTask()
    {
        if (tasks.Count > 0)
        {
            currentTask = tasks.Dequeue();
            state = StateGame.EnterContainer;
            rfcontroller.SetPantallaTxt("EnterContainerRecep", new object[] { currentTask.parentOrder.Name, currentTask.Container });
            return true;
        }
        return false;
    }

    public override void onFinishErrorMsg()
    {
        receptionCamera.setenableActions(true);
    }
    public override void OnSetLocationScanner(string location, string tag)
    {
        if (state == StateGame.EnterStage)
        {
            if (tag == "dock")
            {
                if (location == currentTask.parentOrder.Dock)
                {
                    currentTask.parentOrder.DockRef.gameObject.SetActive(false);
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    AddBonificacion(10);
                    state = StateGame.EnterContainer;
                    rfcontroller.SetPantallaTxt("EnterContainerRecep", new object[] { currentTask.parentOrder.Name, currentTask.Container });
                
                }
                else
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    showerror = true;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { location }));
                    AddPenalty(5);


                }
            }
            else if (!showerror && !waitreading)
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { location }));
                AddPenalty(5);

            }

        }
    }

    public override void OnSetContainerScanner(string container, string tag)
    {
        if (state == StateGame.EnterContainer)
        {
            if (tag == "receptionpallet")
            {
                var pallet = currentTask.ContainerRef;
                if (pallet != null && pallet.ssc == container)
                {
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    if (isFirt)
                    {
                        receptionCamera.setenableActions(false);
                        state = StateGame.ShowConfirmStockContainerRecep;
                    }
                    else
                    {
                        receptionCamera.setenableActions(true);
                        state = StateGame.EnterItem;
                    }
                    receptionCamera.setStateItem();
                    rfcontroller.SetPantallaTxt("EnterArticuloRecep", new object[] { currentTask.parentOrder.Name, container, currentTask.Stock});
                    receptionpallet.gameObject.SetActive(true);
                    this.setOnReceptionLocation();
                    pncliente.gameObject.SetActive(false);
                    receptionpallet.stock.SetSSCC(container);
                    receptionpallet.stock.SetStock(currentTask.Stock , currentTask.Quantity, container, true, currentTask.isFake);
                    AddBonificacion(5);

                }
                else
                {
                    if (!showerror && !waitreading)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerError);
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedorInve", 2f, new object[] { container }));
                        AddPenalty(5);

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
                    StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedorInve", 2f, new object[] { container }));
                    AddPenalty(5);

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
                StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedorInve", 2f, new object[] { container }));
                AddPenalty(5);

            }
        }


    }
    internal enum StateGame
    {
        ShowBienVenido,
        ShowTutorial1,
        ShowTutorialInvenCont,
        ShowPrimerIventario,
        EnterStage,
        EnterContainer,
        EnterItem,
        EnterQuantity,
        ShowFinishLevel,
        ShowReceptionInformada,
        ShowConfirmStockContainerRecep,
        ShowKeyConfirmStockRecep,
        ShowConfirmCantidadRecep,
        FinishLevel
    }

}
