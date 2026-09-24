using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Globalization;
using System;

public class MinigameManager : MonoBehaviour
{
    [Header("References")]
    public TimerManager timerManager;
    public Transform spawnPoint;

    [Header("Game Settings")]
    public int targetTiles = 100;
    public float initialSpawnRate = 1.5f;
    public float baseFallSpeed = 5f;
    public float destroyYThreshold = -6f;

    [Header("Visual Limit Line")]
    public bool showLimitLine = true;
    public Color limitLineColor = Color.red;
    public float limitLineWidth = 0.1f;
    public float limitLineLength = 10f;

    private LineRenderer lineRenderer;

    [Header("Color Buttons: Upper - Left - Right - Bottom"), SerializeField]
    public Dictionary<Button, int> colorButtons = new Dictionary<Button, int>(); //tasto e ID assegnato
    public Button[] buttonArray; // Array di bottoni da assegnare nell'Inspector

    [Header("Score & UI")]
    [SerializeField] private TMP_Text recordText;
    [SerializeField] private TMP_Text tilesCountScore;
    [SerializeField] GameObject gameButton, gameHeader;

    [Header("Countdown UI")]
    [SerializeField] private GameObject counterPanel; // Il pannello 'CounterPnl'
    [SerializeField] private TMP_Text txtCount;       // Il testo 'txtCount'

    private const string RecordKeyPrefix = "ColorGameBestTime_";

    private List<GameObject> activeBlocks = new List<GameObject>();
    private Coroutine spawnCoroutine;
    private float elapsedTime = 0f;
    private int spawnedTiles = 0;
    private int clickedTiles = 0;
    private bool isGameOver = false;

    [Header("Prefabs Pronti")]
    [Tooltip("Inserisci qui i DTO contenenti i Prefab già pronti da far spawnare")]
    public ExplodingObj[] availableTiles;
    public ExplodingObj[] currentGameTiles;

    public void StartGame()
    {
        spawnedTiles = 0;
        clickedTiles = 0;

        if (showLimitLine) CreateLimitLine();

        UpdateRemainingTilesText();
        UpdateRecordText(GetBestTime());
        CreateSetForCurrentGame();

        if (timerManager == null)
        {
            Debug.LogError("TimerManager non assegnato!");
            return;
        }

        // Attiva l'interfaccia di gioco
        if (gameButton != null) gameButton.SetActive(true);
        if (gameHeader != null) gameHeader.SetActive(true);

        // Ferma eventuali routine attive e avvia il conto alla rovescia
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);
        StartCoroutine(CountdownRoutine());
    }

    /// <summary>
    /// Resetta completamente lo stato del gioco e avvia una nuova partita da zero.
    /// </summary>
    public void RestartGame()
    {
        gameButton.SetActive(true);
        gameHeader.SetActive(true);

        // 1. Ferma eventuali Coroutine di spawn ancora attive
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }

        // 2. Distruggi tutti i blocchi ancora presenti in scena
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                Destroy(block);
            }
        }
        activeBlocks.Clear();

        // 3. Resetta lo stato di GameOver e i contatori di gioco
        isGameOver = false;
        elapsedTime = 0f;
        spawnedTiles = 0;
        clickedTiles = 0;

        // 4. Resetta e riavvia il TimerManager
        if (timerManager != null)
        {
            timerManager.StopStopwatch(); // Resetta lo stopwatch esistente
        }

        // 5. Fa ripartire la sessione fresca
        StartGame();
    }

    private void CreateLimitLine()
    {
        GameObject lineObj = new GameObject("LimitLine");
        lineObj.transform.SetParent(transform);

        lineRenderer = lineObj.AddComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));

        // 1. Spessore della linea
        float lineWidth = 0.12f;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        // 2. Prendi il canale Alpha impostato nell'Inspector
        float inspectorAlpha = limitLineColor.a;

        // 3. Applica il colore dell'Inspector mantenendo la sfumatura ai bordi
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
            new GradientColorKey(limitLineColor, 0.0f),
            new GradientColorKey(limitLineColor, 0.5f),
            new GradientColorKey(limitLineColor, 1.0f)
            },
            new GradientAlphaKey[] {
            new GradientAlphaKey(inspectorAlpha * 0.2f, 0.0f), // Ai bordi prende il 20% dell'Alpha dell'Inspector
            new GradientAlphaKey(inspectorAlpha, 0.2f),        // Al centro prende il 100% dell'Alpha dell'Inspector
            new GradientAlphaKey(inspectorAlpha, 0.8f),
            new GradientAlphaKey(inspectorAlpha * 0.2f, 1.0f)
            }
        );
        lineRenderer.colorGradient = gradient;

        float halfWidth = limitLineLength / 2f;
        lineRenderer.SetPosition(0, new Vector3(-halfWidth, destroyYThreshold, 0));
        lineRenderer.SetPosition(1, new Vector3(halfWidth, destroyYThreshold, 0));
    }
    void CreateSetForCurrentGame()
    {
        if (availableTiles == null || availableTiles.Length == 0)
        {
            Debug.LogError("Nessun tile disponibile per il gioco!");
            return;
        }

        List<ExplodingObj> selected = new List<ExplodingObj>();

        // 2. Seleziona un numero di tile unici casuali
        int tilesToSelect = Mathf.Min(4, availableTiles.Length); // Limitiamo a 4 per i bottoni
        while (selected.Count < tilesToSelect)
        {
            ExplodingObj randomTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Length)];
            if (!selected.Contains(randomTile))
            {
                selected.Add(randomTile);
            }
        }
        // 4. Aggiorna i bottoni con i nuovi tile selezionati

        currentGameTiles = selected.ToArray();
        ConfigureColorAndIDButtons(selected);
    }

    private void ConfigureColorAndIDButtons(List<ExplodingObj> availableObj = null)
    {
        if (buttonArray == null) return;

        colorButtons.Clear();

        for (int index = 0; index < buttonArray.Length; index++)
        {
            Button button = buttonArray[index];
            if (button == null) continue;

            // 1. Rimuovi listener precedenti
            button.onClick.RemoveAllListeners();

            if (availableObj != null && index < availableObj.Count)
            {
                ExplodingObj tileData = availableObj[index];
                int assignedID = tileData.colorData.ID; // Copia locale pulita per la closure

                // 2. Salva nel Dictionary la coppia (Button -> ID)
                colorButtons[button] = assignedID;

                // 3. Gestisci la grafica del bottone
                Image buttonImage = button.targetGraphic as Image ?? button.GetComponent<Image>();
                if (buttonImage != null)
                {
                    if (tileData.customButtonSprite != null)
                    {
                        buttonImage.sprite = tileData.customButtonSprite;
                        buttonImage.color = Color.white;
                    }
                    else
                    {
                        buttonImage.color = tileData.colorData.color;
                    }
                }

                // 4. Assegna il listener passantogli una variabile immutabile
                int finalID = assignedID;
                button.onClick.AddListener(() => CheckInput(finalID));
                button.interactable = true;
            }
            else
            {
                button.interactable = false;
            }
        }
    }

    void Update()
    {
        if (isGameOver) return;

        elapsedTime += Time.deltaTime;
        float currentSpeed = baseFallSpeed;

        for (int i = activeBlocks.Count - 1; i >= 0; i--)
        {
            GameObject block = activeBlocks[i];

            if (block != null)
            {
                block.transform.Translate(Vector3.down * currentSpeed * Time.deltaTime);

                if (block.transform.position.y <= destroyYThreshold)
                {
                    activeBlocks.RemoveAt(i);
                    Destroy(block);

                    JuicyPortalBoundary.Instance.OnImpact();

                    RefreshSortingOrders();
                    Debug.Log("<color=orange>Blocco perso!</color>");
                }
            }
        }

        Keyboard kb = Keyboard.current;
        if (kb != null)
        {
            if ((kb.wKey.wasPressedThisFrame || kb.upArrowKey.wasPressedThisFrame) && buttonArray.Length > 0 && colorButtons.ContainsKey(buttonArray[0]))
                CheckInput(colorButtons[buttonArray[0]]);

            if ((kb.aKey.wasPressedThisFrame || kb.leftArrowKey.wasPressedThisFrame) && buttonArray.Length > 1 && colorButtons.ContainsKey(buttonArray[1]))
                CheckInput(colorButtons[buttonArray[1]]);

            if ((kb.dKey.wasPressedThisFrame || kb.rightArrowKey.wasPressedThisFrame) && buttonArray.Length > 2 && colorButtons.ContainsKey(buttonArray[2]))
                CheckInput(colorButtons[buttonArray[2]]);

            if ((kb.sKey.wasPressedThisFrame || kb.downArrowKey.wasPressedThisFrame) && buttonArray.Length > 3 && colorButtons.ContainsKey(buttonArray[3]))
                CheckInput(colorButtons[buttonArray[3]]);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (!isGameOver)
        {
            SpawnTileBlock();
            yield return new WaitForSeconds(initialSpawnRate);
        }
    }

    void SpawnTileBlock()
    {
        if (spawnPoint == null || currentGameTiles == null || currentGameTiles.Length == 0) return;

        // 1. Pesca una SingleTile a caso
        ExplodingObj selectedTile = currentGameTiles[UnityEngine.Random.Range(0, currentGameTiles.Length)];
        if (selectedTile.tilePrefab == null) return;

        // 2. ISTANZIA DIRETTAMENTE IL PREFAB PRONTO
        GameObject newBlock = Instantiate(selectedTile.tilePrefab, spawnPoint.position, Quaternion.identity);
        spawnedTiles++;

        // 3. Salva/Assicura i dati runtime sul blocco appena nato
        ColorDataRuntime runtimeData = newBlock.GetComponent<ColorDataRuntime>();
        if (runtimeData == null) runtimeData = newBlock.AddComponent<ColorDataRuntime>();

        runtimeData.colorIndex = selectedTile.colorData.ID;
        runtimeData.color = selectedTile.colorData.color;

        // 4. Aggiungi alla lista e ordina i layer
        activeBlocks.Add(newBlock);
        RefreshSortingOrders();
    }
    private void RefreshSortingOrders()
    {
        for (int i = 0; i < activeBlocks.Count; i++)
        {
            if (activeBlocks[i] == null) continue;

            // Calcola l'ordine: i blocchi più recenti/in alto stanno sotto, 
            // quelli più in basso (i=0) hanno l'ordine più alto e stanno in primo piano
            int orderValue = activeBlocks.Count - i;

            // 1. Aggiorna il SortingGroup sulla root se presente
            SortingGroup group = activeBlocks[i].GetComponent<SortingGroup>();
            if (group != null)
            {
                group.sortingOrder = orderValue;
            }

            // 2. Per sicurezza, aggiorna anche tutti gli SpriteRenderer dell'oggetto e dei suoi figli
            SpriteRenderer[] renderers = activeBlocks[i].GetComponentsInChildren<SpriteRenderer>();
            foreach (SpriteRenderer sr in renderers)
            {
                sr.sortingOrder = orderValue;
            }
        }
    }

    public void CheckInput(int colorIndexPressed)
    {
        Debug.Log($"Input ricevuto: {colorIndexPressed}");
        if (isGameOver || activeBlocks.Count == 0 || colorIndexPressed < 0) return;

        GameObject targetBlock = activeBlocks[0];
        ColorDataRuntime data = targetBlock.GetComponent<ColorDataRuntime>();

        if (data != null && data.colorIndex == colorIndexPressed)
        {
            // Genera l'esplosione indipendente tramite il Manager
            if (ExplosionManager.Instance != null)
            {
                ExplosionManager.Instance.SpawnJuicyExplosion(targetBlock.transform.position, data.color);
            }

            // Distruggi subito il blocco colpito
            activeBlocks.RemoveAt(0);
            Destroy(targetBlock);

            clickedTiles++;
            UpdateRemainingTilesText();
            RefreshSortingOrders();

            if (clickedTiles >= targetTiles)
            {
                CompleteGame();
            }
        }
        else
        {
            Debug.Log("<color=red>ERRORE DI COLORE!</color>");
        }
    }

    private void UpdateRemainingTilesText()
    {
        if (tilesCountScore != null)
        {
            tilesCountScore.text = $"{Mathf.Max(0, targetTiles - clickedTiles)}";
        }
    }

    private string RecordKey => RecordKeyPrefix + targetTiles;

    private float GetBestTime()
    {
        float bestTime = ScoreManager.Instance != null
        ? ScoreManager.Instance.GetBestTime(targetTiles)
        : float.MaxValue;

        return bestTime;
    }

    private void UpdateRecordText(float recordTime)
    {
        if (recordText != null)
        {
            string recordTextCoverted = recordTime.ToString("F2", CultureInfo.InvariantCulture);
            recordText.text = recordTime == float.MaxValue ? "99:99" : $"{recordTextCoverted}";
        }
    }


    public static Action<float> ? OnGameCompleted;
    private void CompleteGame()
    {
        isGameOver = true;
        if (spawnCoroutine != null) StopCoroutine(spawnCoroutine);

        float completedTime = timerManager != null ? timerManager.StopStopwatch() : elapsedTime;

        FinalExplosion();

        // Salva il tempo usando lo ScoreManager
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddScore(targetTiles, completedTime);
        }

        UpdateRecordText(GetBestTime());

        // Notifica la UI che il gioco è finito passando il tempo impiegato
        OnGameCompleted?.Invoke(completedTime);
    }

    void FinalExplosion()
    {
        foreach (GameObject block in activeBlocks)
        {
            if (block != null)
            {
                ColorDataRuntime data = block.GetComponent<ColorDataRuntime>();
                if (data != null && ExplosionManager.Instance != null)
                {
                    ExplosionManager.Instance.SpawnJuicyExplosion(block.transform.position, data.color);
                }
                Destroy(block);
            }
        }
        activeBlocks.Clear();
    }

    /// <summary>
    /// Coroutine che gestisce il conto alla rovescia 3.. 2.. 1.. GO!
    /// </summary>
    private IEnumerator CountdownRoutine()
    {
        // 1. Mostra il pannello del conto alla rovescia
        if (counterPanel != null) counterPanel.SetActive(true);

        // 3
        if (txtCount != null) txtCount.text = "03";
        yield return new WaitForSeconds(1f);

        // 2
        if (txtCount != null) txtCount.text = "02";
        yield return new WaitForSeconds(1f);

        // 1
        if (txtCount != null) txtCount.text = "01";
        yield return new WaitForSeconds(1f);

        // GO!
        if (txtCount != null) txtCount.text = "GO!";
        yield return new WaitForSeconds(0.5f); // Resta visibile per mezzo secondo

        // 2. Nascondi il pannello
        if (counterPanel != null) counterPanel.SetActive(false);

        // 3. FAI PARTIRE IL GIOCO VERO E PROPRIO
        if (timerManager != null)
        {
            timerManager.StartStopwatch();
        }

        spawnCoroutine = StartCoroutine(SpawnRoutine());
    }
}


/// <summary>
/// Componente MonoBehaviour applicato a runtime per identificare il colore del blocco spawned.
/// </summary>
public class ColorDataRuntime : MonoBehaviour
{
    public int colorIndex;
    public Color color;
}

