using UnityEngine;
using TMPro;

public class LeaderboardRowUI : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text scoreText;

    public void SetData(string playerName, float scoreTime)
    {
        if (nameText) nameText.text = playerName;
        if (scoreText) scoreText.text = $"{scoreTime:F2}s";
    }
}