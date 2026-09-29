using System.Collections;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [Header("Player Managers")]
    [SerializeField] public PlayerManager player1Manager;
    [SerializeField] public PlayerManager player2Manager;

    [SerializeField] private PieceHelpers p1PieceHelpers;
    [SerializeField] private PieceHelpers p2PieceHelpers;

    [Header("Game State")]
    public bool isGameOver = false;
    public int playerWinnerID = 0; // 1 = P1 Wins, 2 = P2 Wins
    public bool isPaused = false;

    [Header("Game Settings")]
    [SerializeField] private int scorePerLine = 100;

    [Header("Gravity System")]
    [Tooltip("Starting gravity delay in seconds per step.")]
    [SerializeField] private float baseGravityDelay = 1.0f;
    [Tooltip("Minimum cap so gravity doesn't reach 0s or drop too fast.")]
    [SerializeField] private float minGravityDelay = 0.05f;
    [Tooltip("Multiplier applied every speed increase interval.")]
    [SerializeField] private float levelSpeedMultiplier = 0.85f;
    [Tooltip("Time in seconds between gravity speed increases.")]
    [SerializeField] private float gravityIncreaseInterval = 60f;

    [Header("Gravity Tracking")]
    public float currentGravityDelay;
    public float gravityTimer = 0f;
    public int currentLevel = 1;

    [Header("Game Debugging: Player 1")]
    public bool disableSpawnForP1 = false;
    public bool disableP1CharacterSkills = false;
    public bool enableGodModeP1 = false;

    [Header("Game Debugging: Player 2")]
    public bool disableSpawnForP2 = false;
    public bool disableP2CharacterSkills = false;
    public bool enableGodModeP2 = false;

    private void Awake()
    {
        // Assign opponent references
        if (player1Manager != null && player2Manager != null)
        {
            player1Manager.opponentPlayerManager = player2Manager;
            player2Manager.opponentPlayerManager = player1Manager;
        }

        if (player1Manager?.pieceSpawner != null) player1Manager.pieceSpawner.pieceHelpers = p1PieceHelpers;
        if (player2Manager?.pieceSpawner != null) player2Manager.pieceSpawner.pieceHelpers = p2PieceHelpers;
    }

    private void Start()
    {
        // Initialize base gravity delay at start
        currentGravityDelay = baseGravityDelay;
        UpdatePlayerGravity();
    }

    private void Update()
    {
        if (isGameOver || isPaused) return;

        // Sync debugging flags to spawners
        if (player1Manager?.pieceSpawner != null) player1Manager.pieceSpawner.disableSpawn = disableSpawnForP1;
        if (player2Manager?.pieceSpawner != null) player2Manager.pieceSpawner.disableSpawn = disableSpawnForP2;

        // --- Gravity Timer System ---
        gravityTimer += Time.deltaTime;
        if (gravityTimer >= gravityIncreaseInterval)
        {
            gravityTimer = 0f;
            IncreaseGravity();
        }
    }

    private void IncreaseGravity()
    {
        currentLevel++;
        // Multiply by levelSpeedMultiplier and ensure it doesn't fall below minGravityDelay
        currentGravityDelay = Mathf.Max(minGravityDelay, currentGravityDelay * levelSpeedMultiplier);
        
        Debug.Log($"[GRAVITY INCREASED] Level {currentLevel}! New Gravity Delay: {currentGravityDelay:F3}s");
        
        UpdatePlayerGravity();
    }

    private void UpdatePlayerGravity()
    {
        // Set current gravity delay directly onto both PlayerBoards
        if (player1Manager != null && player1Manager.playerBoard != null)
        {
            player1Manager.playerBoard.currentgravityDelay = currentGravityDelay;
        }

        if (player2Manager != null && player2Manager.playerBoard != null)
        {
            player2Manager.playerBoard.currentgravityDelay = currentGravityDelay;
        }
    }

    public void ReportPlayerGameLoss(bool isPlayer1)
    {
        if (isGameOver) return;

        playerWinnerID = isPlayer1 ? 2 : 1;
        GameOver();
    }

    public void CheckCatchUpCondition(PlayerManager playerManager)
    {
        PlayerStatus fallenPlayerStatus = playerManager.playerStatus;

        if (fallenPlayerStatus.score >= playerManager.opponentPlayerManager.playerStatus.score)
        {
            string fallenId = fallenPlayerStatus.isPlayer1 ? "P1" : "P2";
            string opponentId = fallenPlayerStatus.isPlayer1 ? "P2" : "P1";
            Debug.Log($"[CATCH-UP] {fallenId} ran out of lives with score {fallenPlayerStatus.score}. {opponentId} can still catch up!");

            StartCoroutine(WaitForCatchUpCompletion(playerManager));
        }
        else
        {
            ReportPlayerGameLoss(fallenPlayerStatus.isPlayer1);
        }
    }

    private IEnumerator WaitForCatchUpCompletion(PlayerManager playerManager)
    {
        PlayerStatus catchingUpPlayer = playerManager.opponentPlayerManager.playerStatus;
        PlayerStatus fallenPlayer = playerManager.playerStatus;
        int targetScore = fallenPlayer.score;

        string catchingPlayerId = catchingUpPlayer.isPlayer1 ? "P1" : "P2";

        while (catchingUpPlayer.lives > 0 && catchingUpPlayer.score < targetScore)
        {
            yield return null;
        }

        if (catchingUpPlayer.score >= targetScore)
        {
            Debug.Log($"[CATCH-UP SUCCESS] {catchingPlayerId} reached {catchingUpPlayer.score} pts!");
            ReportPlayerGameLoss(fallenPlayer.isPlayer1);
        }
        else
        {
            Debug.Log($"[CATCH-UP FAILED] {catchingPlayerId} failed to catch up!");
            ReportPlayerGameLoss(catchingUpPlayer.isPlayer1);
        }
    }

    public void GameOver()
    {
        isGameOver = true;

        string winnerStr = playerWinnerID == 1 ? "Player 1" : (playerWinnerID == 2 ? "Player 2" : "No One");
        Debug.Log($"=======================================");
        Debug.Log($"             GAME OVER                 ");
        Debug.Log($"          WINNER: {winnerStr}          ");
        Debug.Log($"=======================================");

        if (player1Manager?.playerInput != null) player1Manager.playerInput.DeactivateInput();
        if (player2Manager?.playerInput != null) player2Manager.playerInput.DeactivateInput();

        disableSpawnForP1 = true;
        disableSpawnForP2 = true;
    }
}