using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    public event Action<bool> onSetLockPlayer;

    protected void OnSetLocationScanner()
    {
        
    }

    protected void OnSetContainerScanner()
    {
       
    }

    public void setLockPlayer(bool value)
    {
        if (onSetLockPlayer != null)
        {
            onSetLockPlayer(value);
        }
    }
}
