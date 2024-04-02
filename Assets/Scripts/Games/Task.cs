using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task 
{
    public string Location { get; set; }
    public string Container { get; set; }

    public int Points { get; set; }
    public int errors { get; set; }
    public decimal totalTime { get; set; }

    public bool locationScan { get; set; } = false;
    public bool containerScan { get; set; } = false;

}
