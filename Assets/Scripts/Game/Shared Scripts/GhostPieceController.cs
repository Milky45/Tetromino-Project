using UnityEngine;

public class GhostPieceController : MonoBehaviour
{
    public BoardManager board;
    public TetrominoData data;

    public Vector2Int position;
    public Vector2Int[] cells;

    public void Initialize(TetrominoData data, Vector2Int[] originalCells)
    {
        this.data = data;
        this.cells = new Vector2Int[originalCells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = originalCells[i];
        }
    }

    public void UpdateGhost(Vector2Int currentPos)
    {
        // Drop ghost to the lowest valid position
        position = currentPos;

        while (IsValidPosition(position + Vector2Int.down))
        {
            position += Vector2Int.down;
        }

        DrawGhost();
    }

    private void DrawGhost()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(position.x + cell.x, position.y + cell.y, 0);
            board.tilemap.SetTile(tilePos, data.ghostTile); // ghostTile = transparent version
        }
    }

    public void Clear()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(position.x + cell.x, position.y + cell.y, 0);
            board.tilemap.SetTile(tilePos, null);
        }
    }

    private bool IsValidPosition(Vector2Int pos)
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(pos.x + cell.x, pos.y + cell.y, 0);
            if (!board.IsInsideBoard(tilePos) || board.IsTileOccupied(tilePos))
            {
                return false;
            }
        }
        return true;
    }
}
