using UnityEngine;

public class PlayAudioOnColliderEnter : MonoBehaviour
{
    [SerializeField] SoundType sound_type;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (AudioManager.Singleton == null) return;

        // PlaySFX userà PlayOneShot, così se avvengono 3 collisioni 
        // in un decimo di secondo, sentirai 3 suoni sovrapposti 
        // invece di uno solo che si interrompe!
        AudioManager.Singleton.PlaySFXFromTable(Sounds.PATH_MYSTIC_TABLE_SOUND, nameof(sound_type)); //nome enum deve corrispondere al nome dell'audio
    }
}
