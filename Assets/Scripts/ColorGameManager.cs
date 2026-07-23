using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class ColorGameManager : MonoBehaviour
{
    [Header("References")]
    public TimerManager timerManager;    // Riferimento al tuo TimerManager
    public Transform spawnPoint;         // Punto 2D da cui nascono i blocchi (World Space)
    public GameObject colorBlockPrefab;  // Prefab 2D del blocco (con SpriteRenderer)

    [Header("Game Settings")]
    public float gameDuration = 100f;     // Durata partita (100 secondi)
    public float initialSpawnRate = 1.5f; // Tempo tra un blocco e l'altro all'inizio
    public float baseFallSpeed = 5f;      // Velocità di caduta (in Unità Unity/sec)

    [Tooltip("La coordinata Y sotto la quale il blocco viene distrutto se non premuto in tempo")]
    public float destroyYThreshold = -6f; // Regola questo valore nell'Inspector!

    // I 4 colori di gioco (0: Alto, 1: Sinistra, 2: Basso, 3: Destra)
    public Color[] possibleColors = new Color[4] { Color.red, Color.blue, Color.green, Color.yellow };

    // Usiamo una List invece di Queue per poter ciclare e aggiornare facilmente le coordinate Z
    private List<GameObject> activeBlocks = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private float elapsedTime = 0f;
    private bool isGameOver = false;

    void Start()
    {
        if (timerManager == null)
        {
            Debug.LogError("TimerManager non assegnato nell'Inspector!");
            return;
        }

        timerManager.StartTimer(gameDuration, GameOver);
        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }

    void Update()
    {
        if (isGameOver) return;

        elapsedTime += Time.deltaTime;

        // Movimento dei blocchi
        float currentSpeed = baseFallSpeed + (elapsedTime * 0.1f);

        // Cicliamo al contrario per poter rimuovere/distruggere elementi durante lo scorrimento
        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            GameObject block = activeBlocks[i];

            if (block != null)
            {
                // Muovi verso il basso
                block.transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

                // CONTROLLO LIMITE INF. (Punto in cui sparisce il blocco)
                if (block.transform.position.y <= destroyYThreshold)
                {
                    activeBlocks.RemoveAt(i);
                    Destroy(block);

                    // Aggiorna gli ordini dei blocchi rimasti
                    RefreshSortingOrders();

                    Debug.Log("<color=orange>Blocco perso! Superato il limite.</color>");
                }
            }
        }

        // Input con il nuovo Input System
        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) CheckInput(0);
            if (kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) CheckInput(1);
            if (kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) CheckInput(2);
            if (kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) CheckInput(3);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (!isGameOver)
        {
            SpawnColorBlock();

            float currentSpawnRate = Mathf.Clamp(initialSpawnRate - (elapsedTime / 100f), 0.4f, initialSpawnRate);
            yield return new WaitForSeconds(currentSpawnRate);
        }
    }


    // Inside ColorGameManager...

    void SpawnColorBlock()
    {
        if (spawnPoint == null || colorBlockPrefab == null) return;

        GameObject newBlock = Instantiate(colorBlockPrefab, spawnPoint.position, Quaternion.identity);

        // Assegna il colore a MainColor
        int randomColorIndex = Random.Range(0, possibleColors.Length);
        Transform mainColorChild = newBlock.transform.Find("Border/MainColor");
        if (mainColorChild == null) mainColorChild = newBlock.transform.Find("MainColor");

        if (mainColorChild != null)
        {
            SpriteRenderer sr = mainColorChild.GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = possibleColors[randomColorIndex];
        }

        ColorData data = newBlock.AddComponent<ColorData>();
        data.colorIndex = randomColorIndex;

        activeBlocks.Add(newBlock);

        // Aggiorna gli ordini di tutti i blocchi
        RefreshSortingOrders();
    }

    /// <summary>
    /// Aggiorna l'Order in Layer dell'intero blocco usando il componente SortingGroup
    /// </summary>
    private void RefreshSortingOrders()
    {
        for (int i = 0; i < activeBlocks.Count; i++)
        {
            if (activeBlocks[i] == null) continue;

            // Recupera il SortingGroup sull'oggetto Padre
            SortingGroup group = activeBlocks[i].GetComponent<SortingGroup>();

            if (group != null)
            {
                // Il blocco in cima/più avanti (indice più alto o più basso) riceve il layer corretto.
                // Es. Il blocco in fondo ha order = 0, quello sopra order = 1, ecc.
                group.sortingOrder = i;
            }
        }
    }
    public void CheckInput(int colorIndexPressed)
    {
        if (isGameOver || activeBlocks.Count == 0) return;

        // Il primo blocco in lista (indice 0) è quello arrivato prima / più in basso
        GameObject targetBlock = activeBlocks[0];
        ColorData data = targetBlock.GetComponent<ColorData>();

        if (data != null && data.colorIndex == colorIndexPressed)
        {
            // Corretto! Rimuovi dalla lista e distruggi
            activeBlocks.RemoveAt(0);
            Destroy(targetBlock);

            // Ricalcola gli Order in Layer dei blocchi rimasti
            RefreshSortingOrders();

            Debug.Log("<color=green>CORRETTO!</color>");
        }
        else
        {
            Debug.Log("<color=red>ERRORE!</color>");
        }
    }


    void GameOver()
    {
        isGameOver = true;

        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
        }

        Debug.Log("Tempo Scaduto! Game Over.");
    }
}

public class ColorData : MonoBehaviour
{
    public int colorIndex;
}