using UnityEngine;

public class NPCController : MonoBehaviour
{
    public ChatGPTClient chatGPTClient;
    public string npcPrompt = "¡Hola, aventurero! ¿En qué puedo ayudarte hoy?";

    private void Start()
    {
        // Inicio de una conversación con el NPC
        if(chatGPTClient != null)
            StartCoroutine(chatGPTClient.GetChatResponse(npcPrompt, OnChatResponse));
    }

    private void OnChatResponse(string response)
    {
        // Aquí puedes mostrar la respuesta en la UI del juego
        Debug.Log("Respuesta del NPC: " + response);
        // Usa esta respuesta en el diálogo del NPC
    }
}
