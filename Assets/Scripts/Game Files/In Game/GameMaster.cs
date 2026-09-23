using System.Runtime.CompilerServices;
using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [Header("Player Managers")]
    [SerializeField] private PlayerManager player1Manager;
    [SerializeField] private PlayerManager player2Manager;

    [SerializeField] private PieceHelpers pieceHelpers;

    [Header("Game State")]
    [SerializeField] private bool isGameOver = false;
    public int playerWinnerID = 0;
    public bool isPaused = false;

    [Header("Game Settings")]
    [SerializeField] private int scorePerLine = 100;
    [SerializeField] private int initialLevel = 1;
    [SerializeField] private float levelSpeedMultiplier = 0.8f;

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
        // assign their opps
        player1Manager.opponentPlayerManager = player2Manager;
        player2Manager.opponentPlayerManager = player1Manager;
        // assign the playerMangers' piece spawners some piece helpers
        player1Manager.pieceSpawner.pieceHelpers = pieceHelpers;
        player2Manager.pieceSpawner.pieceHelpers = pieceHelpers;
    }

    public void ReportPlayerGameLoss(bool isPlayer1)
    {
        // if true, then its player 1 who lost
        // set the playerWinner into 2 indicating that p2 won, playerWinner = 1 if p1 wins
        if (isPlayer1)
        {
            playerWinnerID = 2;
        }
        else
        {
            playerWinnerID = 1;
        }
    }

    public void CheckCatchUpCondition(PlayerManager playerManager)
    {
        PlayerStatus playerStatus = playerManager.playerStatus;
        PlayerBoard playerBoard = player1Manager.playerBoard;


        // If this player has higher or equal score, opponent can still catch up
        if (playerStatus.score >= player1Manager.opponentPlayerManager.playerStatus.score)
        {
            isGameOver = true;
            playerBoard.ClearAll();
            playerBoard.ghost_tilemap.ClearAllTiles();
            
            // Clear pieces
            GameObject Piece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}");
            Destroy(Piece);
            
            playerManager.pieceSpawner.heldTetromino = null;
            playerStatus.holdUsed = false;
            playerStatus.lastComboMilestone = 0;
            
            // Opponent gets to continue (they're still active)
            // Set opponent's isGameOver to false so they can keep playing
            //pvp.opponentGameManager.isGameOver = false;
            
            string playerId = playerStatus.isPlayer1 ? "P1" : "P2";
            string opponentId = playerStatus.isPlayer1 ? "P2" : "P1";
            Debug.Log($"{playerId} ran out of lives with score {playerStatus.score}. {opponentId} can still catch up!");
            
            // Let the PvP system handle the catch-up phase
            StartCoroutine(WaitForCatchUpCompletion(playerManager));
        }
        else
        {
            // This player has lower score and is out of lives - they lost
            ReportPlayerGameLoss(playerStatus.isPlayer1);
        }
    }

    private System.Collections.IEnumerator WaitForCatchUpCompletion(PlayerManager playerManager)
    {
        PlayerStatus catchingUpPlayer = playerManager.opponentPlayerManager.playerStatus;
        PlayerStatus fallenPlayer = playerManager.playerStatus;
        int targetScore = fallenPlayer.score;
        
        string catchingPlayerId = catchingUpPlayer.isPlayer1 ? "P1" : "P2";
        Debug.Log($"Catch-up phase started! {catchingPlayerId} needs to reach {targetScore} points.");
        
        // Wait while opponent is playing catch-up
        while (catchingUpPlayer.lives > 0 && catchingUpPlayer.score < targetScore)
        {
            yield return null;
        }

        // Catch-up phase ended
        if (catchingUpPlayer.score >= targetScore)
        {
            Debug.Log($"{catchingPlayerId} successfully caught up! Score: {catchingUpPlayer.score}");

        }
        else
        {
            Debug.Log($"{catchingPlayerId} failed to catch up with {fallenPlayer}! Score: {catchingUpPlayer.score}");
        }
        if (catchingUpPlayer.score > targetScore)
        {
            ReportPlayerGameLoss(fallenPlayer.isPlayer1);
        }
        else
        {
            ReportPlayerGameLoss(catchingUpPlayer.isPlayer1);
        }
    }
}
