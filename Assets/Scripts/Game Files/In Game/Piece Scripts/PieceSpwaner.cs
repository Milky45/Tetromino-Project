using UnityEngine;

public class PieceSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerBoard playerBoard;
    public PieceHelpers pieceHelpers;

    [Header("Tetromino Data")]
    public TetrominoData[] tetrominoSet;
    public TetrominoData heldTetromino;
    public TetrominoData nextTetromino;
    [SerializeField] private TetrominoData currentTetromino;

    private void Start()
    {
        if (tetrominoSet != null && tetrominoSet.Length > 0)
        {
            if (nextTetromino == null)
            {
                int randomIndex = Random.Range(0, tetrominoSet.Length);
                nextTetromino = tetrominoSet[randomIndex];
            }
            SpawnPiece(null);
        }
    }

    public void SpawnPiece(TetrominoData data)
    {
        if (tetrominoSet == null || tetrominoSet.Length == 0) return;

        if (playerManager != null && playerManager.gameMaster != null && playerManager.playerStatus != null)
        {
            bool isP1 = playerManager.playerStatus.isPlayer1;
            if (isP1 && playerManager.gameMaster.disableSpawnForP1) return;
            if (!isP1 && playerManager.gameMaster.disableSpawnForP2) return;
        }

        bool isPlayer1 = playerManager != null && playerManager.playerStatus != null && playerManager.playerStatus.isPlayer1;

        var existingPiece = GameObject.Find($"ActivePiece{(isPlayer1 ? "P1" : "P2")}");
        if (existingPiece != null)
        {
            Destroy(existingPiece);
        }

        if (data != null)
        {
            currentTetromino = data;
        }
        else
        {
            if (nextTetromino == null)
            {
                int randomIndex = Random.Range(0, tetrominoSet.Length);
                nextTetromino = tetrominoSet[randomIndex];
            }

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

            currentTetromino = current;
        }

        GameObject pieceObj = new GameObject($"ActivePiece{(isPlayer1 ? "P1" : "P2")}");
        pieceObj.transform.parent = this.transform; // Make it a child of Game_Manager
        PlayerPiece controller = pieceObj.AddComponent<PlayerPiece>();

        if (playerManager != null)
        {
            if (playerManager.pieceMovement != null) 
            {
                playerManager.pieceMovement.activePiece = controller;
                playerManager.pieceMovement.gravityTimer = 0f; // Reset gravity timer for the new piece
            }
            if (playerManager.pieceRotation != null) playerManager.pieceRotation.activePiece = controller;
            playerManager.playerActivePiece = controller;
        }

        controller.pieceHelpers = pieceHelpers;
        controller.data = currentTetromino;
        controller.position = new Vector2Int(0, playerBoard != null ? playerBoard.Bounds.yMax - 4 : 20);
        controller.playerManager = playerManager;
        controller.playerBoard = playerBoard;

        if (playerManager != null && playerManager.gameDisplay != null)
        {
            playerManager.gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after next changes
        }
    }
}