using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class orderwalker 
{
    [SerializeField]
    public Transform waypoint;
    [SerializeField]
    public bool ispicking;
    [SerializeField]
    public bool isfinish;
    [SerializeField]
    public bool isloading;
    [SerializeField]
    public bool isunloading;
    [SerializeField]
    public GameObject pallettoload;

    public orderwalker()
    {
        isfinish = false;
        ispicking = false;
        isunloading = false;
        isloading = false;
    }
}
