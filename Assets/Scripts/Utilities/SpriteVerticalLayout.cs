using UnityEngine;

[ExecuteAlways] // Funziona in tempo reale nell'Editor senza fare Play!
public class SpriteVerticalLayout : MonoBehaviour
{
    [Tooltip("Distanza verticale tra il centro di uno sprite e l'altro")]
    public float spacing = 1.5f;

    [Tooltip("Se vero, li incolonna verso l'alto (Y+), altrimenti verso il basso (Y-)")]
    public bool invert = false;

    private void Update()
    {
        // Ricalcola la posizione dei figli
        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            Transform child = transform.GetChild(i);

            float direction = invert ? 1f : -1f;
            float targetY = i * spacing * direction;

            // Mantiene intatti X e Z locali, aggiorna solo Y
            child.localPosition = new Vector3(child.localPosition.x, targetY, child.localPosition.z);
        }
    }
}