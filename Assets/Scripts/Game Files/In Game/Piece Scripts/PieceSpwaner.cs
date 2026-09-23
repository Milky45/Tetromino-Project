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

    public void SpawnPiece(TetrominoData data)
    {
        bool isPlayer1 = playerManager.playerStatus.isPlayer1;

        var existingPiece = GameObject.Find($"ActivePiece{(isPlayer1 ? "P1" : "P2")}");
        if (existingPiece != null)
        {
            Destroy(existingPiece);
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

        GameObject pieceObj = new GameObject($"ActivePiece{(isPlayer1 ? "P1" : "P2")}");
        pieceObj.transform.parent = this.transform; // Make it a child of Game_Manager
        PlayerPiece controller = pieceObj.AddComponent<PlayerPiece>();

        playerManager.pieceMovement.activePiece = controller;
        playerManager.pieceRotation.activePiece = controller;
        controller.pieceHelpers = pieceHelpers;

        // if null, rng the spawned piece
        // else then spawn a specific piece
        if(data != null)
        {controller.data = data;}
        else{controller.data = currentTetromino;}

        controller.position = new Vector2Int(0, playerBoard.Bounds.yMax - 4);
        controller.playerManager = playerManager;
        
        controller.playerManager.playerActivePiece = controller;
        controller.playerBoard = playerBoard;
        playerManager.gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after next changes
    }
}