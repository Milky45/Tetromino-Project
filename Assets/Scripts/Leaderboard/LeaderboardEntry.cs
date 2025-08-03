using UnityEngine;
using TMPro;

public class LeaderboardEntry : MonoBehaviour
{
    public TMP_Text pidText;
    public TMP_Text playerNameText;
    public TMP_Text highscoreText;
    public TMP_Text survivalTimeText;

    private string FormatSurvivalTime(int seconds)
    {
        if (seconds < 3600) // Less than 1 hour
        {
            int minutes = seconds / 60;
            int remainingSeconds = seconds % 60;
            return $"{minutes:D2}:{remainingSeconds:D2}";
        }
        else // 1 hour or more
        {
            int hours = seconds / 3600;
            int minutes = (seconds % 3600) / 60;
            int remainingSeconds = seconds % 60;
            return $"{hours:D2}:{minutes:D2}:{remainingSeconds:D2}";
        }
    }

    public void SetEntry(int position, string playerName, int highscore, int survivalTime)
    {
        pidText.text = position.ToString() + ".";
        playerNameText.text = playerName;
        highscoreText.text = highscore.ToString();
        survivalTimeText.text = FormatSurvivalTime(survivalTime);
    }
}
