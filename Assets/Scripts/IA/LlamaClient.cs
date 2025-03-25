using UnityEngine.Networking;

public class LLamaClient: ClientIA
{
    private const string url = "https://api.llama-api.com/";
   

    public LLamaClient(): base("LlamaMetaClient", url)
    {
       
    }   
    
}
