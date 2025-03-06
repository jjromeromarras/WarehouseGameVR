using System;
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

        public CategoryScore()
        {
            level = LevelCategory.Principiante;
        }
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
    

    // Clasificación general y por categoría
    public LevelCategory overallLevel;
   

    // Método para calcular la puntuación
    public void CalculateClassifyPlayer()
    {
        // Suma total de puntos
        int totalPoints = 0;

        // Evaluar cada categoría
        foreach (var response in playerResponses)
        {
            int score = (2 - response.Value.pregunta1) + (2 - response.Value.pregunta1);
            // Clasificar categoría
            response.Value.level = GetCategoryLevel(score);
            totalPoints += score;
        }

        // Clasificar el nivel general
        overallLevel = GetOverallLevel(totalPoints);
    }

    public void SetClassifyPlayer(LevelCategory level)
    {       
        // Evaluar cada categoría
        foreach (var response in playerResponses)
        {
            int score = (2 - response.Value.pregunta1) + (2 - response.Value.pregunta1);
            // Clasificar categoría
            response.Value.level = level;
        }

        // Clasificar el nivel general
        overallLevel = level;
    }
    public LevelCategory GetLevel4Category(string categoryName)
    {
        return playerResponses[categoryName].level;
    }

    public LevelCategory SetLevel4Category(string categoryName, string level)
    {
        return playerResponses[categoryName].level = (LevelCategory)Enum.Parse(typeof(LevelCategory), level);
    }
    public void setCategoriaPregunta1(string categoryName, int value)
    {
        playerResponses[categoryName].pregunta1 = value;
    }

    public void setCategoriaPregunta2(string categoryName, int value)
    {
        playerResponses[categoryName].pregunta2 = value;
    }

    // Método para determinar el nivel por categoría
    private LevelCategory GetCategoryLevel(int score)
    {
        if (score <= 1) return LevelCategory.Principiante;
        if (score <= 3) return LevelCategory.Medio;
        return LevelCategory.Avanzado;
    }

    // Método para determinar el nivel general
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
