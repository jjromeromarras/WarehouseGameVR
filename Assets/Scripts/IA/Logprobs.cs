using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;
using static Codice.CM.WorkspaceServer.WorkspaceTreeDataStore;

public class Logprobs 
{
    [Newtonsoft.Json.JsonProperty("text_offset")]
    public int[] TextOffset { get; set; }

    public string[] Tokens { get; set; }
    [Newtonsoft.Json.JsonProperty("token_logprobs")]
    public double[] TokenLogProbs { get; set; }

    [Newtonsoft.Json.JsonProperty("top_logprobs")]
    public List<TopLogprobs> TopLogProbs { get; set; }
    public List<Content> Content { get; set; }
}
