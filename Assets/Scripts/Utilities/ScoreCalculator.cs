using UnityEngine;

public class ScoreCalculator : MonoBehaviour
{
    /// <summary>
    /// Calcola il punteggio in base alla percentuale di accuratezza (0 - 100%).
    /// </summary>
    /// <param name="accuracy">Percentuale (es. 86.5f, 59.9f, 100f)</param>
    /// <returns>Punteggio ottenuto (int)</returns>
    public static int CalculateScore(float accuracy)
    {
        // 1. Sotto il 60% -> 0 punti
        if (accuracy < 60f)
        {
            return 0;
        }
        // 2. Da 60% a 70% -> 10 punti
        else if (accuracy < 70f)
        {
            return 10;
        }
        // 3. Da 70% a 80% -> 25 punti
        else if (accuracy < 85f)
        {
            return 25;
        }
        // 4. Da 85% a 95% -> 50 punti
        else if (accuracy < 95f)
        {
            return 50;
        }
        // 5. Da 95% a 100% (o superiore) -> 100 punti (Perfetto!)
        else
        {
            return 100;
        }
    }
}