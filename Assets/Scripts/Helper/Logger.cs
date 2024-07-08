using System.IO;
using UnityEngine;

public class Logger : MonoBehaviour
{
    private string logFilePath;

    void Start()
    {
        // Define el path del log file
        logFilePath = Path.Combine(Application.persistentDataPath, "game_log.txt");

        // Opcional: Limpia el archivo de log anterior al iniciar
        if (File.Exists(logFilePath))
        {
            File.Delete(logFilePath);
        }

        // Escribir en el log file al iniciar
        Log("Game Started");
    }

    public void Log(string message)
    {
        // Añade la fecha y hora al mensaje
        string logMessage = $"{System.DateTime.Now}: {message}";

        // Escribir el mensaje en el log file
        File.AppendAllText(logFilePath, logMessage + System.Environment.NewLine);

        // Opcional: También puedes imprimir el mensaje en la consola de Unity
        // Debug.Log(logMessage);
    }

    void OnApplicationQuit()
    {
        // Escribir en el log file al salir
        Log("Game Ended");
    }
}
