using System;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    [SerializeField] public timer timer;
    [SerializeField] public AudioClip scannerOK, scannerError;
    [SerializeField] public int numberlevel;
    public int bonificacion;
    public int penalizacion;
    public Game game;
    public event Action<bool> onSetLockPlayer;
    public event Action<string, string, shelf, string, string, string, int> onSetPickingLocation;
    public event Action<int, int, int> onFinishTask;




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

    public void setLockPlayer(bool value)
    {
        if (onSetLockPlayer != null)
        {
            onSetLockPlayer(value);
        }
        timer.SetTimerOn(!value);
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
}
