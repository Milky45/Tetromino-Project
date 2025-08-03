using TMPro;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerSolo : MonoBehaviour
{
    private int comboCount = 0;

    [Header("References")]
    [SerializeField] public BoardManager board;
    [SerializeField] private TetrominoData[] tetrominoSet;
    [SerializeField] public NextDisplayUI nextDisplayUI;
    [SerializeField] public HoldDisplayUI holdDisplayUI;
    [SerializeField] public TMP_Text comboText;
    [SerializeField] public TMP_Text scoreDisplay;
    public TimerDisplay timerDisplay;
    public GameObject gameOverUI;


    [Header("Gameplay Settings")]
    public float moveDelay = 0.08f;
    public static float movementSensitivity = 0.12f; // Controls how quickly the piece responds to input
    [SerializeField] private float movementSpeedMultiplier = 1.0f; // Controls overall movement speed
    private TetrominoData previousTetromino;
    private TetrominoData currentTetromino;
    public TetrominoData nextTetromino;

    public TetrominoData heldTetromino;
    public bool holdUsedThisTurn = false;
    public int P1score = 0;
    public bool isPaused;
    public bool isGameOver = false;

    [Header("Gravity Settings")]
    [SerializeField] internal static float initialGravityDelay = 1.0f;
    [SerializeField] private float gravityIncreaseInterval = 60f; // every 1 min
    [SerializeField] private float minGravityDelay = 0.2f;
    [SerializeField] internal static bool progressiveGravityEnabled = true;

    // Progressive Gravity Settings
    private readonly int[] scoreThresholds = { 1000, 2000, 3000, 4000, 5000, 6000, 7000, 8000, 9000, 10000, 
                                             11000, 12000, 13000, 14000, 15000, 16000, 17000, 18000, 19000, 20000};
    private readonly float[] gravityDelays = { 0.8f, 0.75f, 0.7f, 0.65f, 0.6f, 0.55f, 0.5f, 0.45f, 0.4f, 0.35f,
                                             0.3f, 0.29f, 0.28f, 0.27f, 0.26f, 0.25f, 0.24f, 0.23f, 0.22f, 0.2f,};
    private int currentThresholdIndex = 0;

    private float currentGravityDelay;
    private float elapsedTime = 0f;

    public void Start()
    {
        currentGravityDelay = initialGravityDelay;
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];

        SpawnNextPiece();
        AudioManager.Instance.VSPlayMusic();
    }

    public void Update()
    {
        if (isPaused) return;

        elapsedTime += Time.deltaTime;

        if (progressiveGravityEnabled)
        {
            UpdateProgressiveGravity();
        }
        else
        {
            UpdateStandardGravity();
        }
    }

    private void UpdateProgressiveGravity()
    {
        if (currentThresholdIndex < scoreThresholds.Length && P1score >= scoreThresholds[currentThresholdIndex])
        {
            currentGravityDelay = gravityDelays[currentThresholdIndex];
            currentThresholdIndex++;
            Debug.Log($"Progressive Gravity: Reached {scoreThresholds[currentThresholdIndex-1]} points! New gravity delay: {currentGravityDelay:F2}s");
        }
    }

    private void UpdateStandardGravity()
    {
        if (elapsedTime >= gravityIncreaseInterval)
        {
            elapsedTime = 0f;
            currentGravityDelay -= 0.1f;
            currentGravityDelay = Mathf.Max(currentGravityDelay, minGravityDelay);

            if (Mathf.Approximately(currentGravityDelay, minGravityDelay) || currentGravityDelay <= minGravityDelay + 0.0001f)
            {
                Debug.Log("Reached minimum gravity delay.");
                return;
            }
            else
            {
                Debug.Log($"Gravity increased! New delay: {currentGravityDelay:F2}s");
            }
        }
    }

    public void SpawnNextPiece()
    {
        TetrominoData current = nextTetromino;

        int attempts = 0;
        do
        {
            int randomIndex = Random.Range(0, tetrominoSet.Length);
            nextTetromino = tetrominoSet[randomIndex];
            attempts++;
            if (attempts > 10) break;
        }
        while (nextTetromino == current);

        nextDisplayUI.ShowNext(nextTetromino.tetromino);

        currentTetromino = current;
        previousTetromino = currentTetromino;

        GameObject pieceObj = new GameObject("ActivePiece");
        PieceControllerSolo controller = pieceObj.AddComponent<PieceControllerSolo>();
        controller.board = board;
        controller.data = currentTetromino;
        controller.position = new Vector2Int(0, board.Bounds.yMax - 4);
        controller.gameManager = this;
    }

    public void TryHoldPiece(TetrominoData current, PieceControllerSolo controller)
    {
        if (isPaused) return;
        if (holdUsedThisTurn)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.invalid);
            comboText.color = Color.red;
            comboText.text = "SWAP LOCKED";
            Debug.Log("Hold already used this turn!");
            return;
        }

        controller.Clear();

        if (heldTetromino == null)
        {
            heldTetromino = current;
            SpawnNextPiece();
        }
        else
        {
            TetrominoData temp = heldTetromino;
            heldTetromino = current;
            SpawnHeldPiece(temp);
        }

        AudioManager.Instance.PlaySFX(AudioManager.Instance.holdClip);
        holdUsedThisTurn = true;
        holdDisplayUI.ShowHold(heldTetromino.tetromino);

        Destroy(controller.gameObject);
    }

    public void SpawnHeldPiece(TetrominoData data)
    {
        GameObject pieceObj = new GameObject("ActivePiece");
        PieceControllerSolo controller = pieceObj.AddComponent<PieceControllerSolo>();
        controller.board = board;
        controller.data = data;
        controller.position = new Vector2Int(0, board.Bounds.yMax - 4);
        controller.gameManager = this;

        currentTetromino = data;
    }

    public void ResetHold()
    {
        holdUsedThisTurn = false;
    }

    public void ComboCount()
    {
        int linesCleared = board.ClearLines();
        P1score += 100 * linesCleared;

        if (linesCleared > 0)
        {
            comboCount += linesCleared; // increase combo per successful line clear (any amount)
            
            // Combo bonus (only starts after 2 consecutive clears)
            if (comboCount > 1)
            {
                P1score += 100;
                comboText.color = Color.yellow;
                comboText.text = $"Combo x{comboCount}";

                // Clamp and play combo SFX up to 13
                int soundIndex = Mathf.Clamp(comboCount, 2, 13); // Start from 2
                PlayComboSFX(soundIndex);
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.clear1);
            }
        }
        else
        {
            comboCount = 0;
            comboText.text = "";
        }

        scoreDisplay.text = $"{P1score}";
    }

    private void PlayComboSFX(int combo)
    {
        switch (combo)
        {
            case 2: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear2); break;
            case 3: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear3); break;
            case 4: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear4); break;
            case 5: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear5); break;
            case 6: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear6); break;
            case 7: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear7); break;
            case 8: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear8); break;
            case 9: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear9); break;
            case 10: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear10); break;
            case 11: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear11); break;
            case 12: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear12); break;
            case 13: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear13); break;
            default: break;
        }
    }

    public void GameOver()
    {
        gameOverUI.SetActive(true);
        AudioManager.Instance.VSStopMusic();
        Debug.Log("Game Over - Resetting...");
        board.ClearAll();

        // Clear displays
        nextDisplayUI.HideAll();
        holdDisplayUI.HideAll();

        // Reset state
        heldTetromino = null;
        holdUsedThisTurn = false;
        currentThresholdIndex = 0;

        GameObject existingPiece = GameObject.Find("ActivePiece");
        if (existingPiece) Destroy(existingPiece);

    }

    public void RestartGame()
    {
        timerDisplay.StopTimer();
        StartCoroutine(RestartSceneCoroutine());
    }

    private IEnumerator RestartSceneCoroutine()
    {
        // Optional: Add a fade-out effect here
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void TogglePause()
    {
        if (isGameOver) return; // Don't allow pause if already game over
        GamePauseSolo.Instance?.TogglePause();
    }

    public float GetGravityDelay()
    {
        return currentGravityDelay;
    }

    public float GetMovementSensitivity()
    {
        return movementSensitivity;
    }

    public float GetMovementSpeedMultiplier()
    {
        return movementSpeedMultiplier;
    }
}
