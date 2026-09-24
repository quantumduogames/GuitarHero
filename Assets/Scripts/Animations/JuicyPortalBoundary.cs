using UnityEngine;
using UnityEngine.UI; // Necessario per il componente Image
using DG.Tweening;

public class JuicyPortalBoundary : MonoBehaviour
{
    [Header("UI Reference")]
    [SerializeField] private Image portalImage;

    [Header("Settings")]
    [SerializeField] private float rotationSpeed = 3f; // Secondi per un giro di 360 gradi

    private Color originalColor;

    public static JuicyPortalBoundary Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
        
        // Recupera l'Image se non è stata assegnata da Inspector
        if (portalImage == null)
            portalImage = GetComponent<Image>();

        if (portalImage != null)
            originalColor = portalImage.color;
    }

    private void Start()
    {
        // 1. Rotazione continua dell'Image a 360 gradi
        transform.DORotate(new Vector3(0, 0, -360), rotationSpeed, RotateMode.FastBeyond360)
                 .SetLoops(-1, LoopType.Incremental)
                 .SetEase(Ease.Linear);

        // 2. Pulsazione fluida di scala (effetto respiro)
        transform.DOScale(transform.localScale * 1.05f, 0.8f)
                 .SetLoops(-1, LoopType.Yoyo)
                 .SetEase(Ease.InOutSine);
    }

    /// <summary>
    /// Richiama questo metodo quando un blocco scende e colpisce la barriera!
    /// </summary>
    public void OnImpact()
    {
        // Interrompe le tween attive per eseguire il rimbalzo subito
        transform.DOKill(true);

        // A) Effetto rimbalzo/sussulto di scala al contatto
        transform.DOPunchScale(Vector3.one * 0.3f, 0.2f, 12, 1);

        // B) Flash di colore rapido sull'Image (Bianco -> Rosso -> Colore Originale)
        if (portalImage != null)
        {
            portalImage.DOKill();
            Sequence colorSeq = DOTween.Sequence();
            colorSeq.Append(portalImage.DOColor(Color.white, 0.05f));
            colorSeq.Append(portalImage.DOColor(Color.red, 0.1f));
            colorSeq.Append(portalImage.DOColor(originalColor, 0.25f));
        }
    }
}