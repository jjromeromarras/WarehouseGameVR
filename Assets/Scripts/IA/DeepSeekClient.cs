using System.Collections;
using System.Collections.Generic;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Text.Unicode;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Net.Http;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using UnityEditor.PackageManager.Requests;
using Newtonsoft.Json;
using System.Security.Policy;

public class DeepSeekClient
{
    public readonly string BaseAddress = "https://api.deepseek.com";
    public readonly string BetaBaseAddress = "https://api.deepseek.com/beta";
    public readonly string ChatEndpoint = "/chat/completions";
    public readonly string CompletionEndpoint = "/completions";
    public readonly string UserBalanceEndpoint = "/user/balance";

    public readonly string ModelsEndpoint = "/models";
    private const string StreamDoneSign = "data: [DONE]";
    UnityWebRequest Http;

    //public Newtonsoft.Json.JsonSerializerSettings JsonSerializerOptions = new()
    //{
    //    ReferenceHandler = ReferenceHandler.IgnoreCycles,
    //    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    //    Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    //};
    public string ErrorMsg { get; set; }

    public DeepSeekClient(string apiKey)
    {
        Http = new UnityWebRequest(BaseAddress+ChatEndpoint,"POST");
        Http.downloadHandler = new DownloadHandlerBuffer();
        Http.SetRequestHeader("Content-Type", "application/json");
        Http.SetRequestHeader("Authorization", $"Bearer {apiKey}");
    }

    public IEnumerator Chat(ChatRequest request, string apiKey, Action<string> callback)
    {

        string jsonData = JsonConvert.SerializeObject(request);
        using (UnityWebRequest webRequest = UnityWebRequest.Post("https://api.deepseek.com/chat/completions", jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
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
    public IEnumerator Chat(ChatRequest request, Action<ChatResponse> callback)
    {
        request.stream = false;
             
        var content = JsonConvert.SerializeObject(request);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(content);
        Http.uploadHandler = new UploadHandlerRaw(bodyRaw);
        Http.downloadHandler = new DownloadHandlerBuffer();
        yield return Http.SendWebRequest();


        if (Http.result == UnityWebRequest.Result.ConnectionError ||
                Http.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.LogError($"Error en la solicitud POST: {Http.error}");
            callback?.Invoke(null); // Enviar null si hay un error
        }
        else
        {
            Debug.Log($"Respuesta POST: {Http.downloadHandler.text}");
            var response = JsonConvert.DeserializeObject<ChatResponse>(Http.downloadHandler.text);
            callback?.Invoke(response); // Enviar respuesta como JSON
        }


    }
}
