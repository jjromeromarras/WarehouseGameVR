using UnityEngine.Networking;

public class GrokClient: ClientIA
{
    private const string url = "https://api.x.ai/v1/chat/completions";
    public GrokClient(): 
        base("GrokClient", url)
    {
        
    }
    
}
