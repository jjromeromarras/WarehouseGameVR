﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;

    [SerializeField] public bool UsedIA;
    [SerializeField] public bool showminimap;
    [SerializeField] public bool penalización;
    [SerializeField] public bool mandoxbox;
    [SerializeField] public int minlevel;
    [SerializeField] public int maxlevel;
    [SerializeField] public bool debug;

    public IA iagame;
    public Player player;

    private Logger logger;
    public bool wait4IAResponse;
    public string IAResponse;
    public string[] IAmodels = { "ChatGPT-3.5 Turbo", 
        "ChatGPT-4",       
        "DeepSeek",
        "Grok"};
    public int IAmodelIndx = 0;
    public int localize;

    private string prompclasificacion =  $"Los jugadores se clasifican en cuatro niveles (de menor a mayor): Principiante, Medio, Avanzado, Experto, según su desempeño en las preguntas iniciales. Estas preguntas evalúan:  \r\n- Experiencia previa: Familiaridad con tareas logísticas.  \r\n- Conocimientos teóricos: Conceptos básicos de logística.  \r\n- Habilidades específicas: Fortalezas o debilidades en actividades concretas.  \r\n- Conocimiento técnico: Uso de herramientas como terminales RF o carretillas.  " +
                           $"\r\n Ejemplo de Clasificación:  \r\n- Recepción de Materiales: 3 puntos → Medio.  \r\n- Preparación de Pedidos: 4 puntos → Avanzado.  \r\n- Ubicación de Materiales: 2 puntos → Medio.  \r\n- Manejo de Carretillas: 0 puntos → Principiante.  \r\n- Clasificación General: 8 puntos → Medio." +
                           $"Este jugador recibiría un plan personalizado para reforzar habilidades de manejo de carretillas y ubicación de materiales, mientras que los retos avanzados estarían orientados a preparación de pedidos y recepción.Tu objetivo como IA será procesar esta información y proporcionar orientación adecuada en tiempo real, ajustando los retos y el plan de aprendizaje de acuerdo con el nivel y el progreso del jugador." +
                           $"Por favor si entiendes el mensaje contesta solamente \"SI\".";

    private void Start()
    {
        localize = 0;
        logger = FindObjectOfType<Logger>();
        wait4IAResponse = false;
        UsedIA = false;    
    }

    public void InitialIA()
    {

        string prompt = $"En este proyecto, los jugadores asumen el rol de operarios de almacén y deben superar diversos retos relacionados con tareas logísticas. Tu función será guiar a los jugadores a lo largo de estos retos y diseñar un entrenamiento personalizado para cada uno de ellos." +
            $"Te proporcionaré la siguiente información clave: Clasificación del nivel inicial de los jugadores y descripción de los retos" +
            $"Retos disponibles: 1. Preparación de pedidos. 2. Recepción de materiales. 3. Ubicación de materiales. 4. Manejo de carretillas Analizar los datos proporcionados al completar cada reto (fallos, aciertos y tiempo total).\r\nUtilizar esta información para ajustar los parámetros de los retos futuros y optimizar la curva de aprendizaje del jugador. No necesito respuesta, solamente un \"Si\"";
        if (IAmodels[IAmodelIndx] == "DeepSeek")
        {
            this.iagame.ChatDeepSeek(prompt, (chatResponse) =>
            {
                ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                if (responseMsg != null)
                {

                    // result
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    this.iagame.ChatDeepSeek(prompclasificacion, (response) =>
                    {

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Deserializar el JSON si es necesario
                            ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                            Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            wait4IAResponse = false;
                        }
                        else
                        {
                            Debug.LogError("No se recibió ninguna respuesta del modelo.");
                            GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");
                        }
                    });
                }
                else
                {
                    GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                }
            });
        }
        else if (IAmodels[IAmodelIndx] == "Grok")
        {
            this.iagame.ChatGrok(prompt, (chatResponse) =>
            {
                ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                if (responseMsg != null)
                {

                    // result
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    this.iagame.ChatGrok(prompclasificacion, (response) =>
                    {

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Deserializar el JSON si es necesario
                            ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                            Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                            wait4IAResponse = false;
                        }
                        else
                        {
                            Debug.LogError("No se recibió ninguna respuesta del modelo.");
                            GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                        }
                    });
                }
                else
                {
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                    GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                }
            });
        }
        else if (IAmodels[IAmodelIndx] == "ChatGPT-3.5 Turbo")
        {
            this.iagame.ChatGPT(prompt, (chatResponse) =>
            {
                ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                if (responseMsg != null)
                {

                    // result
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    this.iagame.ChatGPT(prompclasificacion, (response) =>
                    {

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Deserializar el JSON si es necesario
                            ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                            Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                            wait4IAResponse = false;
                        }
                        else
                        {
                            Debug.LogError("No se recibió ninguna respuesta del modelo.");
                            GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                        }
                    }, LLMModels.gpt3turbo);
                }
                else
                {
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                    GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                }
            }, LLMModels.gpt3turbo);
        }
        else if (IAmodels[IAmodelIndx] == "ChatGPT-4")
        {
            this.iagame.ChatGPT(prompt, (chatResponse) =>
            {
                ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                if (responseMsg != null)
                {

                    // result
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    this.iagame.ChatGPT(prompclasificacion, (response) =>
                    {

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Deserializar el JSON si es necesario
                            ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                            Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                            wait4IAResponse = false;
                        }
                        else
                        {
                            Debug.LogError("No se recibió ninguna respuesta del modelo.");
                            GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                        }
                    }, LLMModels.gpt4turbo);
                }
                else
                {
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                    GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                }
            }, LLMModels.gpt4turbo);
        }
        else if (IAmodels[IAmodelIndx] == "Llama")
        {
            this.iagame.ChatLlama(prompt, (chatResponse) =>
            {
                ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                if (responseMsg != null)
                {

                    // result
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    this.iagame.ChatLlama(prompclasificacion, (response) =>
                    {

                        if (!string.IsNullOrEmpty(response))
                        {
                            // Deserializar el JSON si es necesario
                            ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(chatResponse);
                            Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                            GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                            wait4IAResponse = false;
                        }
                        else
                        {
                            Debug.LogError("No se recibió ninguna respuesta del modelo.");
                            GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                        }
                    });
                }
                else
                {
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                    GameManager.Instance.WriteLog("No se recibió ninguna respuesta del modelo.");

                }
            });
        }
    }
    private void Awake()
    {
        if (GameManager.Instance == null)
        {
            player = new Player();
            GameManager.Instance = this;
            //this.UsedIA = true;
            this.showminimap = false;
            this.penalización = true;
            this.mandoxbox = true;
            iagame = new IA();
            wait4IAResponse = true;

            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    public string GetLanguage()
    {
        if (localize == 0)
        {
            return "Español";
        }
        else if (localize == 1)
            return "Ingles";
        else
            return "Frances";

    }
    public void SendIAMsg(string prompt)
    {
        wait4IAResponse = true;
        Debug.Log($"Pregunta IA {IAmodels[IAmodelIndx]}: {prompt}");
        GameManager.Instance.WriteLog($"Pregunta IA {IAmodels[IAmodelIndx]}: {prompt}");
        if (IAmodels[IAmodelIndx] == "DeepSeek")
        {
            this.iagame.ChatDeepSeek(prompt, (response) =>
            {

                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(response);
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    string patron = @"\{[\s\S]*?\}";
                    Match match = Regex.Match(responseMsg?.choices.FirstOrDefault()?.message?.content, patron);
                    IAResponse = match.Value;
                    wait4IAResponse = false;
                }


            });
        } else if (IAmodels[IAmodelIndx] == "Grok")
        {
            this.iagame.ChatGrok(prompt, (response) =>
            {

                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(response);
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    string patron = @"\{[\s\S]*?\}";
                    Match match = Regex.Match(responseMsg?.choices.FirstOrDefault()?.message?.content, patron);
                    IAResponse = match.Value;
                    wait4IAResponse = false;
                }

            });
        }
        else if (IAmodels[IAmodelIndx] == "ChatGPT-3.5 Turbo")
        {
            this.iagame.ChatGPT(prompt, (response) =>
            {

                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(response);
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    string patron = @"\{[\s\S]*?\}";
                    Match match = Regex.Match(responseMsg?.choices.FirstOrDefault()?.message?.content, patron);
                    IAResponse = match.Value;
                    wait4IAResponse = false;
                }

            }, LLMModels.gpt3turbo);
        }
        else if (IAmodels[IAmodelIndx] == "ChatGPT-4")
        {
            this.iagame.ChatGPT(prompt, (response) =>
            {

                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ChatResponse responseMsg = JsonConvert.DeserializeObject<ChatResponse>(response);
                    Debug.Log($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");
                    GameManager.Instance.WriteLog($"Respuesta: {responseMsg?.choices.FirstOrDefault()?.message?.content}");

                    string patron = @"\{[\s\S]*?\}";
                    Match match = Regex.Match(responseMsg?.choices.FirstOrDefault()?.message?.content, patron);
                    IAResponse = match.Value;
                    wait4IAResponse = false;
                }

            }, LLMModels.gpt4turbo);
        }       
    }
    public IEnumerator BackMenu()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainMenu");
        yield return null;
    }
    public IEnumerator ResetLevel()
    {
        Cursor.lockState = CursorLockMode.None;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Warehouse");
        yield return null;
    }

    public void WriteLog(string msg)
    {
        logger.Log(msg);
    }
}
