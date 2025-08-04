using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;

public class VsGameOverManager : MonoBehaviour
{
    public GameManager player1Manager;
    public GameManagerP2 player2Manager;
    public TimerDisplay sharedTimer;
    public float startTextAnim = 239f;

    [Header("P1 Character Prefabs")]
    public GameObject P1Tetro;
    public GameObject P1PackHat;
    public GameObject P1Scorch;
    public GameObject P1Kor;
    public GameObject P1Dodoke;
    public GameObject P1YunJin;

    [Header("P2 Character Prefabs")]
    public GameObject P2Tetro;
    public GameObject P2PackHat;
    public GameObject P2Scorch;
    public GameObject P2Kor;
    public GameObject P2Dodoke;
    public GameObject P2YunJin;

    [Header("Game Over UI")]
    public GameObject GameOverText;
    public TMP_Text GO_MainText;
    public TMP_Text GO_OutlineText;
    public GameObject RestartBtn;

    private bool p1Failed = false;
    private bool p2Failed = false;
    private int player1FailTime = -1; // in seconds
    private int player2FailTime = -1; // in seconds
    public bool isCatchingUp = false;
    private int fallenPlayer = 0; // 0 = none, 1 = P1, 2 = P2 <-- NEW
    private int i = 0;

    private float hue = 0f;
    private float hsvCycleDuration = 1f; // 60 seconds
    private float hsvTimer = 0f;
    private bool isCycling = false;

    // Call this when the Game Over text appears
    public void StartHSVOutlineCycle()
    {
        hue = 0f;
        hsvTimer = 0f;
        isCycling = true;
    }
    public void Start()
    {
        //MainTextObj.SetActive(false);
        UpdateP1CharacterDisplay();
        UpdateP2CharacterDisplay();
        AudioManager.Instance.VSPlayMusic();
    }

    void Update()
    {
        if (isCycling)
        {
            hsvTimer += Time.deltaTime;
            hue += Time.deltaTime / hsvCycleDuration;
            if (hue > 1f) hue = 0f;

            GO_OutlineText.color = Color.HSVToRGB(hue, 1f, 1f);
        }


        if (isCatchingUp)
        {
            float timeElapsed = sharedTimer.GetElapsedTime(); // You'll add this method to TimerDisplay
            if (timeElapsed >= 393f)
            {
                Debug.Log("Time's up during catch-up phase!");
                EndGame();
                isCatchingUp = false;
            }

            // Check if the active player surpassed the failed one
            if (fallenPlayer == 1 && player2Manager.P2score > player1Manager.P1score)
            {
                Debug.Log("Player 2 caught up and passed Player 1!");
                player2Manager.GameOver();
                EndGame();
                isCatchingUp = false;
            }
            else if (fallenPlayer == 2 && player1Manager.P1score > player2Manager.P2score)
            {
                Debug.Log("Player 1 caught up and passed Player 2!");
                player1Manager.GameOver();
                EndGame();
                isCatchingUp = false;
            }
        }

        if (!p1Failed && !p2Failed)
        {
            float timeElapsed = sharedTimer.GetElapsedTime(); // Assuming this exists in TimerDisplay
            if (timeElapsed >= 393f)
            {
                Debug.Log("Time's up - both players survived!");
                EndGame();
            }
        }

    }

    private void UpdateP1CharacterDisplay()
    {
        // Deactivate all
        P1Tetro.SetActive(false);
        P1PackHat.SetActive(false);
        P1Scorch.SetActive(false);
        P1Kor.SetActive(false);
        P1Dodoke.SetActive(false);
        P1YunJin.SetActive(false);


        // Activate based on grid position
        if (CharacterSelector.P1ChosenCharacter == 1)
        {
            P1Tetro.SetActive(true);
        }
        else if (CharacterSelector.P1ChosenCharacter == 2)
        {
            P1Kor.SetActive(true);
        }
        else if (CharacterSelector.P1ChosenCharacter == 3)
        {
            P1PackHat.SetActive(true);
        }
        else if (CharacterSelector.P1ChosenCharacter == 4)
        {
            P1YunJin.SetActive(true);
        }
        else if (CharacterSelector.P1ChosenCharacter == 5)
        {
            P1Dodoke.SetActive(true);
        }
        else if (CharacterSelector.P1ChosenCharacter == 6)
        {
            P1Scorch.SetActive(true);
        }
        else
        {
            P1Kor.SetActive(true); // default debug purpouses
        }
    }

    private void UpdateP2CharacterDisplay()
    {
        // Deactivate all
        P2Tetro.SetActive(false);
        P2PackHat.SetActive(false);
        P2Scorch.SetActive(false);
        P2Kor.SetActive(false);
        P2Dodoke.SetActive(false);
        P2YunJin.SetActive(false);

        // Activate based on grid position
        if (CharacterSelector.P2ChosenCharacter == 1)
        {
            P2Tetro.SetActive(true);
        }
        else if (CharacterSelector.P2ChosenCharacter == 2)
        {
            P2Kor.SetActive(true);
        }
        else if (CharacterSelector.P2ChosenCharacter == 3)
        {
            P2PackHat.SetActive(true);
        }
        else if (CharacterSelector.P2ChosenCharacter == 4)
        {
            P2YunJin.SetActive(true);
        }
        else if (CharacterSelector.P2ChosenCharacter == 5)
        {
            P2Dodoke.SetActive(true);
        }
        else if (CharacterSelector.P2ChosenCharacter == 6)
        {
            P2Scorch.SetActive(true);
        }
        else
        {
            P2Tetro.SetActive(true); // default debug purpouses
        }
    }

    public void OnPlayer1Fail()
    {
        if (p1Failed) return;
        p1Failed = true;
        player1FailTime = Mathf.FloorToInt(sharedTimer.GetElapsedTime());
        Debug.Log($"Player 1 failed at {player1FailTime} seconds.");

        if (!p2Failed)
        {
            isCatchingUp = true; // <-- NEW
            fallenPlayer = 1;    // <-- NEW
            Debug.Log("Player 2 is catching up!");
            return;
        }

        EndGame(); // Both players failed
    }

    public void OnPlayer2Fail()
    {
        if (p2Failed) return;
        p2Failed = true;
        player2FailTime = Mathf.FloorToInt(sharedTimer.GetElapsedTime());
        Debug.Log($"Player 2 failed at {player2FailTime} seconds.");

        if (!p1Failed)
        {
            isCatchingUp = true; // <-- NEW
            fallenPlayer = 2;    // <-- NEW
            Debug.Log("Player 1 is catching up!");
            return;
        }

        EndGame(); // Both players failed
    }

    private void EndGame()
    {
        i++;
        Debug.Log($"EndGame called {i} times");
        AudioManager.Instance.VSStopMusic();
        sharedTimer.StopTimer();
        player1Manager.GameOver();
        player2Manager.GameOver();
        GameOverText.SetActive(true);
        StartHSVOutlineCycle();
        GO_MainText.text = "GAME OVER";
        GO_OutlineText.text = "GAME OVER";

        Debug.Log("VS Mode Over!");
        Debug.Log($"Final Score - P1: {player1Manager.P1score} | P2: {player2Manager.P2score}");
        //GameOverSave.SaveEndGameStats(player1Manager.P1score, player2Manager.P2score);

        StartCoroutine(ShowResultAfterDelay());
    }

    private IEnumerator ShowResultAfterDelay()
    {
        yield return new WaitForSeconds(3f);

        string winnerMessage = "";

        if (isCatchingUp)
        {
            if (player1Manager.P1score == player2Manager.P2score)
            {
                // New case: both players failed, and scores are equal
                winnerMessage = "DRAW!";
            }
            else if (fallenPlayer == 1)
            {
                winnerMessage = player2Manager.P2score > player1Manager.P1score
                    ? "PLAYER 2 WINS!" : "PLAYER 1 WINS!";
            }
            else if (fallenPlayer == 2)
            {
                winnerMessage = player1Manager.P1score > player2Manager.P2score
                    ? "PLAYER 1 WINS!" : "PLAYER 2 WINS!";
            }
            else
            {
                winnerMessage = "DRAW!";
            }
        }
        else
        {
            if (player1Manager.P1score > player2Manager.P2score)
            {
                winnerMessage = "PLAYER 1 WINS!";
            }
            else if (player2Manager.P2score > player1Manager.P1score)
            {
                winnerMessage = "PLAYER 2 WINS!";
            }
            else
            {
                winnerMessage = "DRAW!";
            }
        }
        RestartBtn.SetActive(true);
        GO_MainText.text = winnerMessage;
        GO_OutlineText.text = winnerMessage;
    }

    public void RestartGame()
    {
        StartCoroutine(RestartSceneCoroutine());
    }

    private IEnumerator RestartSceneCoroutine()
    {
        // Optional: Add a fade-out effect here
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}