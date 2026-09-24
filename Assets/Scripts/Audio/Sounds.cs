using UnityEngine;

public enum SoundType
{
    Bounce,
    Hit,
    BounceMultiplier,
    Peg
}

public static class Sounds
{
    private const string Root = "TableSounds/";

    public const string PATH_MYSTIC_TABLE_SOUND = "MysticDropSoundTable";
    public const string PATH_MATCH_THREE_TABLE_SOUND = "MatchThreeSoundTable";
    public const string PATH_MUSIC_TABLE_SOUND = "MusicSoundTable";
    public const string PATH_WHEEL_OF_DESTINY_TABLE_SOUND = "WheelOfDestinySoundTable";
    public const string PATH_UI_TABLE_SOUND = "UISoundTable";
    public const string PATH_FIRST_WORLD_TABLE_SOUND = "FirstWorldSoundTable";
    public const string PATH_GALACTIC_BATTLE_TABLE_SOUND = "GalacticBattleSoundTable";
    public const string PATH_RESULT_POPUP = "WinPanelSoundTable";
    public const string PATH_REWARD_PANELS = "RewardsPanelSoundTable";

    /// <summary>
    /// Carica una tabella specifica dalla cartella Resources.
    /// </summary>
    public static SoundTable GetTable(string resourcePath)
    {
        SoundTable table = Resources.Load<SoundTable>(Root + resourcePath);
        if (table == null)
        {
            Debug.LogError($"[Sounds] Tabella non trovata al percorso: Resources/{resourcePath}");
        }
        return table;
    }
}
