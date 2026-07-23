using UnityEngine;
using UnityEngine.InputSystem; // Importiamo il nuovo Input System!

public class TimerTester : MonoBehaviour
{
    [SerializeField] private TimerManager timerManager;
    [SerializeField] private float testDuration = 10f;

    void Start()
    {
        if (timerManager == null)
            timerManager = GetComponent<TimerManager>();

        // Facciamo partire il timer all'avvio con un'azione di prova alla fine
        timerManager.StartTimer(testDuration, () =>
        {
            Debug.Log("TEMPO SCADUTO! Azione eseguita con successo.");
        });
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        // Premi SPAZIO per stoppare il timer
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            timerManager.StopTimerFor(0);
            Debug.Log("Timer Pausato!");
        }

        // Premi INVIO (della tastiera principale o del tastierino) per farlo ripartire
        if (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame)
        {
            timerManager.PlayTimerAfterStop();
            Debug.Log("Timer Ripreso!");
        }
    }
}