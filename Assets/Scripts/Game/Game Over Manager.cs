using UnityEngine;
using TMPro;
using NUnit.Framework;

public class GameOverManager : MonoBehaviour
{
    public Game_Manager P1gameManager;
    public Game_Manager P2gameManager;
    public Animator gameOverAnimator;
    public GameObject winDisplay;
    public GameObject gameOverDisplay;
    public string playerName;
    public string playerScore;
    public TextMeshProUGUI winStringMain;
    public TextMeshProUGUI winStringHigh;

    public AudioManager audioManager;
    public bool gameEnd = false;
    public bool isSolo = false;

    private void Awake()
    {
        audioManager.VSPlayMusic();
    }

    private void Start()
    {
        if (!isSolo) { Invoke(nameof(TimedGameOver), 400f); }
    }

    public void TimedGameOver()
    {
        if(gameEnd)
        {
            return;
        }
        if (P1gameManager.player.score > P2gameManager.player.score)
        {
            P1gameManager.player.isWinner = true;
            P2gameManager.player.isWinner = false;
        }
        else
        {
            P2gameManager.player.isWinner = true;
            P1gameManager.player.isWinner = false;
        }
        TriggerGameOver();
    }

    public void TriggerGameOver()
    {
        audioManager.VSStopMusic();
        if (isSolo)
        {
            playerScore = P1gameManager.player.score.ToString();
            gameEnd = true;
            gameOverAnimator.Play("GameOver");
            Invoke(nameof(DisplayScore), 2.5f);
            return;
        }
        if (P1gameManager.player.isWinner && !P2gameManager.player.isWinner)
        {
            playerName = "Player 1";
        }
        else
        {
            playerName = "Player 2";
        }
        gameEnd = true;
        gameOverAnimator.Play("GameOver");
        Invoke(nameof(DisplayWinner), 2.5f);
    }
    
    public void DisplayScore()
    {
        winStringMain.text = $"Final Score: {playerScore}";
        winStringHigh.text = $"Final Score: {playerScore}";
        gameOverDisplay.SetActive(false);
        winDisplay.SetActive(true);
    }

    public void DisplayWinner()
    {
        winStringMain.text = $"{playerName} Wins!";
        winStringHigh.text = $"{playerName} Wins!";
        gameOverDisplay.SetActive(false);
        winDisplay.SetActive(true);
    }
}
