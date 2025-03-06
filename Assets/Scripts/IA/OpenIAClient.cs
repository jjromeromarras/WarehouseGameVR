using UnityEngine.Networking;

public class OpenIAClient : ClientIA
{
    private const string apiKey = "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";
    private const string url = "https://api.openai.com/v1/chat/completions";


    UnityWebRequest Http;

    public OpenIAClient(): base(apiKey, url)
    {
        
    }
   
}
