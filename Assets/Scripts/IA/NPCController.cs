using UnityEngine;

public class NPCController : MonoBehaviour
{
    public ChatGPTClient chatGPTClient;
    public string npcPrompt = "�Hola, aventurero! �En qu� puedo ayudarte hoy?";

    private void Start()
    {
        // Inicio de una conversaci�n con el NPC
        if(chatGPTClient != null)
            StartCoroutine(chatGPTClient.GetChatResponse(npcPrompt, OnChatResponse));
    }

    private void OnChatResponse(string response)
    {
        // Aqu� puedes mostrar la respuesta en la UI del juego
        Debug.Log("Respuesta del NPC: " + response);
        // Usa esta respuesta en el di�logo del NPC
    }
}
