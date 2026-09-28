using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{
    [SerializeField] private GameObject bannersContainer;

    [Header("Banners")]
    [SerializeField] private GameObject newRecordBanner;   // Corrisponde a Panel - NewRecord
    [SerializeField] private GameObject defaultBanner;     // Corrisponde a Panel - Default

    [Header("Leaderboards Parent Objects")]
    [SerializeField] private Transform localLeaderboardContainer;
    [SerializeField] private Transform localLeaderboardContainerInGame;
    [SerializeField] private Transform globalLeaderboardContainer;
    [SerializeField] private Transform globalLeaderboardContainerInGame;

    private void OnEnable()
    {
        HundredBombsManager.OnGameCompleted += ChooseBannerToShow;
    }

    private void OnDisable()
    {
        HundredBombsManager.OnGameCompleted -= ChooseBannerToShow;
    }

    private void Start()
    {
        RefreshLeaderboardsUI();
    }

    /// <summary>
    /// Metodo per aggiornare visivamente le classifiche attingendo dai dati salvati o fake.
    /// </summary>
    private void RefreshLeaderboardsUI()
    {
        if (ScoreManager.Instance == null) return;

        int targetTiles = 100;

        // Popola i dati salvati/iniziali nelle varie leaderboard
        PopulateLeaderboardUI(localLeaderboardContainer, ScoreManager.Instance.GetLocalTopScores(targetTiles));
        PopulateLeaderboardUI(localLeaderboardContainerInGame, ScoreManager.Instance.GetLocalTopScores(targetTiles));
        PopulateLeaderboardUI(globalLeaderboardContainer, ScoreManager.Instance.GetFakeGlobalTopScores(targetTiles));
        PopulateLeaderboardUI(globalLeaderboardContainerInGame, ScoreManager.Instance.GetFakeGlobalTopScores(targetTiles));
    }

    private void ChooseBannerToShow(float completedTime)
    {
        if (ScoreManager.Instance == null) return;

        HideAllBanners();
        int targetTiles = 100;
        string playerName = PlayerPrefs.GetString(DataConstDatabase.PlayerName, "Not found");
        // 1. Aggiorna lo ScoreManager col nuovo tempo
        ScoreManager.Instance.AddScore(targetTiles, completedTime, playerName);
        ScoreManager.Instance.AddFakeGlobalScore(targetTiles, completedTime, playerName);

        // 2. Rinfresca visivamente i 5 elementi nelle leaderboard con i nuovi dati aggiornati
        RefreshLeaderboardsUI();

        // 3. Calcola posizioni
        int globalRank = ScoreManager.Instance.GetFakeGlobalRankForScore(targetTiles, completedTime);
        int localRank = ScoreManager.Instance.GetRankForScore(targetTiles, completedTime);

        string formattedTime = completedTime.ToString("F2", CultureInfo.InvariantCulture);

        // 4. Mostra New Record se è 1° al mondo (globalRank == 0) o 1° locale (localRank == 0)
        if (globalRank == 0 || localRank == 0)
        {
            ShowBannerWithData(newRecordBanner, "NEW RECORD!", formattedTime);
        }
        else
        {
            ShowBannerWithData(defaultBanner, "TRY AGAIN!", formattedTime);
        }

        if (bannersContainer != null) bannersContainer.SetActive(true);
    }

    /// <summary>
    /// Attiva il Gameobject del banner specificato e ne aggiorna il Titolo e lo Score tramite FinishBanner.
    /// </summary>
    private void ShowBannerWithData(GameObject bannerObj, string title, string score)
    {
        if (bannerObj == null) return;

        bannerObj.SetActive(true);

        FinishBanner bannerScript = bannerObj.GetComponent<FinishBanner>();
        if (bannerScript != null)
        {
            bannerScript.SetTexts(title, score);
        }
    }

    private void PopulateLeaderboardUI(Transform container, List<ScoreEntry> scores)
    {
        if (container == null) return;

        LeaderboardRowUI[] rows = container.GetComponentsInChildren<LeaderboardRowUI>(true);

        for (int i = 0; i < rows.Length; i++)
        {
            if (i < scores.Count)
            {
                rows[i].gameObject.SetActive(true);
                rows[i].SetData(scores[i].playerName, scores[i].scoreTime);
            }
            else
            {
                rows[i].gameObject.SetActive(false);
            }
        }
    }

    private void HideAllBanners()
    {
        if (newRecordBanner) newRecordBanner.SetActive(false);
        if (defaultBanner) defaultBanner.SetActive(false);
        if (bannersContainer) bannersContainer.SetActive(false);
    }
}