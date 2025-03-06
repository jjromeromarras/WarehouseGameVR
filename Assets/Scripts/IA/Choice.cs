public class Choice 
{
    [Newtonsoft.Json.JsonProperty("finish_reason")]
    public string FinishReason { get; set; }
    public int index { get; set; }
    public MessageDeep message { get; set; }
  
}
