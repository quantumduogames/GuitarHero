using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ScoreEntry
{
    public string playerName;
    public float scoreTime;

    public ScoreEntry(string name, float time)
    {
        playerName = name;
        scoreTime = time;
    }
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    private const string LocalTimeKeyPrefix = "LocalBestTime_";
    private const string LocalNameKeyPrefix = "LocalBestName_";

    private const string GlobalTimeKeyPrefix = "FakeGlobalBestTime_";
    private const string GlobalNameKeyPrefix = "FakeGlobalBestName_";

    public const int MaxScores = 5;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // --- LOCALE ---

    public List<ScoreEntry> GetTopScores(int targetTiles = 100)
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();
        for (int i = 0; i < MaxScores; i++)
        {
            string timeKey = $"{LocalTimeKeyPrefix}{targetTiles}_{i}";
            string nameKey = $"{LocalNameKeyPrefix}{targetTiles}_{i}";

            if (PlayerPrefs.HasKey(timeKey))
            {
                float time = PlayerPrefs.GetFloat(timeKey);
                string name = PlayerPrefs.GetString(nameKey, "Player");
                scores.Add(new ScoreEntry(name, time));
            }
        }
        return scores;
    }

    public int AddScore(int targetTiles, float newTime, string playerName = "You")
    {
        return SaveToLeaderboard(LocalTimeKeyPrefix, LocalNameKeyPrefix, targetTiles, newScore: new ScoreEntry(playerName, newTime));
    }

    public int GetRankForScore(int targetTiles, float time)
    {
        return GetRankInList(GetTopScores(targetTiles), time);
    }

    // --- FAKE GLOBALE ---

    public List<ScoreEntry> GetFakeGlobalTopScores(int targetTiles = 100)
    {
        List<ScoreEntry> scores = new List<ScoreEntry>();
        string[] fakeNames = { "SpeedDemon", "ProGamer99", "PixelKing", "Shadow", "NeonRider" };

        for (int i = 0; i < MaxScores; i++)
        {
            string timeKey = $"{GlobalTimeKeyPrefix}{targetTiles}_{i}";
            string nameKey = $"{GlobalNameKeyPrefix}{targetTiles}_{i}";

            if (!PlayerPrefs.HasKey(timeKey))
            {
                float defaultTime = 10f + (i * 2.5f);
                PlayerPrefs.SetFloat(timeKey, defaultTime);
                PlayerPrefs.SetString(nameKey, fakeNames[i % fakeNames.Length]);
            }

            scores.Add(new ScoreEntry(PlayerPrefs.GetString(nameKey), PlayerPrefs.GetFloat(timeKey)));
        }
        return scores;
    }

    public int AddFakeGlobalScore(int targetTiles, float newTime, string playerName = "You")
    {
        return SaveToLeaderboard(GlobalTimeKeyPrefix, GlobalNameKeyPrefix, targetTiles, newScore: new ScoreEntry(playerName, newTime));
    }

    public int GetFakeGlobalRankForScore(int targetTiles, float time)
    {
        return GetRankInList(GetFakeGlobalTopScores(targetTiles), time);
    }

    // --- HELPER ---

    private int SaveToLeaderboard(string timePrefix, string namePrefix, int targetTiles, ScoreEntry newScore)
    {
        List<ScoreEntry> scores = (timePrefix == GlobalTimeKeyPrefix)
            ? GetFakeGlobalTopScores(targetTiles)
            : GetTopScores(targetTiles);

        int rankPosition = -1;

        for (int i = 0; i < scores.Count; i++)
        {
            if (newScore.scoreTime < scores[i].scoreTime)
            {
                rankPosition = i;
                break;
            }
        }

        if (rankPosition == -1 && scores.Count < MaxScores)
        {
            rankPosition = scores.Count;
        }

        if (rankPosition != -1)
        {
            scores.Insert(rankPosition, newScore);
            if (scores.Count > MaxScores) scores.RemoveAt(scores.Count - 1);

            for (int i = 0; i < scores.Count; i++)
            {
                PlayerPrefs.SetFloat($"{timePrefix}{targetTiles}_{i}", scores[i].scoreTime);
                PlayerPrefs.SetString($"{namePrefix}{targetTiles}_{i}", scores[i].playerName);
            }
            PlayerPrefs.Save();
        }

        return rankPosition;
    }

    private int GetRankInList(List<ScoreEntry> scores, float time)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            if (Mathf.Approximately(scores[i].scoreTime, time)) return i;
        }
        return -1;
    }

    /// <summary>
    /// Restituisce il miglior tempo assoluto (1° posto) della classifica locale.
    /// </summary>
    public float GetBestTime(int targetTiles)
    {
        List<ScoreEntry> scores = GetTopScores(targetTiles);

        // Se la lista contiene almeno un record (1° posto), restituisce il suo tempo
        if (scores.Count > 0)
        {
            return scores[0].scoreTime;
        }

        // Se non c'è ancora nessun record salvato, restituisce float.MaxValue
        return float.MaxValue;
    }
}