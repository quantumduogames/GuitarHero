using System;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class TimerManager : MonoBehaviour
{
    [SerializeField] private TMP_Text timerText;
    private float currentTime;
    private Action savedCallback; // Salviamo la callback originale in caso di pausa
    private Coroutine timerCoroutine;

    public void UpdateTimerText(float time)
    {
        // Usiamo Mathf.Max per evitare che mostri valori negativi (es. -0.01) alla fine
        if (timerText != null)
        {
            timerText.text = Mathf.Max(0, time).ToString("F2");
        }
    }

    Coroutine stopCor;
    public void StopTimerFor(float timeToStop)
    {
        if (stopCor != null)
        {
            StopCoroutine(stopCor);
            stopCor = null;
        }

        stopCor = StartCoroutine(StopTimerAfterCoroutine(timeToStop));
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    public IEnumerator StopTimerAfterCoroutine(float timeToStop)
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }

        yield return new WaitForSeconds(timeToStop);

        PlayTimerAfterStop();
    }



    public void StartTimer(float duration, Action callback)
    {
        StopTimerFor(0);

        // Salviamo l'azione da compiere alla fine
        savedCallback = callback;
        currentTime = duration;

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