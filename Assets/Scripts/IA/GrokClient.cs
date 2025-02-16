using UnityEngine.Networking;

public class GrokClient: ClientIA
{
  
    private const string apiKey = "xai-Bu7AIb5DCJOKiltuDLagQwMDaFMB5iyxbx2aEUtUf80HrSKuygxyzH7n8RkwXS1LcNaqzmEBckUZQRuh";
    private const string url = "https://api.x.ai/v1/chat/completions";

    UnityWebRequest Http;

    public GrokClient(): 
        base(apiKey, url)
    {
        
    }
    
}
