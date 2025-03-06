using System.Collections.Generic;

public partial class ChatRequest
{
    public List<MessageDeep> messages { get; set; }
    public string model { get; set; }
    public bool stream { get; set; }


    public ChatRequest()
    {        
        stream = false;
    }
}
