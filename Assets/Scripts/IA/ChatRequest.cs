using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public partial class ChatRequest
{
    public List<MessageDeep> messages { get; set; }
    public string model { get; set; }
    public bool stream { get; set; }


    public ChatRequest()
    {
        model = DeepSeekModels.ChatModel;
        stream = false;
    }
}
