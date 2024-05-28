using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    [SerializeField] public timer timer;
    [SerializeField] public AudioClip scannerOK, scannerError;
    public int bonificacion;
    public int penalizacion;
    public event Action<bool> onSetLockPlayer;
    public event Action<string, string, shelf> onSetPickingLocation;
    public event Action<int, int, int> onFinishLevel;

    public virtual void OnSetLocationScanner(string location, string tag)
    {

        
    }

    public virtual void OnSetContainerScanner(string container, string tag)
    {

        
    }

    public virtual bool CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        return false;
    }

    public virtual void OnExistPickingScene() { }

    public virtual void onResetTask()
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
        if(onFinishLevel != null)
        {
            onFinishLevel(Convert.ToInt16(timer.TimeLeft), bonificacion, penalizacion);
        }
    }
    public void setPickingLocation(string stock, string container, shelf location)
    {
        if (onSetPickingLocation != null)
        {
            onSetPickingLocation(stock, container, location);
        }
       
    }
}
