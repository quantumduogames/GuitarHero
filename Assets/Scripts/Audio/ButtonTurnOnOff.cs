// using Microsoft.Unity.VisualStudio.Editor;
using System;
using Unity.VisualScripting;
using UnityEngine;

public class ButtonTurnOnOff : MonoBehaviour
{
    [SerializeField] GameObject off_bracket;

    private void Start()
    {
        off_bracket.SetActive(AudioManager.Singleton.GetMuted(type));
    }

    public enum EventType
    {
        Music,
        SFX,
        UI,
        Vibration
    }

    public EventType type;

    public void OnToggle()
    {
        switch (type)
        {
            case EventType.Music:
                AudioManager.Singleton.ToggleMusic();
                break;
            case EventType.SFX:
                AudioManager.Singleton.ToggleSFX();

                break;
            case EventType.UI:
                AudioManager.Singleton.ToggleUI();

                break;
            case EventType.Vibration:
                break;
            default:
                break;
        }
        off_bracket.SetActive(AudioManager.Singleton.GetMuted(type));
    }
}
