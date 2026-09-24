using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using static UnityEngine.EventSystems.EventTrigger;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Singleton;

    [Header("Runtime Config")]
    [Min(1)] public int minimum_sfx_sources = 3;

    [Header("Audio Sources")]
    public AudioSource music_source;
    public AudioSource ui_source;
    public AudioSource[] sfx_sources;

    [Header("Volumes")]
    [Range(0f, 1f)] public float music_volume = 1f;
    [Range(0f, 1f)] public float ui_volume = 1f;
    [Range(0f, 1f)] public float sfx_volume = 1f;

    private int sfx_index = 0;

    private void Awake()
    {
        if (Singleton != null && Singleton != this)
        {
            // If this scene was authored with a larger SFX pool, adapt the persistent manager.
            int desiredPoolSize = sfx_sources != null ? sfx_sources.Length : 0;
            Singleton.EnsureSFXPoolSize(desiredPoolSize);
            Destroy(gameObject); // delete duplicate if any
            return;
        }
        Singleton = this;

        EnsureSFXPoolSize();

        music_source.loop = true;
        ApplyVolumes();
    }

    private void EnsureSFXPoolSize(int desiredSize = -1)
    {
        int targetSize = desiredSize > 0 ? desiredSize : minimum_sfx_sources;
        if (targetSize < 1) targetSize = 1;

        if (sfx_sources == null)
            sfx_sources = new AudioSource[0];

        List<AudioSource> validSources = new List<AudioSource>(sfx_sources.Length);
        for (int i = 0; i < sfx_sources.Length; i++)
        {
            if (sfx_sources[i] != null)
                validSources.Add(sfx_sources[i]);
        }

        AudioSource template = null;
        if (validSources.Count > 0)
            template = validSources[0];

        while (validSources.Count < targetSize)
        {
            GameObject poolObj = new GameObject($"SFXAudio_Auto_{validSources.Count + 1}");
            poolObj.transform.SetParent(transform, false);
            AudioSource newSource = poolObj.AddComponent<AudioSource>();

            if (template != null)
            {
                newSource.outputAudioMixerGroup = template.outputAudioMixerGroup;
                newSource.spatialBlend = template.spatialBlend;
                newSource.priority = template.priority;
                newSource.dopplerLevel = template.dopplerLevel;
                newSource.minDistance = template.minDistance;
                newSource.maxDistance = template.maxDistance;
                newSource.rolloffMode = template.rolloffMode;
                newSource.bypassEffects = template.bypassEffects;
                newSource.bypassListenerEffects = template.bypassListenerEffects;
                newSource.bypassReverbZones = template.bypassReverbZones;
            }

            newSource.playOnAwake = false;
            newSource.loop = false;
            newSource.volume = sfx_volume;
            validSources.Add(newSource);
        }

        sfx_sources = validSources.ToArray();
    }

    private void ApplyVolumes()
    {
        music_source.volume = music_volume;
        ui_source.volume = ui_volume;
        foreach (var src in sfx_sources)
            src.volume = sfx_volume;
    }


    // All'interno di AudioManager.cs
    [Header("Tables")]
    public List<SoundTable> activeTables = new List<SoundTable>();

    #region MODULAR PLAY METHODS

    public void PlayMusicFromTable(string tablePath, string soundId, float fadeTime = 0.5f)
    {
        SoundTable table = Sounds.GetTable(tablePath);
        if (table != null)
        {
            SoundEntry entry = table.GetAudioClip(soundId);
            if (entry.clip != null)
            {
                // Richiama il tuo metodo PlayMusic con il fade
                PlayMusic(entry.clip, fadeTime);
            }
        }
    }

    public void StopMusic()
    {
        if (music_source != null)
        {
            music_source.Stop();       // Ferma la riproduzione attuale
            music_source.clip = null;   // Rimuove il riferimento all'AudioClip
        }
    }

    // Per la UI
    public void PlayUIFromTable(string tablePath, string soundId)
    {
        SoundTable table = Sounds.GetTable(tablePath);
        if (table != null)
        {
            SoundEntry entry = table.GetAudioClip(soundId);
            if (entry.clip != null)
            {
                // Richiama il tuo metodo PlayUI
                // Moltiplica il volume UI per il multiplier della clip
                ui_source.PlayOneShot(entry.clip, ui_volume * entry.volumeMultiplier);
            }
        }
    }
    IEnumerator PlaySFXWithDelay(SoundEntry entry, bool loop, float delay)
    {
        yield return new WaitForSeconds(delay);

        PlaySFX_Internal(entry.clip, entry.volumeMultiplier, loop);
    }
    // Per gli SFX (usa il tuo sistema di pooling)
    public void PlaySFXFromTable(string tablePath, string soundId)
    {
        SoundTable table = Sounds.GetTable(tablePath);
        if (table != null)
        {
            SoundEntry entry = table.GetAudioClip(soundId);
            if (entry.clip != null)
            {
                if (entry.delay > 0)
                {
                    StartCoroutine(PlaySFXWithDelay(entry, entry.isLoop, entry.delay));
                }
                else
                {
                    PlaySFX_Internal(entry.clip, entry.volumeMultiplier, entry.isLoop);
                }
            }
        }
    }

    private void PlaySFX_Internal(AudioClip clip, float multiplier, bool loop)
    {
        if (clip == null || sfx_sources.Length == 0) return;

        for (int i = 0; i < sfx_sources.Length; i++)
        {
            int index = (sfx_index + i) % sfx_sources.Length;
            AudioSource src = sfx_sources[index];

            if (!src.isPlaying)
            {
                sfx_index = (index + 1) % sfx_sources.Length;

                src.clip = clip;
                src.volume = sfx_volume * multiplier;
                src.loop = loop;
                src.Play();

                return;
            }
        }

        AudioSource fallback = sfx_sources[sfx_index];
        sfx_index = (sfx_index + 1) % sfx_sources.Length;

        fallback.Stop();
        fallback.clip = clip;
        fallback.volume = sfx_volume * multiplier;
        fallback.loop = loop;
        fallback.Play();
    }
    public void StopLoopSFXFromTable(string tablePath, string soundId)
    {
        SoundTable table = Sounds.GetTable(tablePath);
        if (table != null)
        {
            SoundEntry audio_to_stop = table.GetAudioClip(soundId);
            if (audio_to_stop.clip != null)
            {
                StopLoopSFX(audio_to_stop.clip);
            }
        }
    }
    public void StopLoopSFX(AudioClip clip)
    {
        foreach (AudioSource src in sfx_sources)
        {
            if (src.clip == clip)
            {
                src.Stop();
                src.loop = false;
                src.clip = null;
            }
        }
    }

    #endregion

    #region MUSIC

    Coroutine music_coroutine;

    public void PlayMusic(AudioClip clip, float fadeTime = 0.5f)
    {
        if (clip == null || music_source.clip == clip) return;

        if (music_coroutine != null)
        {
            StopCoroutine(music_coroutine);
        }

        music_coroutine = StartCoroutine(FadeMusic(clip, fadeTime));
    }

    private IEnumerator FadeMusic(AudioClip newClip, float time)
    {
        float t = 0f;
        float startVolume = music_source.volume;

        // Fade out
        while (t < time)
        {
            t += Time.deltaTime;
            music_source.volume = Mathf.Lerp(startVolume, 0f, t / time);
            yield return null;
        }

        music_source.clip = newClip;
        music_source.Play();

        // Fade in
        t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            music_source.volume = Mathf.Lerp(0f, music_volume, t / time);
            yield return null;
        }
    }

    public void SetMusicVolume(float value)
    {
        music_volume = Mathf.Clamp01(value);
        music_source.volume = music_volume;
    }
    #endregion

    #region UI SFX
    public void PlayUI(AudioClip clip)
    {
        if (clip == null) return;
        ui_source.PlayOneShot(clip, ui_volume);
    }

    public void SetUIVolume(float value)
    {
        ui_volume = Mathf.Clamp01(value);
        ui_source.volume = ui_volume;
    }
    #endregion

    #region GAME SFX
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfx_sources.Length == 0) return;

        for (int i = 0; i < sfx_sources.Length; i++)
        {
            int index = (sfx_index + i) % sfx_sources.Length;
            AudioSource src = sfx_sources[index];

            if (!src.isPlaying)
            {
                sfx_index = (index + 1) % sfx_sources.Length;
                src.PlayOneShot(clip, sfx_volume);
                return;
            }
        }

        AudioSource fallback = sfx_sources[sfx_index];
        sfx_index = (sfx_index + 1) % sfx_sources.Length;
        fallback.PlayOneShot(clip, sfx_volume);
    }

    public void SetSFXVolume(float value)
    {
        sfx_volume = Mathf.Clamp01(value);
        foreach (var src in sfx_sources)
            src.volume = sfx_volume;
    }
    #endregion

    #region MUTE AUDIO
    public AudioMixer mainMixer;
    private bool _isMusicMuted, _isSFXMuted, _isUIMuted;

    public bool GetIsMusicMuted() => _isMusicMuted;
    public bool GetIsUIMuted() => _isUIMuted;
    public bool GetIsSFXMuted() => _isSFXMuted;

    public bool GetMuted(ButtonTurnOnOff.EventType type)
    {
        switch (type)
        {
            case ButtonTurnOnOff.EventType.Music:
                return GetIsMusicMuted();
            case ButtonTurnOnOff.EventType.SFX:
                return GetIsSFXMuted();
            case ButtonTurnOnOff.EventType.UI:
                return GetIsUIMuted();
            case ButtonTurnOnOff.EventType.Vibration:
                break;
            default:
                break;
        }
        return false;
    }

    public void ToggleMusic()
    {
        _isMusicMuted = !_isMusicMuted;
        mainMixer.SetFloat("MusicVolume", _isMusicMuted ? -80f : 0f);
    }

    public void ToggleSFX()
    {
        _isSFXMuted = !_isSFXMuted;
        mainMixer.SetFloat("SFXVolume", _isSFXMuted ? -80f : 0f);
    }

    public void ToggleUI()
    {
        _isUIMuted = !_isUIMuted;
        mainMixer.SetFloat("UIVolume", _isUIMuted ? -80f : 0f);
    }
    #endregion

    void Update()
    {
#if UNITY_EDITOR

        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        if (keyboard.mKey.wasPressedThisFrame)
        {
            ToggleMusic();
            Debug.Log($"[AudioTest] Music Muted: {_isMusicMuted}");
        }

        if (keyboard.sKey.wasPressedThisFrame)
        {
            ToggleSFX();
            Debug.Log($"[AudioTest] SFX Muted: {_isSFXMuted}");
        }

        if (keyboard.uKey.wasPressedThisFrame)
        {
            ToggleUI();
            Debug.Log($"[AudioTest] UI Muted: {_isUIMuted}");
        }

        if (keyboard.numpadPlusKey.wasPressedThisFrame || keyboard.equalsKey.wasPressedThisFrame) // 'equalsKey' is the '+' key on most keyboards
        {
            SetSFXVolume(sfx_volume + 0.1f);
            Debug.Log($"[AudioTest] SFX Volume: {sfx_volume}");
        }

        if (keyboard.numpadMinusKey.wasPressedThisFrame || keyboard.minusKey.wasPressedThisFrame)
        {
            SetSFXVolume(sfx_volume - 0.1f);
            Debug.Log($"[AudioTest] SFX Volume: {sfx_volume}");
        }
#endif
    }
}
