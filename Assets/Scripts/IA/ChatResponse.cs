using System.Collections;
using System.Collections.Generic;
using System.Text.Json.Serialization;


public class ChatResponse
{
   
    public string Id { get; set; } = default!;
  
    public List<Choice> Choices { get; set; }
    

    public long Created { get; set; }
   
    public string Model { get; set; } = default!;

    [Newtonsoft.Json.JsonProperty("system_fingerprint")]
    public string SystemFingerprint { get; set; }

   
    public string Object { get; set; } = default!;
    
    public Usage Usage { get; set; }
}
