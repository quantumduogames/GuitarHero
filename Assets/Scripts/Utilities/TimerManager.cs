using System;
using System.Collections;
using System.Globalization;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private float currentTime;
    private Action savedCallback; // Salviamo la callback originale in caso di pausa
    private Coroutine timerCoroutine;
    private Coroutine stopwatchCoroutine;

    public float ElapsedTime { get; private set; }

    /// <summary>
    /// Avvia un cronometro crescente usato dalla modalità a numero fisso di tile.
    /// </summary>
    public void StartStopwatch()
    {
        StopStopwatch();
        ElapsedTime = 0f;
        UpdateStopwatchText();
        stopwatchCoroutine = StartCoroutine(RunStopwatch());
    }

    /// <summary>
    /// Ferma il cronometro e restituisce il tempo impiegato in secondi.
    /// </summary>
    public float StopStopwatch()
    {
        if (stopwatchCoroutine != null)
        {
            StopCoroutine(stopwatchCoroutine);
            stopwatchCoroutine = null;
        }

        UpdateStopwatchText();
        return ElapsedTime;
    }

    private IEnumerator RunStopwatch()
    {
        while (true)
        {
            ElapsedTime += Time.deltaTime;
            UpdateStopwatchText();
            yield return null;
        }
    }

    private void UpdateStopwatchText()
    {
        if (timerText != null)
        {
            string updateText = ElapsedTime.ToString("F2", CultureInfo.InvariantCulture);
            timerText.text = updateText;
        }
    }

    public void UpdateTimerText(float time)
    {
        // Usiamo Mathf.Max per evitare che mostri valori negativi (es. -0.01) alla fine
        if (timerText != null)
        {
            timerText.text = Mathf.Max(0, time).ToString("F2", CultureInfo.InvariantCulture);
        }
    }

    private Coroutine stopCoroutine;

    public void StopTimerFor(float timeToStop)
    {
        StopTimer();

        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
        }

        stopCoroutine = StartCoroutine(ResumeTimerAfterDelay(timeToStop));
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator ResumeTimerAfterDelay(float timeToStop)
    {
        yield return new WaitForSeconds(timeToStop);
        stopCoroutine = null;
        PlayTimerAfterStop();
    }



    public void StartTimer(float duration, Action callback)
    {
        StopTimer();

        if (stopCoroutine != null)
        {
            StopCoroutine(stopCoroutine);
            stopCoroutine = null;
        }

        savedCallback = callback;
        currentTime = Mathf.Max(0, duration);

        timerCoroutine = StartCoroutine(RunTimer());
    }

    private IEnumerator RunTimer()
    {
        while (currentTime > 0)
        {
            UpdateTimerText(currentTime);
            currentTime -= Time.deltaTime;
            yield return null;
        }

        // Forza il testo a 0.00 alla fine
        UpdateTimerText(0);

        // Riferimento locale temporaneo per evitare problemi se resettato
        Action finishedAction = savedCallback;
        savedCallback = null;
        timerCoroutine = null;

        finishedAction?.Invoke();
    }

    // Corretto il nome e ora passa la callback originale salvata!
    public void PlayTimerAfterStop()
    {
        if (currentTime > 0 && timerCoroutine == null)
        {
            timerCoroutine = StartCoroutine(RunTimer());
        }
    }
}
