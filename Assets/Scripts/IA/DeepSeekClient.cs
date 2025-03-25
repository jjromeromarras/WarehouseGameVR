using UnityEngine.Networking;

public class DeepSeekClient: ClientIA
{
    private const string url = "https://api.deepseek.com/chat/completions";
   

    public DeepSeekClient(): base("DeepSeekClient", url)
    {
       
    }   
    
}
