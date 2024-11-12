using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
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
    // Start is called before the first frame update
    void Start()
    {
        InitLevel();
        
        if (rfcontroller != null)
        {
            rfcontroller.SetTitle("TareasAutomaticas");
        }
        switch (numberlevel)
        {
            case 1:
                game = new GameReception(data, 1, "Tutorial Reception");
                state = StateGame.ShowBienVenido;
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
                    showTexto("PrimerBienvenida");
                    break;
                }
            case StateGame.ShowTutorial1:
                {
                    showTexto("Tutorial1Recep");
                    break;
                }
            case StateGame.ShowTutorialInvenCont:
                {
                    showTexto("TutorialInveCont");
                    break;
                }
            case StateGame.ShowPrimerIventario:
                {
                    showTexto("PrimerInventario");
                    break;
                }
            case StateGame.ShowFinishLevel:
                {
                    timer.SetTimerOn(false);
                    infotext.SetActiveInfo(true);
                    waitreading = true;
                    StartCoroutine(infotext.SetMessageKey("FinalizarReception", 2f));                 
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
            bonificacion += 10;
            GameManager.Instance.player.Score += 10;
            state = StateGame.EnterQuantity;
            rfcontroller.SetPantallaTxt("EnterCantidadRecep", new object[] { currentTask.parentOrder.Name, currentTask.Container, currentTask.Stock, currentTask.Quantity });
            receptionCamera.setStateQuantity();
            receptionCamera.setenableActions(true);
            return false;
        }
        else if (!check && currentTask.isFake)
        {
            SoundManager.SharedInstance.PlaySound(scannerOK);
            bonificacion += 10;
            GameManager.Instance.player.Score += 10;
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
            penalizacion += 5;
            GameManager.Instance.player.Score -= 5;
            return false;
        }
    }
    public override void onResetTask()
    {
       
            penalizacion += 5;
            GameManager.Instance.player.Score -= 5;      
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
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;
                    return false;
                }
                else
                {
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    infotext.SetActiveInfo(true);
                    setLockPlayer(true);
                    NextStep();
                    // Reception correcto
                    bonificacion += 10;
                    GameManager.Instance.player.Score += 10;
                    return true;
                }
            }
            else
            {
                showerror = true;
                SoundManager.SharedInstance.PlaySound(scannerError);
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errorreceptionquantity", 2f, new object[] { currentTask.Quantity }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;   
                return false;
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
                    break;
                }

            case StateGame.ShowTutorial1:
                {
                    if (currentTask != null)
                    {
                        rfcontroller.SetPantallaTxt("Reception", new object[] { currentTask.parentOrder.Name, currentTask.Location });
                    }
                    setLockPlayer(false);
                    infotext.SetActiveInfo(false);
                    state = StateGame.EnterStage;
                }
                break;
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
        infotext.SetActiveInfo(false);    
        if (currentTask.isLast)
        {
            state = StateGame.ShowFinishLevel;
        }
        else
        {
            GetTask();
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
                    SoundManager.SharedInstance.PlaySound(scannerOK);
                    bonificacion += 10;
                    GameManager.Instance.player.Score += 10;
                    state = StateGame.EnterContainer;
                    rfcontroller.SetPantallaTxt("EnterContainerRecep", new object[] { currentTask.parentOrder.Name, currentTask.Container });
                    receptionCamera.setenableActions(true);
                }
                else
                {
                    SoundManager.SharedInstance.PlaySound(scannerError);
                    showerror = true;
                    infotext.SetActiveInfo(true);
                    StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { location }));
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;

                }
            }
            else if (!showerror && !waitreading)
            {
                SoundManager.SharedInstance.PlaySound(scannerError);
                showerror = true;
                infotext.SetActiveInfo(true);
                StartCoroutine(infotext.SetMessageKey("errordockscanner", 2f, new object[] { location }));
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
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
                    state = StateGame.EnterItem;
                    receptionCamera.setStateItem();
                    rfcontroller.SetPantallaTxt("EnterArticuloRecep", new object[] { currentTask.parentOrder.Name, container, currentTask.Stock});
                    receptionpallet.gameObject.SetActive(true);
                    this.setOnReceptionLocation();
                    pncliente.gameObject.SetActive(false);
                    receptionpallet.stock.SetSSCC(container);
                    receptionpallet.stock.SetStock(currentTask.Stock , currentTask.Quantity, container, true, currentTask.isFake);
                    bonificacion += 5;
                    GameManager.Instance.player.Score += 5;
                    receptionCamera.setenableActions(true);
                }
                else
                {
                    if (!showerror && !waitreading)
                    {
                        SoundManager.SharedInstance.PlaySound(scannerError);
                        showerror = true;
                        infotext.SetActiveInfo(true);
                        StartCoroutine(infotext.SetMessageKey("ErrorIntroducirContenedorInve", 2f, new object[] { container }));
                        penalizacion += 5;
                        GameManager.Instance.player.Score -= 5;
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
                    penalizacion += 5;
                    GameManager.Instance.player.Score -= 5;                    
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
                penalizacion += 5;
                GameManager.Instance.player.Score -= 5;
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
        FinishLevel
    }

}
