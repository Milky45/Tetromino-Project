using UnityEngine;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    public Tilemap tilemap; // Assign your existing Tilemap here
    public Tilemap ghostTilemap;
    public TileBase[] tileTypes; // Array of Tetromino tiles (e.g. Red, Blue, Green, etc.)
    public int LinesCleared { get; private set; } = 0;
    private int receivedDeadLineCount = 0;

    public Vector2Int boardSize = new Vector2Int(10, 24);

    public RectInt Bounds => new RectInt(-boardSize.x / 2, -boardSize.y / 2, boardSize.x, boardSize.y);

    public void SetTile(Vector3Int position, TileBase tile)
    {
        tilemap.SetTile(position, tile);
    }

    public void ClearTile(Vector3Int position)
    {
        tilemap.SetTile(position, null);
    }

    public bool IsInsideBoard(Vector3Int pos)
    {
        return Bounds.Contains((Vector2Int)pos);
    }

    public bool IsTileOccupied(Vector3Int pos)
    {
        return tilemap.HasTile(pos);
    }

    public void ClearAll()
    {
        tilemap.ClearAllTiles();
    }

    public void SpawnTetromino(Vector2Int spawnPosition, TetrominoData tetromino)
    {
        foreach (Vector2Int cell in tetromino.cells)
        {
            Vector3Int tilePosition = new Vector3Int(spawnPosition.x + cell.x, spawnPosition.y + cell.y, 0);

            if (IsInsideBoard(tilePosition))
            {
                tilemap.SetTile(tilePosition, tetromino.tile);
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
            ClearBottomDeadLines(linesClearedThisTurn);
        }

        LinesCleared += linesClearedThisTurn;
        return linesClearedThisTurn;
    }

    private bool IsLineFull(int y)
    {
        for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
        {
            Vector3Int pos = new Vector3Int(x, y - boardSize.y / 2, 0);
            if (!tilemap.HasTile(pos))
            {
                return false;
            }
        }
        return true;
    }

    private void ClearLine(int y)
    {
        for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
        {
            Vector3Int pos = new Vector3Int(x, y - boardSize.y / 2, 0);
            tilemap.SetTile(pos, null);
        }
    }

    public void PushUp()
    {
        for (int y = boardSize.y - 2; y >= 0; y--)
        {
            for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
            {
                Vector3Int from = new Vector3Int(x, y - boardSize.y / 2, 0);
                Vector3Int to = new Vector3Int(x, y + 1 - boardSize.y / 2, 0);

                TileBase tile = tilemap.GetTile(from);
                tilemap.SetTile(to, tile);
                tilemap.SetTile(from, null);
            }
        }
    }

    public void AddDeadLine()
    {
        int holeX = Random.Range(-boardSize.x / 2, boardSize.x / 2);
        int y = -boardSize.y / 2;

        for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
        {
            Vector3Int pos = new Vector3Int(x, y, 0);
            if (x == holeX)
            {
                tilemap.SetTile(pos, null);
            }
            else
            {
                tilemap.SetTile(pos, tileTypes[7]);
            }
        }

        receivedDeadLineCount++;
    }

    private void ClearBottomDeadLines(int linesCleared)
    {
        int linesToRemove = Mathf.Min(linesCleared, receivedDeadLineCount);

        for (int i = 0; i < linesToRemove; i++)
        {
            int y = -boardSize.y / 2;

            ClearLineFromWorldY(y);
        }

        receivedDeadLineCount -= linesToRemove;
    }

    private void ClearLineFromWorldY(int worldY)
    {
        for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
        {
            Vector3Int pos = new Vector3Int(x, worldY, 0);
            tilemap.SetTile(pos, null);
        }

        for (int y = worldY + 1; y < boardSize.y / 2; y++)
        {
            for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
            {
                Vector3Int from = new Vector3Int(x, y, 0);
                Vector3Int to = new Vector3Int(x, y - 1, 0);

                TileBase tile = tilemap.GetTile(from);
                tilemap.SetTile(to, tile);
                tilemap.SetTile(from, null);
            }
        }
    }

    private void MoveRowsDown(int fromY)
    {
        for (int y = fromY; y < boardSize.y - 1; y++)
        {
            for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
            {
                Vector3Int from = new Vector3Int(x, y + 1 - boardSize.y / 2, 0);
                Vector3Int to = new Vector3Int(x, y - boardSize.y / 2, 0);

                TileBase tile = tilemap.GetTile(from);
                tilemap.SetTile(to, tile);
                tilemap.SetTile(from, null);
            }
        }
    }

    public void ScorchSkillClearBottomLines()
    {
        var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();
        if (activePiece != null)
            activePiece.Clear();
        int linesToRemove = 5;
        int linesCleared = 0;
        int y = -boardSize.y / 2;

        while (linesCleared < linesToRemove && y < boardSize.y / 2)
        {
            bool hasAnyTile = false;
            bool isDeadLine = true;

            for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(pos))
                {
                    hasAnyTile = true;

                    if (tilemap.GetTile(pos) != tileTypes[7])
                    {
                        isDeadLine = false;
                    }
                }
            }

            if (hasAnyTile)
            {
                ClearLineFromWorldY(y);
                linesCleared++;

                if (isDeadLine)
                {
                    receivedDeadLineCount--;
                    receivedDeadLineCount = Mathf.Max(receivedDeadLineCount, 0);
                }
            }
            else
            {
                y++;
                continue;
            }
        }if (activePiece != null)
        {
            activePiece.Set();
        }
    }

}



