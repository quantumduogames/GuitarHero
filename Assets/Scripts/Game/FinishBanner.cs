using TMPro;
using UnityEngine;

public class FinishBanner : MonoBehaviour
{
    [SerializeField] TMP_Text titleText;
    [SerializeField] TMP_Text scoreText;

    internal void SetTexts(string title, string score)
    {
        if (titleText != null)
            titleText.text = title;
        if (scoreText != null)
            scoreText.text = score;
    }

}
