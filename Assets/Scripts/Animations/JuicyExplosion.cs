using UnityEngine;
using DG.Tweening;

public class JuicyExplosion : MonoBehaviour
{
    [SerializeField] private ParticleSystem ps;

    public void Play(Vector3 position, Color color)
    {
        transform.position = position;

        // 1. Applica colore e avvia particelle
        var main = ps.main;
        main.startColor = color;
        ps.Play();

        // 2. Juice extra: Animazione di scala/pop immediata sul centro dell'esplosione
        transform.localScale = Vector3.one * 0.5f;
        transform.DOScale(1.2f, 0.08f).SetEase(Ease.OutBack).OnComplete(() =>
        {
            transform.DOScale(1.0f, 0.08f);
        });

        // 3. Camera Shake
        if (Camera.main != null)
        {
            Camera.main.transform.DOComplete();
            Camera.main.transform.DOShakePosition(0.12f, 0.15f, 25);
        }

        // 4. Autodistruzione quando la particella finisce
        float totalDuration = main.duration + main.startLifetime.constantMax;
        Destroy(gameObject, totalDuration);
    }
}