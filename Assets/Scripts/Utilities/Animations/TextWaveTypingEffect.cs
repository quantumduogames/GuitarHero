using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextWaveTypingEffect : MonoBehaviour
{
    [Header("Component Reference")]
    [SerializeField] private TextMeshProUGUI textComponent;

    [Header("Typewriter Settings")]
    [SerializeField] private float writeDuration = 1.5f;
    [SerializeField] private Ease writeEase = Ease.Linear;

    [Header("Wave / Juice Settings")]
    [SerializeField] private bool enableWaveEffect = true;
    [SerializeField] private float waveScalePunch = 0.15f;
    [SerializeField] private float waveDuration = 0.5f;
    [SerializeField] private int waveVibrato = 5;
    [SerializeField] private float waveElasticity = 1f;

    private Tween textTween;
    private Tween waveTween;

    public bool AutoStart;

    private void Awake()
    {
        // Se non viene assegnato nell'inizializzatore, prova a cercarlo nello stesso GameObject
        if (textComponent == null)
        {
            textComponent = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        if (AutoStart)
        {
            PlayLoadingAnimation(textComponent.text);
        }
    }

    /// <summary>
    /// Avvia l'effetto macchina da scrivere combinato con l'effetto onda.
    /// </summary>
    public void PlayLoadingAnimation(string targetText)
    {
        // Resetta eventuali animazioni precedenti sullo stesso testo
        StopAnimation();

        if (textComponent == null) return;

        textComponent.text = "";
        string currentText = "";

        // Sostituto compatibile con DOTween Free per l'effetto DOText
        textTween = DOTween.To(() => currentText, x => currentText = x, targetText, writeDuration)
            .SetEase(writeEase)
            .OnUpdate(() => {
                textComponent.text = currentText;
            });

        // Avvia l'effetto sussulto/onda in loop continuo
        if (enableWaveEffect)
        {
            waveTween = textComponent.transform.DOPunchScale(Vector3.one * waveScalePunch, waveDuration, waveVibrato, waveElasticity)
                .SetLoops(-1, LoopType.Restart);
        }
    }

    /// <summary>
    /// Ferma le animazioni di caricamento e imposta il testo finale con un effetto "Pop" di impatto.
    /// </summary>
    public void PlayFinalText(string finalName)
    {
        StopAnimation();

        if (textComponent == null) return;

        textComponent.text = finalName;
        textComponent.transform.localScale = Vector3.one;

        // Effetto Pop finale sul nome della squadra assegnata
        textComponent.transform.DOPunchScale(Vector3.one * (waveScalePunch * 1.5f), 0.6f, 10, 1f);
    }

    /// <summary>
    /// Blocca e pulisce tutti i tween attivi in sicurezza.
    /// </summary>
    public void StopAnimation()
    {
        if (textTween != null && textTween.IsActive()) textTween.Kill();
        if (waveTween != null && waveTween.IsActive()) waveTween.Kill();

        if (textComponent != null)
        {
            textComponent.transform.localScale = Vector3.one;
        }
    }

    private void OnDestroy()
    {
        StopAnimation();
    }
}