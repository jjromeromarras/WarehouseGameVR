using System;
using TMPro;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    [SerializeField] public timer timer;
    [SerializeField] public nivelText txtNivel;
    [SerializeField] public TextMeshProUGUI txtErrores;
    [SerializeField] public AudioClip scannerOK, scannerError;
    [SerializeField] public int numberlevel;
    [SerializeField] public bool tutorial;
    public event Action<string, string, shelf, string, string, string, int> onSetPickingLocation;
    public event Action onSetReceptionLocation;
    public int bonificacion;
    public int penalizacion;
    public bool showhelp;
    public event Action<bool> onSetLockPlayer;
    public event Action<int, int, int> onFinishTask;
    public bool waitreading;
    public bool showerror;
    public Game game;
    public int currentData;
    public void InitLevel()
    {
        bonificacion = 0;
        penalizacion = 0;
        showhelp = tutorial;

        waitreading = false;
        showerror = false;

        if (infotext != null)
        {
            infotext.onFinishInfoText += FinishInfoText;
            infotext.SetActiveInfo(true);
        }
        GameManager.Instance.player.Data.Add(GameManager.Instance.player.Data.Count, new datalevels());
        currentData = GameManager.Instance.player.Data.Count-1;
        GameManager.Instance.player.Data[currentData].InitialPoints = GameManager.Instance.player.Score;
        settxtErrors(0);

    }

    public virtual void OnSetLocationScanner(string location, string tag)
    {

        
    }

    public virtual void OnSetContainerScanner(string container, string tag)
    {

        
    }

    public virtual void OnSetDockScanner(string dock, string tag)
    {

    }

    public virtual void onErrorContainerClient() { }

    public virtual bool CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        return false;
    }

    public virtual bool CheckContainerPicking(string container)
    {
        return false;
    }

    public virtual void OnExistPickingScene() { }

    public virtual void onResetTask()
    {
        
    }

    public virtual void onErrorPicking()
    {

    }
    public virtual void onFinishErrorMsg()
    {

    }
    public void setLockPlayer(bool value)
    {
        if (onSetLockPlayer != null)
        {
            onSetLockPlayer(value);
        }
        timer.SetTimerOn(!value);
    }

    public void showTextoKey(string key)
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

    public void showMsg(string msg)
    {
        if (!waitreading)
        {
            timer.SetTimerOn(false);
            infotext.SetActiveInfo(true);
            waitreading = true;
            StartCoroutine(infotext.SetMessageText(msg, 2f));
        }
    }

    public void setFinishLevel()
    {
        if(onFinishTask != null)
        {
            onFinishTask(Convert.ToInt16(timer.TimeLeft), bonificacion, penalizacion);
        }
    }
    public void setPickingLocation(string stock, string container, shelf location, string contclient1, string contclient2, string contclient3, int pedido)
    {
        if (onSetPickingLocation != null)
        {
            onSetPickingLocation(stock, container, location, contclient1, contclient2, contclient3, pedido);
        }

    }

    public void setOnReceptionLocation()
    {
        if (onSetReceptionLocation != null)
        {
            onSetReceptionLocation();
        }

    }
    public virtual void NextStep() { }

    public void FinishInfoText()
    {
        waitreading = false;
        if (showerror)
        {
            showerror = false;
            infotext.SetActiveInfo(false);
            setLockPlayer(false);
            onFinishErrorMsg();
        }
        else
        {
            NextStep();
        }
    }

    public void settxtErrors(int errores)
    {
        if (txtErrores != null)
        {
            txtErrores.text = errores.ToString();
        }
    }

    public void AddPenalty(int penalty)
    {
        penalizacion += penalty;
        GameManager.Instance.player.Score -= penalty;
        GameManager.Instance.player.Data[currentData].Errors += 1;
        settxtErrors(GameManager.Instance.player.Data[currentData].Errors);
    }

    public void AddBonificacion(int bonus)
    {
        bonificacion += bonus;
        GameManager.Instance.player.Score += bonus;
        GameManager.Instance.player.Data[currentData].Aciertos += 1;
    }
}
