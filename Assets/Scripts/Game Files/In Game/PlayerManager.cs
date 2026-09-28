using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public PlayerBoard playerBoard;
    public GameMaster gameMaster;
    public PlayerStatus playerStatus;
    public PieceMovement pieceMovement;
    public PieceRotation pieceRotation;
    public PlayerInput playerInput;
    public PlayerManager opponentPlayerManager;
    public PlayerPiece playerActivePiece;
    public PieceSpawner pieceSpawner;
    public ComboCounter comboCounter;

    public GameDisplay gameDisplay;
    public AudioManager audioManager;
    public PvP pvp;
    public Shaker shaker;

    [Header("Timers")]
    public float invertTimer;
    public float hardDropLockoutTimer;
    
    private void PlayComboSFX(int combo)
    {
        switch (combo)
        {
            case 2: audioManager.PlaySFX(audioManager.clear2); break;
            case 3: audioManager.PlaySFX(audioManager.clear3); break;
            case 4: audioManager.PlaySFX(audioManager.clear4); break;
            case 5: audioManager.PlaySFX(audioManager.clear5); break;
            case 6: audioManager.PlaySFX(audioManager.clear6); break;
            case 7: audioManager.PlaySFX(audioManager.clear7); break;
            case 8: audioManager.PlaySFX(audioManager.clear8); break;
            case 9: audioManager.PlaySFX(audioManager.clear9); break;
            case 10: audioManager.PlaySFX(audioManager.clear10); break;
            case 11: audioManager.PlaySFX(audioManager.clear11); break;
            case 12: audioManager.PlaySFX(audioManager.clear12); break;
            case 13: audioManager.PlaySFX(audioManager.clear13); break;
            default: break;
        }
    }

    public void TryHoldPiece(TetrominoData current, PlayerPiece controller)
    {   
        if (playerStatus.holdUsed)
        {
            Debug.Log("Hold already used this turn!");
            return;
        }

        if (controller != null && controller.pieceHelpers != null)
        {
            controller.pieceHelpers.ClearActivePiece();
        }

        if (pieceSpawner.heldTetromino == null)
        {
            pieceSpawner.heldTetromino = current;
            if (gameDisplay != null) gameDisplay.LogTetrominoStatus(pieceSpawner.nextTetromino, pieceSpawner.heldTetromino);
            pieceSpawner.SpawnPiece(null);
        }
        else
        {
            TetrominoData temp = pieceSpawner.heldTetromino;
            pieceSpawner.heldTetromino = current;
            if (gameDisplay != null) gameDisplay.LogTetrominoStatus(pieceSpawner.nextTetromino, pieceSpawner.heldTetromino);
            pieceSpawner.SpawnPiece(temp);
        }

        playerStatus.holdUsed = true;

        if (controller != null)
        {
            Destroy(controller.gameObject);
        }
    }

    public void LoseLife()
    {
        if (shaker != null) shaker.boardShake();
        playerStatus.lives--;
        Debug.Log($"Player lost a life! Lives remaining: {playerStatus.lives}");

        if (gameDisplay != null)
        {
            gameDisplay.UpdateHeartIcons(playerStatus.lives);
            gameDisplay.Ammo_Update(playerStatus.attackAmmo);
            gameDisplay.UpdateEMPStateIcon();
        }

        if (playerStatus.lives <= 0)
        {
            // Check if opponent is already out of lives
            if (pvp != null && pvp.opponentPlayerManager != null && pvp.opponentPlayerManager.playerStatus != null && pvp.opponentPlayerManager.playerStatus.lives <= 0)
            {
                // Both players are out of lives - game ends based on score
                if (gameMaster != null) gameMaster.ReportPlayerGameLoss(playerStatus.isPlayer1);
            }
            else
            {
                // Only this player is out of lives - check if catch-up is needed
                if (gameMaster != null) gameMaster.CheckCatchUpCondition(this);
            }
        }
        else
        {
            // Reset the board and continue the game
            ResetPlayerAfterLifeLoss();
            if (gameDisplay != null) gameDisplay.UpdateComboText();
            if (playerBoard != null)
            {
                playerBoard.ClearAll();
                if (playerBoard.ghost_tilemap != null) playerBoard.ghost_tilemap.ClearAllTiles();
            }
        }
    }

    private void ResetPlayerAfterLifeLoss()
    {
        GameObject existingPiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);
        // Clear the board
        playerBoard.ClearAll();
        playerBoard.ghost_tilemap.ClearAllTiles();
        
        // Reset game state
        pieceSpawner.heldTetromino = null;
        playerStatus.holdUsed = false;
        playerStatus.comboCount = 0;
        playerStatus.lastComboMilestone = 0;
        playerStatus.attackAmmo = 0;
        playerStatus.hasEmpGrenade = false;
     
        // Reset pending deadlines
        playerStatus.pendingDeadLines = 0;
        
        // Spawn a new piece to continue the game
        int randomIndex = Random.Range(0, pieceSpawner.tetrominoSet.Length);
        pieceSpawner.nextTetromino = pieceSpawner.tetrominoSet[randomIndex];
        gameDisplay.LogTetrominoStatus(pieceSpawner.nextTetromino, pieceSpawner.heldTetromino); // Log after board reset
        playerBoard.ClearAll();
        playerBoard.ghost_tilemap.ClearAllTiles();
        
        Invoke(nameof(LossRespawn), 3f);
        
        Debug.Log("Board reset after life loss. Game continues!");
    }

    public void LossRespawn()
    {
        GameObject existingPiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);
        playerBoard.ClearAll();
        playerBoard.ghost_tilemap.ClearAllTiles();
        pieceSpawner.SpawnPiece(null);
    }
}
