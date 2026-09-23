using UnityEngine;

public class PieceHelpers : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerBoard playerBoard;
    public void Set()
    {
        foreach (Vector2Int cell in playerManager.playerActivePiece.cells)
        {
            Vector3Int tilePos = new Vector3Int(playerManager.playerActivePiece.position.x + cell.x, playerManager.playerActivePiece.position.y + cell.y, 0);
            playerBoard.main_tilemap.SetTile(tilePos, playerManager.playerActivePiece.data.tile);
        }
    }

    public void LockPiece()
    {
        playerManager.comboCounter.ComboCount();

        foreach (Vector2Int cell in playerManager.playerActivePiece.cells)
        {
            int yPos = playerManager.playerActivePiece.position.y + cell.y;
            if (yPos > 6)
            {
                playerManager.LoseLife();
                Destroy(this.gameObject); // Just in case
                return;
            }
        }

        Destroy(this.gameObject); // Remove current piece
        playerManager.pieceSpawner.SpawnPiece(null); // Don't do this if game is ending!
    }

    public bool IsValidPosition(Vector2Int pos, Vector2Int[] testCells = null)
    {
        Vector2Int[] checkCells = testCells ?? playerManager.playerActivePiece.cells;

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

    public void ClearActivePiece()
    {
        foreach (Vector2Int cell in playerManager.playerActivePiece.cells)
        {
            Vector3Int tilePos = new Vector3Int(playerManager.playerActivePiece.position.x + cell.x, playerManager.playerActivePiece.position.y + cell.y, 0);
            playerBoard.main_tilemap.SetTile(tilePos, null);
        }
    }
}
