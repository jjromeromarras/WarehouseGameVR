using System.Collections;
using UnityEngine;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;

public class ClientIA
{
    private string apiKey;
    private string Url;

    UnityWebRequest Http;

    public ClientIA(string strapiKey, string strUrl)
    {
        this.apiKey = strapiKey;
        this.Url = strUrl;
    }

    public IEnumerator Chat(ChatRequest request, Action<string> callback)
    {

        string jsonData = JsonConvert.SerializeObject(request);
        using (UnityWebRequest webRequest = UnityWebRequest.Post(Url, jsonData))
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
}
