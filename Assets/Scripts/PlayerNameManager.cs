using System.Globalization;
using TMPro; // Usa TextMeshPro per l'input field
using UnityEngine;

public class PlayerNameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField nameInputField; // Il campo di input del nome nell'immagine
    [SerializeField] private TMP_Text recordTxt; // Il campo di input del nome nell'immagine


    private void Start()
    {
        string loadedName = NameDatabase.Instance.GetRandomUserFormatName();
        SaveRandomNameIfNotExist(loadedName);
    }

    void SaveRandomNameIfNotExist(string name)
    {
        if (!PlayerPrefs.HasKey(DataConstDatabase.PlayerName))
        {
            PlayerPrefs.SetString(DataConstDatabase.PlayerName, name);
            PlayerPrefs.Save();
        }
    }

    public void LoadUserInfo()
    {
        LoadPlayerBestRecord();
        LoadPlayerName();
    }

    /// <summary>
    /// Carica il nome salvato in precedenza (se esiste) e lo mostra nell'InputField.
    /// </summary>
    void LoadPlayerName()
    {
        string loadedName;

        if (nameInputField == null) return;

        if (!PlayerPrefs.HasKey(DataConstDatabase.PlayerName))
        {
            loadedName = NameDatabase.Instance.GetRandomUserFormatName();

            // Salviamo subito il nome fake generato per mantenerlo coerente
            PlayerPrefs.SetString(DataConstDatabase.PlayerName, loadedName);
            PlayerPrefs.Save();
        }
        else
        {
            loadedName = PlayerPrefs.GetString(DataConstDatabase.PlayerName);
        }

        nameInputField.text = loadedName;
    }

    void LoadPlayerBestRecord()
    {
        if (recordTxt != null && ScoreManager.Instance != null)
        {
            float bestTime = ScoreManager.Instance.GetBestTime(HundredBombsManager.targetTiles);

            if (bestTime == float.MaxValue)
            {
                recordTxt.text = "--.--"; // Se non c'è ancora un record
            }
            else
            {
                recordTxt.text = bestTime.ToString("F2", CultureInfo.InvariantCulture);
            }
        }
    }

    /// <summary>
    /// Da collegare all'evento OnClick del bottone CONFIRM.
    /// Salva il nome inserito dall'utente.
    /// </summary>
    public void SavePlayerName()
    {
        if (nameInputField == null) return;

        string nameToSave = nameInputField.text;

        // Se l'utente lascia il campo vuoto, rimettiamo un nome di default
        if (string.IsNullOrWhiteSpace(nameToSave))
        {
            return;
        }

        PlayerPrefs.SetString(DataConstDatabase.PlayerName, nameToSave);
        PlayerPrefs.Save();

        Debug.Log($"Nome giocatore salvato: {nameToSave}");

        // Opzionale: qui puoi aggiungere la chiusura del pannello del menu
        // gameObject.SetActive(false);
    }
}