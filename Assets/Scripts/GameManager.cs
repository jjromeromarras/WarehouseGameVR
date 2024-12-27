using Newtonsoft.Json;
using System;
using System.Collections;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static UnityEditor.PlayerSettings;
using log4net.Core;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static GameManager Instance;

    [SerializeField] public bool showayuda;
    [SerializeField] public bool showminimap;
    [SerializeField] public bool penalización;
    [SerializeField] public bool mandoxbox;
    [SerializeField] public int minlevel;
    [SerializeField] public int maxlevel;

    public IA iagame;
    public Player player;
    private UnityEngine.AsyncOperation asyncLoad;
    private Logger logger;
    public bool wait4IAResponse;
    public string IAResponse;
    public int localize;
    private void Start()
    {
        localize = 0;
        logger = FindObjectOfType<Logger>();
        wait4IAResponse = false;
    }
    private void Awake()
    {
        if(GameManager.Instance == null)
        {
            player = new Player();
            GameManager.Instance = this;
            this.showayuda = true;
            this.showminimap = true;
            this.penalización = true;
            this.mandoxbox = true;
            iagame = new IA();
            wait4IAResponse = true;
            string prompt = "Hola, necesito que me contestes en español. Estoy desarrollando un videojuego de gamificación sobre el funcionamiento de logistica en los almacenes. Los jugadores asumiran el rol de un operario de un almacén realizando diversos retos (preparación de pedidos, recepción de materiales, uso de carretillas etc). Cada una de estas categorias dispone de varios retos. Tú papel consistirá en guiar a los jugadores durante estos retos y diseñar un entrenamiento personalizado. " +
                "Te proporcionaré información para poder clasificar el nivel de los jugadores, te proporcionaré información de los retos (su composición y como graduar su dificultad). " +
                "Durante el juego te proporcionaré información del resultados de los jugadores para que les puedas propocirionar y realizar una entrenamiento personalizado (ajustando la curva de dificultad al jugador)." +
                "Por favor si entiendes el mensaje contesta solamente \"SI\".";
            this.iagame.Chat("Llama 3.2 3B Instruct", prompt, (response) =>
            {
                if (!string.IsNullOrEmpty(response))
                {
                    // Deserializar el JSON si es necesario
                    ResponseMsg responseMsg = JsonConvert.DeserializeObject<ResponseMsg>(response);
                    Debug.Log($"Respuesta: {responseMsg.choices[0].message.content}");
                    if (responseMsg.choices[0].message.content == "SI")
                    {
                        prompt = "El primer paso es medir el nivel del jugador para poder ajustar los retos y el sistema de aprendizaje. Disponemos de los siguientes retos: " +
                        "3 retos de preparación de pedidos. 2 retos de recepción de materiales y clasificación de materiales." +
                        "2 retos de ubicación de material. 2 retos de manejo de carretillas. Necesito medir el nivel de jugador en cada uno de estos retos a fin " +
                        "de ajustar su dificultad. Explicación de los retos: En los retos de preparación de pedidos el jugador aprender a prepara un pedido mediante el juego. Debe seleccionar el contenedor cliente donde guardar el material recogido y posteriormente realizar la tarea de picking.Una tarea de picking consiste en escanear la ubicación origen del contenedor donde se encuentra el material, escanear el contenedor origen y coger el material del contenedor. Cada reto esta compuesto por 1 o varias tareas.Y cada reto puede preparar de 1 a 3 pedidos a la vez." +
                        "En los retos de recepción el jugador debe ir al pulmon de recepción, escanear los contenedores a recepcionar, confirmar que el material es correcto, tanto en el tipo como en cantidad.Cada reto puede tener de 1 a 8 contenedores y la dificultad estan en confirmar el material del contenedor(pudiendo ser monoreferencia o multireferencia). " +
                        "En los retos de ubicación el jugador de ubicar correctamente los contenedores recepcionados. Pudiendo ser guiado el procedimiento o libremente por usuario. " +
                        "Finalmente en los retos de carretilla el jugador debe aprender a manejar una carretilla elevadora. En el primer reto de carretillas debe descargar un camión de contenedores y llevarlos a la zona de recepción. " +
                        "En el segundo reto de carretillas debe recoger las paletas que salen del almacén automático (4 salidas) y llevar los contenedores a tres zonas: ubicarlos, pulmon o muelle " +
                        "Para medir el nivel inicial del jugador y diseñar un plan de entrenamiento personalizado, realizaremos una serie de preguntas al jugador. Estarán centradas en " +
                        ":Experiencia previa: Conocer su familiaridad con las tareas representadas en los retos." +
                        "Conocimientos teóricos: Evaluar conceptos logísticos básicos." +
                        "Habilidades específicas: Identificar fortalezas o debilidades en actividades concretas." +
                        "Conocimiento técnico: Saber si está familiarizado con las herramientas del almacén (terminal de RF, carretillas, etc.). " +
                        "Clasificaremos a los jugadores en cuatro niveles: Principiante, Medio, Avanzado, Experto, en función de sus respuestas a las preguntas, usaremos un sistema de puntos acumulativos por categoría y establecer rangos de puntuación para cada nivel. En función de las respuestas de cada categoría el jugador obtendrá un puntación." +
                        "La clasificación del jugador su nivel general y/o dentro de cada categoría sería la siguiente: Principiante: 0 - 4 puntos, Medio: 5 - 8 puntos, Avanzado: 9 - 12 puntos, Experto: 13 - 16 puntos." +
                        "Ejemplo de Clasificación: Jugador A (Respuestas): Recepción de Materiales: 3 puntos (Medio). Preparación de Pedidos: 4 puntos (Avanzado). Ubicación de Materiales: 2 puntos (Medio). Manejo de Carretillas: 0 puntos (Principiante). Clasificación General: 8 puntos → Medio. Este jugador recibiría un plan de entrenamiento enfocado en mejorar habilidades de manejo de carretillas y refuerzo en ubicación de materiales, mientras que los retos de preparación de pedidos y recepción serían más avanzados." +
                        "Por favor si entiendes el mensaje contesta solamente \"SI\".";
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
        this.iagame.Chat("Llama 3.2 3B Instruct", prompt, (response) =>
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
