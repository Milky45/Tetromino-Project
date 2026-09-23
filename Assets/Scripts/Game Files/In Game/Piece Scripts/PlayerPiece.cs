using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPiece : MonoBehaviour
{
    [Header("References")]
    public PlayerBoard playerBoard;
    public PlayerManager playerManager;
    public TetrominoData data;
    public ComboCounter comboCounter;

    [Header("Piece Interaction Reference")]
    public PieceHelpers pieceHelpers;
    public PieceMovement pieceMovement;
    public PieceRotation pieceRotation;

    public Vector2Int position;
    public Vector2Int[] cells;
    private Vector2Int[] ghostCells;
    
    
    private float lockoutDuration = 0.1f;

    
    public void Start()
    {
        cells = new Vector2Int[data.cells.Length];
        ghostCells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = data.cells[i];
        }

        pieceHelpers.Set();
    }

    private void Update()
    {
        pieceHelpers.Set();
        DrawGhost();
    }

    private void HardDrop()
    {
        while (playerManager.pieceMovement.TryMove(Vector2Int.down))
        {
            // Keep moving down while it's valid
            continue;
        }

        // Lock the piece in place when it can't move further
        playerManager.hardDropLockoutTimer = lockoutDuration;
        playerManager.playerStatus.holdUsed = false;
        pieceHelpers.LockPiece();
        playerManager.audioManager.PlaySFX(playerManager.audioManager.dropClip);
    }

    

    public bool IsValidPosition(Vector2Int pos, Vector2Int[] testCells = null)
    {
        Vector2Int[] checkCells = testCells ?? cells;

        foreach (Vector2Int cell in checkCells)
        {
            Vector3Int tilePos = new Vector3Int(pos.x + cell.x, pos.y + cell.y, 0);

            if (!playerBoard.IsInsideBoard(tilePos) || playerBoard.IsTileOccupied(tilePos))
            {
                return false;
            }
        }
        return true;
    }

    public void DrawGhost()
    {
        Vector2Int ghostPos = position;
        pieceHelpers.ClearActivePiece();
        while (IsValidPosition(ghostPos + Vector2Int.down))
        {
            ghostPos += Vector2Int.down;
        }

        pieceHelpers.Set();

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = new Vector3Int(ghostPos.x + cells[i].x, ghostPos.y + cells[i].y, 0);
            playerBoard.ghost_tilemap.SetTile(tilePos, data.ghostTile);
        }
    }

    public void ClearGhost()
    {
        playerBoard.ghost_tilemap.ClearAllTiles(); // MUCH cleaner
    }

}
