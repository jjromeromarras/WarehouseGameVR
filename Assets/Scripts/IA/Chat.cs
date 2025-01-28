using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using System.Security.Policy;
using System.Text;
using System.Threading;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public partial class IA
{

    private const string SERVER = "http://localhost:4891/";
    private const string SERVERDEEPSEEK = "https://api.deepseek.com";
    private const string apiKey = "sk-840bb031b4b446df8b81a4ceea7fb6ab";
    private class Endpoints
    {
        public const string MODELS = "v1/models";
        public const string TEXT = "v1/completions";
        public const string CHAT = "v1/chat/completions";
    }
    private DeepSeekClient clientDeepSeek = new DeepSeekClient(apiKey);

    private IEnumerator PostRequest(string uri, string jsonData, Action<string> callback)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Post(uri, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // Enviar solicitud
            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError($"Error en la solicitud POST: {webRequest.error}");
                callback?.Invoke(null); // Enviar null si hay un error
            }
            else
            {
                Debug.Log($"Respuesta POST: {webRequest.downloadHandler.text}");
                callback?.Invoke(webRequest.downloadHandler.text); // Enviar respuesta como JSON
            }
        }
    }


    /// <summary>
    /// Generate a response from prompt with chat context/history
    /// </summary>
    /// <param name="model">Ollama Model Syntax (<b>eg.</b> llama3.1)</param>
    /// <param name="image">A multimodal model is required to handle images (<b>eg.</b> llava)</param>
    /// <param name="keep_alive">The behavior to keep the model loaded in memory</param>
    /// <returns>response string from the LLM</returns>
    public void Chat(string model, string prompt, Action<string> callback)
    {


        Prompt systemPrompt = new Prompt
        {
            model = model,
            max_tokens = 50,
            messages = new Message[]
            {
                new Message
                {
                    content = prompt,
                    role = "user"
                }
            },
            temperature = 0.28f
        };

        string payload = JsonConvert.SerializeObject(systemPrompt);
        string uri = SERVER + Endpoints.CHAT;

        // Iniciar la coroutine PostRequest
        GameManager.Instance.StartCoroutine(PostRequest(uri, payload, callback));

    }

    public void ChatDeepSeek(string prompttxt, Action<string> callback)
    {
        var serverpromp = $"Eres un asistente en formación logística. Esta diseñado para " +
            $"poder personalizar el entrenamiento de cualquier operario del almacén. Tus conocimientos" +
            $"en lo logisticas son profundos. Entiendes los procesos de preparación de pedidos, recepción," +
            $"ubicación de material o manejo de carretillas. Además eres un experto en gamificación a fin de poder" +
            $"diseñar los entrenamiento mas atractivos y personalizados. Tu labor es ayudar a personalizar" +
            $"el entranamiento de los operarios del almacén.";

        var request = new ChatRequest
        {
            messages = new System.Collections.Generic.List<MessageDeep>() {
                MessageDeep.NewSystemMessage(serverpromp),
                MessageDeep.NewUserMessage(prompttxt)
            },

        };

        GameManager.Instance.StartCoroutine(clientDeepSeek.Chat(request, apiKey, callback));

      

    }
}
