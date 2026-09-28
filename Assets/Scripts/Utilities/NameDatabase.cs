using UnityEngine;

public class NameDatabase : MonoBehaviour
{
    public static NameDatabase Instance { get; private set; }

    [Header("Fake Database di Nomi Inventati")]
    [SerializeField]
    private string[] funnyNames = new string[]
    {
        "SpeedDemon",
        "ProGamer99",
        "PixelKing",
        "ShadowNinja",
        "NeonRider",
        "TurboSnail",
        "ClickMaster",
        "BlazeRunner",
        "CyberSamurai",
        "GlitchHero"
    };

    private void Awake()
    {
        // Configurazione Singleton persistente o per scena
        if (Instance == null)
        {
            Instance = this;
            // Se vuoi che sopravvenga al cambio di scena decommenta la riga sotto:
            // DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Restituisce un nome casuale in formato "User" + numero a 3 cifre (es. User435).
    /// </summary>
    public string GetRandomUserFormatName()
    {
        int randomId = Random.Range(100, 1000);
        return $"User#{randomId}";
    }

    /// <summary>
    /// Restituisce un nome casuale preso dalla lista predefinita di nomi inventati.
    /// </summary>
    public string GetRandomFunnyName()
    {
        if (funnyNames == null || funnyNames.Length == 0)
        {
            return GetRandomUserFormatName(); // Fallback di sicurezza
        }

        int randomIndex = Random.Range(0, funnyNames.Length);
        return funnyNames[randomIndex];
    }

    public string[] GetFiveFakeGlobalName()
    {
        string[] choicedNames = new string[5];

        for (int i = 0; i < choicedNames.Length; i++)
        {
            choicedNames[i] = funnyNames[i];
        }

        return choicedNames;
    }
}