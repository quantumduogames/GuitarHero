using UnityEngine;

public class PlayUIAudioOnClick : MonoBehaviour
{
    [SerializeField] string audio_name;
    public void PlayUIAudio()
    {
        if (AudioManager.Singleton != null)
        {
            AudioManager.Singleton.PlayUIFromTable(Sounds.PATH_UI_TABLE_SOUND, audio_name);
        }
    }

}
