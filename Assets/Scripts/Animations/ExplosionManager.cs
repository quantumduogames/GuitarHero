using UnityEngine;

public class ExplosionManager : MonoBehaviour
{
    public static ExplosionManager Instance { get; private set; }

    [Header("Prefab dell'esplosione")]
    [SerializeField] private JuicyExplosion explosionPrefab; // Trascina il Prefab qui dall'Inspector!

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SpawnJuicyExplosion(Vector3 position, Color color)
    {
        if (explosionPrefab == null)
        {
            Debug.LogWarning("Prefab dell'esplosione non assegnato nell'ExplosionManager!");
            return;
        }

        // Istanzia il prefab ed esegui la sua logica interna
        JuicyExplosion explosionInstance = Instantiate(explosionPrefab, position, Quaternion.identity);
        explosionInstance.Play(position, color);
    }
}