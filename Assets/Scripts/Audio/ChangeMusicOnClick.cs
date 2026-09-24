using UnityEngine;

public class ChangeMusicOnClick : MonoBehaviour
{
    [SerializeField] string audio_name;
    public void PlayMusic()
    {
        if (AudioManager.Singleton != null)
        {
            AudioManager.Singleton.PlayMusicFromTable(Sounds.PATH_MUSIC_TABLE_SOUND, audio_name);
        }
    }

    public void RemoveMusic()
    {
        if (AudioManager.Singleton != null)
        {
            AudioManager.Singleton.StopMusic();
        }
    }

}
