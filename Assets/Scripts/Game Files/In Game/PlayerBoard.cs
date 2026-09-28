using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerBoard : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public Shaker shaker;

    [Header("Tilemaps")]
    public Tilemap main_tilemap;
    public Tilemap ghost_tilemap;
    public Tilemap opponentScorch_tilemap;
    public TileBase[] tile_types;
    public Vector2Int boardSize = new Vector2Int(10, 24);

    [Header("Misc")]
    public float currentgravityDelay;
    public int LinesCleared { get; private set; } = 0;
    public int receivedDeadLineCount = 0;
    public RectInt Bounds => new RectInt(-boardSize.x / 2, -boardSize.y / 2, boardSize.x, boardSize.y);

    public bool IsInsideBoard(Vector3Int pos)
    {
        return Bounds.Contains((Vector2Int)pos);
    }

    public bool IsTileOccupied(Vector3Int pos)
    {
        return main_tilemap.HasTile(pos);
    }

    public void ClearAll()
    {
        main_tilemap.ClearAllTiles();
    }

    public void SpawnTetromino(Vector2Int spawnPosition, TetrominoData tetromino)
    {
        foreach (Vector2Int cell in tetromino.cells)
        {
            Vector3Int tilePosition = new Vector3Int(spawnPosition.x + cell.x, spawnPosition.y + cell.y, 0);

            if (IsInsideBoard(tilePosition))
            {
                main_tilemap.SetTile(tilePosition, tetromino.tile);
            }
        }
    }

    public int ClearLines()
    {
        int linesClearedThisTurn = 0;

        for (int y = 0; y < boardSize.y; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                MoveRowsDown(y);
                y--;
                linesClearedThisTurn++;
            }
        }

        if (linesClearedThisTurn > 0)
        {
            receivedDeadLineCount = Mathf.Max(0, receivedDeadLineCount - linesClearedThisTurn);
        }

        LinesCleared += linesClearedThisTurn;
        return linesClearedThisTurn;
    }

    private bool IsLineFull(int y)
    {
        int minY = -boardSize.y / 2;
        int minX = -boardSize.x / 2;
        int maxX = boardSize.x / 2;

        for (int x = minX; x < maxX; x++)
        {
            Vector3Int pos = new Vector3Int(x, y + minY, 0);
            if (!main_tilemap.HasTile(pos))
            {
                return false;
            }
        }
        return true;
    }

    private void ClearLine(int y)
    {
        int minY = -boardSize.y / 2;
        int minX = -boardSize.x / 2;
        int maxX = boardSize.x / 2;

        for (int x = minX; x < maxX; x++)
        {
            Vector3Int pos = new Vector3Int(x, y + minY, 0);
            main_tilemap.SetTile(pos, null);
        }
    }

    public void PushUp()
    {
        int minY = -boardSize.y / 2;
        int maxY = boardSize.y / 2;
        int minX = -boardSize.x / 2;
        int maxX = boardSize.x / 2;

        for (int y = maxY - 2; y >= minY; y--)
        {
            for (int x = minX; x < maxX; x++)
            {
                Vector3Int from = new Vector3Int(x, y, 0);
                Vector3Int to = new Vector3Int(x, y + 1, 0);

                TileBase tile = main_tilemap.GetTile(from);
                if (to.y < maxY)
                {
                    main_tilemap.SetTile(to, tile);
                }
                main_tilemap.SetTile(from, null);
            }
        }
    }

    public void AddDeadLine()
    {
        int minX = -boardSize.x / 2;
        int maxX = boardSize.x / 2;
        int holeX = Random.Range(minX, maxX);
        int y = -boardSize.y / 2;

        TileBase garbageTile = (tile_types != null && tile_types.Length > 0) 
            ? tile_types[Mathf.Clamp(7, 0, tile_types.Length - 1)] 
            : null;

        for (int x = minX; x < maxX; x++)
        {
            Vector3Int pos = new Vector3Int(x, y, 0);
            if (x == holeX)
            {
                main_tilemap.SetTile(pos, null);
            }
            else
            {
                main_tilemap.SetTile(pos, garbageTile);
            }
        }

        receivedDeadLineCount++;
    }

    private void MoveRowsDown(int fromY)
    {
        int minY = -boardSize.y / 2;
        int minX = -boardSize.x / 2;
        int maxX = boardSize.x / 2;

        for (int y = fromY; y < boardSize.y - 1; y++)
        {
            for (int x = minX; x < maxX; x++)
            {
                Vector3Int from = new Vector3Int(x, y + 1 + minY, 0);
                Vector3Int to = new Vector3Int(x, y + minY, 0);

                TileBase tile = main_tilemap.GetTile(from);
                main_tilemap.SetTile(to, tile);
                main_tilemap.SetTile(from, null);
            }
        }
    }

    public void ApplyDeadLine()
    {
        var activePiece = playerManager.playerActivePiece;

        if (activePiece != null)
            activePiece.pieceHelpers.ClearActivePiece();

        PushUp();
        AddDeadLine();

        if (activePiece != null)
        {
            if (!activePiece.IsValidPosition(activePiece.position))
            {
                activePiece.pieceHelpers.LockPiece(); // Lock if overlapping right away
            }
            else
            {
                if (!activePiece.pieceMovement.TryMove(Vector2Int.down))
                {
                    activePiece.pieceHelpers.LockPiece(); // Lock if resting
                }
                else
                {
                    activePiece.pieceHelpers.Set(); // Update ghost/position
                }
            }
        }
    }
}
