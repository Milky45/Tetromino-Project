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

    public bool disableSpawn = false;

    private System.Collections.Generic.List<TetrominoData> bag = new System.Collections.Generic.List<TetrominoData>();

    private void Start()
    {
        if (tetrominoSet != null && tetrominoSet.Length > 0)
        {
            if (nextTetromino == null)
            {
                nextTetromino = GetNextFromBag();
            }
            SpawnPiece(null);
        }
    }

    private TetrominoData GetNextFromBag()
    {
        if (bag.Count == 0)
        {
            RefillBag();
        }
        if (bag.Count == 0) return null;
        TetrominoData next = bag[0];
        bag.RemoveAt(0);
        return next;
    }

    private void RefillBag()
    {
        if (tetrominoSet == null || tetrominoSet.Length == 0) return;
        System.Collections.Generic.List<TetrominoData> list = new System.Collections.Generic.List<TetrominoData>(tetrominoSet);
        while (list.Count > 0)
        {
            int index = Random.Range(0, list.Count);
            bag.Add(list[index]);
            list.RemoveAt(index);
        }
    }

    public void SpawnPiece(TetrominoData data)
    {
        if (disableSpawn) return;
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
                nextTetromino = GetNextFromBag();
            }

            currentTetromino = nextTetromino;
            nextTetromino = GetNextFromBag();
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
        int spawnY = playerBoard != null ? (playerBoard.boardSize.y / 2 - 2) : 10;
        controller.position = new Vector2Int(-1, spawnY);
        controller.playerManager = playerManager;
        controller.playerBoard = playerBoard;

        if (playerManager != null && playerManager.gameDisplay != null)
        {
            playerManager.gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after next changes
        }
    }
}