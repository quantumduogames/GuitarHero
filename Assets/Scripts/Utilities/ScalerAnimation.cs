using UnityEngine;

public class ScalerAnimation : MonoBehaviour
{
    [Header("Scale Settings")]
    public float minScale = 0.8f;       // Minimum scale value
    public float maxScale = 1.2f;       // Maximum scale value
    public float duration = 1.5f;       // Time to go from min to max
    public bool loop = true;            // If true, keep pulsing

    [Header("Transition Settings")]
    public bool useSmoothDamp = false;  // If true, use SmoothDamp for transitions
    public AnimationCurve customCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    // Custom curve editable in Inspector (like Animator curves)

    private Vector3 originalScale;      // Initial object scale
    private float timer = 0f;           // Timer for animation
    private bool scalingUp = true;      // Direction of scaling
    private float velocity = 0f;        // Used by SmoothDamp

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        if (!loop) return;

        // Update timer
        timer += Time.deltaTime / duration;
        if (timer >= 1f)
        {
            timer = 0f;
            scalingUp = !scalingUp; // Switch direction
        }

        // Determine progress (0  1)
        float progress = scalingUp ? timer : 1f - timer;

        float scaleValue;

        if (useSmoothDamp)
        {
            // SmoothDamp creates fluid transitions with natural deceleration
            float target = Mathf.Lerp(minScale, maxScale, progress);
            scaleValue = Mathf.SmoothDamp(transform.localScale.x / originalScale.x, target, ref velocity, 0.2f);
        }
        else
        {
            // Use custom AnimationCurve for fine-tuned easing
            float curved = customCurve.Evaluate(progress);
            scaleValue = Mathf.Lerp(minScale, maxScale, curved);
        }

        // Apply scale
        transform.localScale = originalScale * scaleValue;
    }
}
