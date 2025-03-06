using System.Collections.Generic;


public class ChatResponse
{
   
    public string id { get; set; } = default!;
  
    public List<Choice> choices { get; set; }
    
    public long created { get; set; }

    public string model { get; set; }

    [Newtonsoft.Json.JsonProperty("object")]
    public string Object { get; set; } = default!;
}
