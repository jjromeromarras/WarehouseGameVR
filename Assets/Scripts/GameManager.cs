using Newtonsoft.Json;
using System.Collections;
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
    [SerializeField] public bool enableIA;
    [SerializeField] public bool debug;

    public IA iagame;
    public Player player;
    private UnityEngine.AsyncOperation asyncLoad;
    private Logger logger;
    public bool wait4IAResponse;
    public string IAResponse;
    public string[] IAmodels = { "ChatGPT-3.5 Turbo", "ChatGPT-4", "Nous Hermes 2 Mistral DPO", "Llama 3.2 3B Instruct", "GPT4All Falcon", "DeepSeek" };
    public int IAmodelIndx = 5;
    public int localize;
    
    private void Start()
    {
        localize = 0;
        logger = FindObjectOfType<Logger>();
        wait4IAResponse = false;
        enableIA = true;
       

    }

    public void InitialIA()
    {
        IAmodelIndx = 5;

        string prompt = $"En este proyecto, los jugadores asumen el rol de operarios de almacén y deben superar diversos retos relacionados con tareas logísticas. Tu función será guiar a los jugadores a lo largo de estos retos y diseñar un entrenamiento personalizado para cada uno de ellos." +
            $"Te proporcionaré la siguiente información clave: Clasificación del nivel inicial de los jugadores y descripción de los retos" +
            $"Retos disponibles: 1. Preparación de pedidos. 2. Recepción de materiales. 3. Ubicación de materiales. 4. Manejo de carretillas Analizar los datos proporcionados al completar cada reto (fallos, aciertos y tiempo total).\r\nUtilizar esta información para ajustar los parámetros de los retos futuros y optimizar la curva de aprendizaje del jugador. No necesito respuesta, solamente un \"Si\"";
        this.iagame.Chat("DeepSeek", prompt, (response) =>
            {
                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ResponseMsg responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(response);
                    Debug.Log($"Respuesta: {responseMsg.choices[0].message.content}");
                    if (responseMsg.choices[0].message.content == "SI")
                    {
                        prompt = $"Los jugadores se clasifican en cuatro niveles (de menor a mayor): Principiante, Medio, Avanzado, Experto, según su desempeño en las preguntas iniciales. Estas preguntas evalúan:  \r\n- Experiencia previa: Familiaridad con tareas logísticas.  \r\n- Conocimientos teóricos: Conceptos básicos de logística.  \r\n- Habilidades específicas: Fortalezas o debilidades en actividades concretas.  \r\n- Conocimiento técnico: Uso de herramientas como terminales RF o carretillas.  " +
                        $"\r\n Ejemplo de Clasificación:  \r\n- Recepción de Materiales: 3 puntos → Medio.  \r\n- Preparación de Pedidos: 4 puntos → Avanzado.  \r\n- Ubicación de Materiales: 2 puntos → Medio.  \r\n- Manejo de Carretillas: 0 puntos → Principiante.  \r\n- Clasificación General: 8 puntos → Medio." +
                        $"Este jugador recibiría un plan personalizado para reforzar habilidades de manejo de carretillas y ubicación de materiales, mientras que los retos avanzados estarían orientados a preparación de pedidos y recepción.Tu objetivo como IA será procesar esta información y proporcionar orientación adecuada en tiempo real, ajustando los retos y el plan de aprendizaje de acuerdo con el nivel y el progreso del jugador." +
                        $"Por favor si entiendes el mensaje contesta solamente \"SI\".";
                        this.iagame.Chat("Llama 3.2 3B Instruct", prompt, (response) =>
                        {
                            if (!string.IsNullOrEmpty(response))
                            {
                                if (!string.IsNullOrEmpty(response))
                                {
                                    // Deserializar el JSON si es necesario
                                    ResponseMsg responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(response);
                                    Debug.Log($"Respuesta: {responseMsg.choices[0].message.content}");
                                    wait4IAResponse = false;
                                }
                            }
                        });
                    }
                }
                else
                {
                    Debug.LogError("No se recibió ninguna respuesta del modelo.");
                }
            });
    }
    private void Awake()
    {
        if(GameManager.Instance == null)
        {
            player = new Player();
            GameManager.Instance = this;
            this.UsedIA = true;
            this.showminimap = true;
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
        //Llama 3.2 3B Instruct"
        this.iagame.Chat(IAmodels[IAmodelIndx], prompt, (response) =>
        {
            if (!string.IsNullOrEmpty(response))
            {
                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ResponseMsg responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(response);
                    Debug.Log($"Respuesta: {responseMsg.choices[0].message.content}");
                    IAResponse = responseMsg.choices[0].message.content;
                    wait4IAResponse = false;
                }
            }
        });
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
