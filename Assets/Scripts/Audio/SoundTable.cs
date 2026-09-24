using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

[System.Serializable]
public struct SoundEntry
{
    public string id;       // Nome identificativo (es. "click", "back")
    public AudioClip clip;  // Il file audio
    [Range(-1f, 5f)] public float volumeMultiplier; // Per bilanciare i suoni tra loro
    public bool isLoop; // Per bilanciare i suoni tra loro
    public float delay; // Per bilanciare i suoni tra loro
}

[CreateAssetMenu(fileName = "NewSoundsTable", menuName = "Audio/Your new fantastic sound table!")]
public class SoundTable : ScriptableObject
{
    public List<SoundEntry> entries = new List<SoundEntry>();

    // Metodo per cercare il clip velocemente nella tabella
    public SoundEntry GetAudioClip(string id)
    {
        int index = entries.FindIndex(e => !string.IsNullOrEmpty(e.id) && e.id.Equals(id, System.StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            Debug.LogWarning($"[SoundTable] Audio id '{id}' non trovato nella tabella '{name}'.");
            return default;
        }

        var entry = entries[index];
        if (entry.clip == null)
        {
            Debug.LogWarning($"[SoundTable] Audio id '{id}' trovato nella tabella '{name}', ma clip null.");
            return default;
        }

        if (entry.volumeMultiplier <= 0f)
        {
            entry.volumeMultiplier = 1f;
        }

        return entry;
    }
}
