using System;
using UnityEngine.Networking;

public class OpenIAClient : ClientIA
{
    
    private const string url = "https://api.openai.com/v1/chat/completions";

    public OpenIAClient(): base("OpenIAClient", url)
    {
        
    }
   
}
