using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewRecordPanel : MonoBehaviour
{
    [System.Serializable]
    public struct RankConfig
    {
        public string rankSuffix;        // "1ST", "2ND", "3RD", "4TH", "5TH"
        public Sprite trophySprite;      // Sprite del trofeo corrispondente
        public ParticleSystem vfxEffect; // VFX specifico da attivare (opzionale)
    }

    [Header("Riferimenti UI")]
    [SerializeField] private Image trophyImage;
    [SerializeField] private TextMeshProUGUI txtPlace;
    [SerializeField] private TextMeshProUGUI txtRecordedTime;

    [Header("Configurazione Posizioni (Indice 0 = 1° posto, 4 = 5° posto)")]
    [SerializeField] private List<RankConfig> rankConfigs = new List<RankConfig>();

    [Header("VFX Generici (Es. Coriandoli, Trombette, Luci)")]
    // Puoi trascinare tutti i ParticleSystem che vuoi da Unity
    [SerializeField] private List<ParticleSystem> globalWinVFX = new List<ParticleSystem>();

    /// <summary>
    /// Configura il pannello in base alla posizione (da 1 a 5).
    /// </summary>
    /// <param name="position">Posizione ottenuta (1, 2, 3, 4 o 5)</param>
    /// <param name="timeRecorded">Tempo effettuato in formato stringa o float</param>
    public void SetupPanel(int position, string timeRecorded)
    {
        int index = position;

        // Controllo sul range 0 - 4 (per 5 elementi)
        if (index < 0 || index >= rankConfigs.Count)
        {
            Debug.LogWarning($"[WinPanelController] Posizione {position} fuori range (0 - {rankConfigs.Count - 1})!");
            return;
        }

        RankConfig config = rankConfigs[index];

        if (trophyImage != null && config.trophySprite != null)
        {
            trophyImage.sprite = config.trophySprite;
        }

        if (txtPlace != null)
        {
            txtPlace.text = config.rankSuffix;
        }

        if (txtRecordedTime != null)
        {
            txtRecordedTime.text = timeRecorded + "!";
        }

        TurnOffAllVFX();

        if (config.vfxEffect != null)
        {
            config.vfxEffect.gameObject.SetActive(true);
            config.vfxEffect.Play();
        }

        foreach (var vfx in globalWinVFX)
        {
            if (vfx != null)
            {
                vfx.gameObject.SetActive(true);
                vfx.Play();
            }
        }

        gameObject.SetActive(true);
    }

    private void TurnOffAllVFX()
    {
        foreach (var config in rankConfigs)
        {
            if (config.vfxEffect != null)
            {
                config.vfxEffect.Stop();
                config.vfxEffect.gameObject.SetActive(false);
            }
        }

        foreach (var vfx in globalWinVFX)
        {
            if (vfx != null)
            {
                vfx.Stop();
                vfx.gameObject.SetActive(false);
            }
        }
    }
}
