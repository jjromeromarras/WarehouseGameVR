using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopLogprobs 
{
    public string Token { get; set; }
    public long Logprob { get; set; }
    public byte[] Bytes { get; set; }
}
