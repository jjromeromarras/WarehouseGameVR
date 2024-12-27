using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Prompt 
{
    public string model { get; set; }
    public Message[] messages { get; set; }
    public int max_tokens { get; set; }
    public float temperature { get; set; }

    public Prompt()
    {
        messages = new Message[1];
    }
}
