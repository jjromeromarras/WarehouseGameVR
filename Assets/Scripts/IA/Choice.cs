using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class Choice 
{
    [Newtonsoft.Json.JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
    public int index { get; set; }
    public MessageDeep message { get; set; }
  
}
