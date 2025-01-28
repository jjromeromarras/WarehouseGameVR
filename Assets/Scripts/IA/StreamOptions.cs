using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class StreamOptions 
{
    [Newtonsoft.Json.JsonProperty("include_usage")]
    public bool IncludeUsage { get; set; }
}
