using UnityEngine.Networking;

public class DeepSeekClient: ClientIA
{   
    private const string apiKey = "sk-840bb031b4b446df8b81a4ceea7fb6ab";
    private const string url = "https://api.deepseek.com/chat/completions";
    UnityWebRequest Http;
    

    public DeepSeekClient(): base(apiKey, url)
    {
       
    }   
    
}
