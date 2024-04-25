using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    [SerializeField] public timer timer;
    public event Action<bool> onSetLockPlayer;
    public event Action<string, string, shelf> onSetPickingLocation;    

    public virtual int OnSetLocationScanner(string location, string tag)
    {

        return 0;
    }

    public virtual int OnSetContainerScanner(string container, string tag)
    {

        return 0;
    }

    public virtual int CheckPicking(int cantplatano, int cantuvas, int cantpiña, int cantperas, int cantmelocoton, int cantmanzana, int cantfresa)
    {
        return 0;
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

    public void setPickingLocation(string stock, string container, shelf location)
    {
        if (onSetPickingLocation != null)
        {
            onSetPickingLocation(stock, container, location);
        }
       
    }
}
