using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class datalevels 
{
    public long InitialPoints { get; set; }
    public long FinalPoints { get; set; }
    public int Aciertos { get; set; }
    public int Errors { get; set; }
    public double TotalTime { get; set; }

    public override string ToString()
    {
        return $"-- InitialPoints:{InitialPoints} -- FinalPoints{FinalPoints} -- Aciertos:{Aciertos} -- Errors:{Errors} -- TotalTime:{TotalTime}";
    }
}
