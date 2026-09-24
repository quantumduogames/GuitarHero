using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class UIEventManager : MonoBehaviour
{
    [SerializeField] private GameObject bannersContainer;

    [Header("Global Leaderboard Banners (Simulated)")]
    [SerializeField] private GameObject worldRecordBanner;   // Banner 1° al Mondo!
    [SerializeField] private GameObject top5GlobalBanner;   // Banner Top 5 Globale!

    [Header("Local Leaderboard Banners")]
    [SerializeField] private GameObject recordBanner;       // Banner 1° Locale
    [SerializeField] private GameObject top5LocalBanner;    // Banner Top 5 Locale
    [SerializeField] private GameObject retryBanner;        // Banner Vittoria Normale / Retry

    [Header("Leaderboards Parent Objects")]
    [SerializeField] private Transform localLeaderboardContainer;  // Contenitore Padre delle 5 righe Locali
    [SerializeField] private Transform globalLeaderboardContainer; // Contenitore Padre delle 5 righe Globali

    private void OnEnable()
    {
        MinigameManager.OnGameCompleted += ChooseBannerToShow; 
    }

    private void OnDisable()
    {
        MinigameManager.OnGameCompleted -= ChooseBannerToShow; 
    }

    private void ChooseBannerToShow(float completedTime)
    {
        if (ScoreManager.Instance == null) return; 

        HideAllBanners(); 
        int targetTiles = 100; 

        // 1. Aggiorna lo ScoreManager col nuovo tempo
        ScoreManager.Instance.AddScore(targetTiles, completedTime, "You"); 
        ScoreManager.Instance.AddFakeGlobalScore(targetTiles, completedTime, "You"); 

        // 2. Popola visivamente i 5 elementi nelle due Leaderboard
        PopulateLeaderboardUI(localLeaderboardContainer, ScoreManager.Instance.GetTopScores(targetTiles)); 
        PopulateLeaderboardUI(globalLeaderboardContainer, ScoreManager.Instance.GetFakeGlobalTopScores(targetTiles)); 

        // 3. Calcola posizioni e attiva il banner idoneo con i relativi testi
        int globalRank = ScoreManager.Instance.GetFakeGlobalRankForScore(targetTiles, completedTime); 
        int localRank = ScoreManager.Instance.GetRankForScore(targetTiles, completedTime); 

        string formattedTime = completedTime.ToString("F2", CultureInfo.InvariantCulture) + "s";

        if (globalRank == 0) 
        {
            ShowBannerWithData(worldRecordBanner, "WORLD RECORD!", formattedTime);
        }
        else if (globalRank >= 1 && globalRank <= 4) 
        {
            ShowBannerWithData(top5GlobalBanner, $"GLOBAL TOP {globalRank + 1}!", formattedTime);
        }
        else if (localRank == 0) 
        {
            ShowBannerWithData(recordBanner, "NEW BEST TIME!", formattedTime);
        }
        else if (localRank >= 1 && localRank <= 4) 
        {
            ShowBannerWithData(top5LocalBanner, $"LOCAL TOP {localRank + 1}!", formattedTime);
        }
        else
        {
            ShowBannerWithData(retryBanner, "TRY AGAIN!", formattedTime);
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
        if (worldRecordBanner) worldRecordBanner.SetActive(false); 
        if (top5GlobalBanner) top5GlobalBanner.SetActive(false); 
        if (recordBanner) recordBanner.SetActive(false); 
        if (top5LocalBanner) top5LocalBanner.SetActive(false); 
        if (retryBanner) retryBanner.SetActive(false); 
        if (bannersContainer) bannersContainer.SetActive(false); 
    }
}