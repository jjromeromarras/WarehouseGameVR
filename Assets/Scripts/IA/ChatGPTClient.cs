using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using System.Text;
using Newtonsoft.Json;  // Para serialización JSON, instala Newtonsoft JSON desde el Package Manager

public class ChatGPTClient : MonoBehaviour
{
    private string apiKey = "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"; // Coloca aquí tu clave API de OpenAI
    private string apiUrl = "https://api.openai.com/v1/chat/completions";

    public IEnumerator GetChatResponse(string prompt, System.Action<string> callback)
    {
        var requestData = new
        {
            model = "gpt-3.5-turbo", // O usa "gpt-4" si tienes acceso
            messages = new[]
            {
                new { role = "system", content = "Actúa como un NPC en un videojuego." },
                new { role = "user", content = prompt }
            },
            max_tokens = 100 // Ajusta el límite de tokens para tus respuestas
        };

        string jsonBody = JsonConvert.SerializeObject(requestData);
        var request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + apiKey);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var responseJson = JsonConvert.DeserializeObject<ChatGPTResponse>(request.downloadHandler.text);
            string responseText = responseJson.choices[0].message.content;
            callback(responseText);
        }
        else
        {
            Debug.LogError("Error en la solicitud: " + request.error);
        }
    }
}

[System.Serializable]
public class ChatGPTResponse
{
    public Choice[] choices;
}

[System.Serializable]
public class Choice
{
    public Message message;
}

[System.Serializable]
public class Message
{
    public string content;
}
