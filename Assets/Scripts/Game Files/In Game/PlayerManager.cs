using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    [Header("References")]
    public PlayerBoard playerBoard;
    public GameMaster gameMaster;
    public PlayerStatus playerStatus;
    public PieceMovement pieceMovement;
    public PieceRotation pieceRotation;
    public UnityEngine.InputSystem.PlayerInput playerInput;
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
        if (gameMaster != null && gameMaster.isGameOver) return;

        // Cancel pending respawns if another top-out happens in sequence
        CancelInvoke(nameof(LossRespawn));

        if (shaker != null) shaker.boardShake();

        playerStatus.lives--;
        string idStr = playerStatus.isPlayer1 ? "P1" : "P2";
        Debug.Log($"[LIFE LOSS] {idStr} lost a life! Remaining: {playerStatus.lives}");

        if (gameDisplay != null)
        {
            gameDisplay.UpdateHeartIcons(playerStatus.lives);
            gameDisplay.Ammo_Update(playerStatus.attackAmmo);
            gameDisplay.UpdateEMPStateIcon();
        }

        if (playerStatus.lives <= 0)
        {
            // Check if opponent is already out of lives
            if (opponentPlayerManager != null && opponentPlayerManager.playerStatus != null && opponentPlayerManager.playerStatus.lives <= 0)
            {
                if (gameMaster != null) gameMaster.ReportPlayerGameLoss(playerStatus.isPlayer1);
            }
            else
            {
                if (gameMaster != null) gameMaster.CheckCatchUpCondition(this);
            }
        }
        else
        {
            ResetPlayerAfterLifeLoss();
        }
    }

    private void ResetPlayerAfterLifeLoss()
    {
        // Clear active piece object
        GameObject existingPiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);

        playerActivePiece = null;

        // Clear boards
        if (playerBoard != null)
        {
            playerBoard.ClearAll();
            if (playerBoard.ghost_tilemap != null) playerBoard.ghost_tilemap.ClearAllTiles();
        }

        // Reset turn and status state
        if (pieceSpawner != null) pieceSpawner.heldTetromino = null;
        playerStatus.holdUsed = false;
        playerStatus.comboCount = 0;
        playerStatus.lastComboMilestone = 0;
        playerStatus.attackAmmo = 0;
        playerStatus.hasEmpGrenade = false;
        playerStatus.pendingDeadLines = 0;

        if (gameDisplay != null) gameDisplay.UpdateComboText();

        // Queue respawn
        Invoke(nameof(LossRespawn), 2f);
    }

    public void LossRespawn()
    {
        if (gameMaster != null && gameMaster.isGameOver) return;

        GameObject existingPiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);

        if (playerBoard != null)
        {
            playerBoard.ClearAll();
            if (playerBoard.ghost_tilemap != null) playerBoard.ghost_tilemap.ClearAllTiles();
        }

        if (pieceSpawner != null)
        {
            pieceSpawner.SpawnPiece(null);
        }
    }
}