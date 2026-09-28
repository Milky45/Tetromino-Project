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
        if (data != null && data.cells != null)
        {
            cells = new Vector2Int[data.cells.Length];
            ghostCells = new Vector2Int[data.cells.Length];
            for (int i = 0; i < cells.Length; i++)
            {
                cells[i] = data.cells[i];
            }
        }

        if (pieceHelpers != null)
        {
            pieceHelpers.Set();
        }
    }

    private void Update()
    {
        if (pieceHelpers != null)
        {
            pieceHelpers.Set();
        }
        DrawGhost();
    }

    public bool IsValidPosition(Vector2Int pos, Vector2Int[] testCells = null)
    {
        Vector2Int[] checkCells = testCells ?? cells;
        if (playerBoard == null || checkCells == null) return false;

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
        if (playerBoard == null || playerBoard.ghost_tilemap == null || pieceHelpers == null) return;
        playerBoard.ghost_tilemap.ClearAllTiles();

        Vector2Int ghostPos = position;
        pieceHelpers.ClearActivePiece();

        while (IsValidPosition(ghostPos + Vector2Int.down))
        {
            ghostPos += Vector2Int.down;
        }

        pieceHelpers.Set();

        if (data != null && data.ghostTile != null && cells != null)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                Vector3Int tilePos = new Vector3Int(ghostPos.x + cells[i].x, ghostPos.y + cells[i].y, 0);
                playerBoard.ghost_tilemap.SetTile(tilePos, data.ghostTile);
            }
        }
    }

    public void ClearGhost()
    {
        if (playerBoard != null && playerBoard.ghost_tilemap != null)
        {
            playerBoard.ghost_tilemap.ClearAllTiles();
        }
    }

}
