using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Singleton { get; private set; }

    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    public Camera camera;

    private void Awake()
    {
        Singleton = this;
        StartCoroutine(WaitUntilCameraIsSetWithBoard());
    }

    [ContextMenu("Esegui Test Shake")]
    public void TestShake()
    {
        ShakeCamera(1, 1.2f);
    }
    IEnumerator WaitUntilCameraIsSetWithBoard()
    {
        yield return new WaitForEndOfFrame();
        // Salva la posizione originale
        originalPos = camera.transform.localPosition;
    }

    public void ShakeCamera(float duration, float magnitude)
    {
        // Se uno shake � gi� attivo, stoppiamolo
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);


        // Avvia lo shake
        shakeRoutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            Vector3 shakeOffset = new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0f
            );

            camera.transform.localPosition = originalPos + shakeOffset;

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ripristina la posizione
        camera.transform.localPosition = originalPos;
        shakeRoutine = null;
    }
}

