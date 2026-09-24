using System;
using System.Runtime.InteropServices;
using UnityEngine;

public static class IOSAudioSessionBootstrap
{
#if UNITY_IOS && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void Cosmo_SetAudioSessionPlayback();
#endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void ConfigureIOSAudioSession()
    {
#if UNITY_IOS && !UNITY_EDITOR
        try
        {
            Cosmo_SetAudioSessionPlayback();
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"[IOSAudioSessionBootstrap] Failed to configure iOS audio session: {ex.Message}");
        }
#endif
    }
}
