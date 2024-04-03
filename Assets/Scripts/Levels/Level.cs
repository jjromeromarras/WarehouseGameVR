using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] public rfcontroller rfcontroller;   
    [SerializeField] public infotextcontroller infotext;
    public event Action<bool> onSetLockPlayer;

    public virtual int OnSetLocationScanner(string location)
    {
        return 0;
    }

    public virtual int OnSetContainerScanner(string container, string tag)
    {
        return 0;
    }

    public void setLockPlayer(bool value)
    {
        if (onSetLockPlayer != null)
        {
            onSetLockPlayer(value);
        }
    }
}
