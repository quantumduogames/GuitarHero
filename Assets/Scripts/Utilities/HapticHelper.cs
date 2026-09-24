using UnityEngine;

public static class HapticHelper
{
    private static float lastVibrationTime;
    private const float MinInterval = 0.15f;

    public static bool Enabled = true;

    public static void LightImpact()
    {
        if (!Enabled) return;

#if UNITY_EDITOR
        Debug.Log("HAPTIC");
       // UnityEditor.EditorApplication.Beep();
#endif

#if UNITY_ANDROID || UNITY_IOS
    if (Time.unscaledTime - lastVibrationTime < MinInterval)
        return;

    lastVibrationTime = Time.unscaledTime;

    if (SystemInfo.supportsVibration)
        Handheld.Vibrate();
#endif
    }
}