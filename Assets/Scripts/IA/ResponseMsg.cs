using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ResponseMsg 
{
    public Choices[] choices;
    public string id { get; set; }
    public long created { get; set; }
}
