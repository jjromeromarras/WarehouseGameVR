using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http;
using System.Text;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
using UnityEngine.Networking;

public partial class IA
{

    private const string SERVER = "http://localhost:4891/";
    private const string SERVERDEEPSEEK = "https://api.deepseek.com";
    private class Endpoints
    {
        public const string MODELS = "v1/models";
        public const string TEXT = "v1/completions";
        public const string CHAT = "v1/chat/completions";
    }


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


    private IEnumerator PostRequest(string uri, string jsonData, string apiKey, Action<string> callback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(uri, "POST"))
        {

            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {apiKey}");
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
        if (model != "DeepSeek")
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
        else
        {
            ChatDeepSeek(prompt, callback);
        }
    }

    public void ChatDeepSeek(string prompttxt, Action<string> callback)
    {
        // Tu clave de API
        string apiKey = "sk-840bb031b4b446df8b81a4ceea7fb6ab";

        // Datos que deseas enviar en el cuerpo de la solicitud
        var requestData = new
        {
            prompt = prompttxt,
            max_tokens = 50
        };

        var request = new ChatRequest
        {
            Messages = [
        Message.NewSystemMessage("You are a language translator"),
        Message.NewUserMessage("Please translate 'They are scared! ' into English!")
    ],
            // Specify the model
            Model = Constant.Model.ChatModel
        };


        // Serializar los datos a JSON
        string jsonContent = Newtonsoft.Json.JsonConvert.SerializeObject(requestData);

        // Iniciar la coroutine PostRequest
        string uri = SERVERDEEPSEEK + Endpoints.CHAT;
        GameManager.Instance.StartCoroutine(PostRequest(uri, jsonContent, apiKey, callback));

    }
}
