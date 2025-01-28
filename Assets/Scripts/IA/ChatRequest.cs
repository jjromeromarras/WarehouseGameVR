using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class ChatRequest
{
    public List<MessageDeep> Messages { get; set; }
    public string Model { get; set; }

    [Newtonsoft.Json.JsonProperty("frequency_penalty")]
    public double FrequencyPenalty { get; set; }

    [Newtonsoft.Json.JsonProperty("max_tokens")]
    public long MaxTokens { get; set; } = 4096;
    [Newtonsoft.Json.JsonProperty("presence_penalty")]
    public double PresencePenalty { get; set; } = 0;

    /// <summary>
    /// type:text or json_object
    /// </summary>
    [Newtonsoft.Json.JsonProperty("response_format")]
    public ResponseFormat ResponseFormat { get; set; }

    /// <summary>
    /// Up to 16 sequences where the API will stop generating further tokens.
    /// </summary>
    public List<string> Stop { get; set; }
    
   
    public bool Stream { get; set; }


    [Newtonsoft.Json.JsonProperty("stream_options")]
    public StreamOptions StreamOptions { get; set; }
    public double Temperature { get; set; } = 1;
    [Newtonsoft.Json.JsonProperty("top_p")]
    public long TopP { get; set; } = 1;
    [Newtonsoft.Json.JsonProperty("logprobs")]
    public bool Logprobs { get; set; }
    [Newtonsoft.Json.JsonProperty("top_logprobs")]
    public int? TopLogprobs { get; set; }

    public ChatRequest()
    {
        Model = DeepSeekModels.ChatModel;
        FrequencyPenalty = 0;
        MaxTokens = 4096;
        PresencePenalty = 0;
        Stop = new List<string>();
        Messages = new List<MessageDeep>();
        Temperature = 1;
        TopP = 1;


    }
}
