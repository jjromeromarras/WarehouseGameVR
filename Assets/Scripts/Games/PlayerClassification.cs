using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClassification 
{
    public class CategoryScore
    {
        public int pregunta1;
        public int pregunta2;
        public LevelCategory level;
    }

    // Diccionario de respuestas del jugador (las claves son las preguntas)
    public Dictionary<string, CategoryScore> playerResponses = new Dictionary<string, CategoryScore>
    {
        { "General", new CategoryScore() },
        { "RecepcionMateriales", new CategoryScore() },
        { "PreparacionPedidos", new CategoryScore() },
        { "UbicacionMateriales", new CategoryScore() },
        { "ManejoCarretillas", new CategoryScore() }
    };

    // Puntuaciones m�ximas por categor�a y total
    private const int maxPointsPerCategory = 4;
    private const int maxTotalPoints = 16;

    // Clasificaci�n general y por categor�a
    public LevelCategory overallLevel;
   

    // M�todo para calcular la puntuaci�n
    public void ClassifyPlayer()
    {

        // Suma total de puntos
        int totalPoints = 0;

        // Evaluar cada categor�a
        foreach (var response in playerResponses)
        {
            int score = (2 - response.Value.pregunta1) + (2 - response.Value.pregunta1);
            // Clasificar categor�a
            response.Value.level = GetCategoryLevel(score);
            totalPoints += score;
        }

        // Clasificar el nivel general
        overallLevel = GetOverallLevel(totalPoints);
    }
    
    public void setCategoriaPregunta1(string categoryName, int value)
    {
        playerResponses[categoryName].pregunta1 = value;
    }

    public void setCategoriaPregunta2(string categoryName, int value)
    {
        playerResponses[categoryName].pregunta2 = value;
    }

    // M�todo para determinar el nivel por categor�a
    private LevelCategory GetCategoryLevel(int score)
    {
        if (score <= 1) return LevelCategory.Principiante;
        if (score <= 3) return LevelCategory.Medio;
        return LevelCategory.Avanzado;
    }

    // M�todo para determinar el nivel general
    private LevelCategory GetOverallLevel(int totalPoints)
    {
        if (totalPoints <= 4) return LevelCategory.Principiante;
        if (totalPoints <= 8) return LevelCategory.Medio;
        if (totalPoints <= 12) return LevelCategory.Avanzado;
        return LevelCategory.Experto;
    }
   

    public enum LevelCategory
    {
        Principiante,
        Medio,
        Avanzado,
        Experto
    }
}
