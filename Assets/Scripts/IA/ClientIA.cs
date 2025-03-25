using System.Collections;
using System;
using UnityEngine.Networking;
using Newtonsoft.Json;
using System.Diagnostics;

public class ClientIA
{
    private string apiKey;
    private string Url;

    public ClientIA(string variableName, string strUrl)
    {
        this.Url = strUrl;
        ApiKey = Environment.GetEnvironmentVariable(variableName);
    }

    public string ApiKey { get => apiKey; set => apiKey = value; }

    public IEnumerator Chat(ChatRequest request, Action<string> callback)
    {

        string jsonData = JsonConvert.SerializeObject(request);
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start(); // Inicia la medición del tiempo
        using (UnityWebRequest webRequest = UnityWebRequest.Post(Url, jsonData))
        {
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.SetRequestHeader("Content-Type", "application/json");
            webRequest.SetRequestHeader("Authorization", $"Bearer {ApiKey}");
            // Enviar solicitud
            yield return webRequest.SendWebRequest();
            stopwatch.Stop(); // Detiene la medición del tiempo
            UnityEngine.Debug.Log($"Tiempo de ejecución de la IA: {stopwatch.ElapsedMilliseconds} ms");
            GameManager.Instance.WriteLog($"Tiempo de ejecución de la IA: {stopwatch.ElapsedMilliseconds} ms");


            if (webRequest.result == UnityWebRequest.Result.ConnectionError ||
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                UnityEngine.Debug.LogError($"Error en la solicitud POST: {webRequest.error}");
                GameManager.Instance.WriteLog($"Error en la solicitud POST: {webRequest.error}");
                callback?.Invoke(null); // Enviar null si hay un error
            }
            else
            {
                GameManager.Instance.WriteLog($"Respuesta POST: {webRequest.downloadHandler.text}");
                UnityEngine.Debug.Log($"Respuesta POST: {webRequest.downloadHandler.text}");
                callback?.Invoke(webRequest.downloadHandler.text); // Enviar respuesta como JSON
            }
        }
    }
}
