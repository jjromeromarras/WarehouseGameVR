using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class Choice 
{
    [Newtonsoft.Json.JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
    public int Index { get; set; }
    public Message Message { get; set; }
  
    public Logprobs Logprobs { get; set; }

   
    public Message Delta { get; set; }

    /// <summary>
    /// for completion 
    /// </summary>
    public string Text { get; set; } = string.Empty;
}
