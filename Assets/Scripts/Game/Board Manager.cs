using UnityEngine;
using UnityEngine.Tilemaps;

public class Board_Manager : MonoBehaviour
{
    public Tilemap main_tilemap;
    public Tilemap ghost_tilemap;
    private TileBase[] tile_types;
    public Vector2Int boardSize = new Vector2Int(10, 24);
    public int LinesCleared { get; private set; } = 0;
    private int receivedDeadLineCount = 0;
    public RectInt Bounds => new RectInt(-boardSize.x / 2, -boardSize.y / 2, boardSize.x, boardSize.y);

    [SerializeField] private Player player; // Assign this in the inspector
    private bool isInitialized = false; // Track if tilemaps have been initialized

    private void Awake()
    {
        // If no player is assigned, try to find one automatically
        if (player == null)
        {
            player = FindFirstObjectByType<Player>();
            Debug.LogWarning("No Player assigned to Board Manager. Using first Player found in scene.");
        }
        
        if (player == null)
        {
            Debug.LogError("Player not found! Cannot initialize Board Manager.");
            return;
        }

        // Wait for the next frame to ensure Player.Awake() has completed
        StartCoroutine(InitializeAfterPlayerReady());
    }

    private System.Collections.IEnumerator InitializeAfterPlayerReady()
    {
        // Wait for the end of frame to ensure all Awake() methods have completed
        yield return new WaitForEndOfFrame();
        
        // Now initialize the tilemaps
        InitializeTilemaps();
    }

    private void InitializeTilemaps()
    {
        // Find the appropriate tilemaps based on player type
        string playerTag = player.isPlayer1 ? "P1" : "P2";
        Debug.Log($"{playerTag} is awake, initializing Board Manager...");
        
        // Find MainTileMap with P1/P2 tag
        GameObject mainTileMapObj = GameObject.Find($"MainTileMap{playerTag}");
        if (mainTileMapObj != null)
        {
            main_tilemap = mainTileMapObj.GetComponent<Tilemap>();
            if (main_tilemap == null)
            {
                Debug.LogError($"MainTileMap{playerTag} found but no Tilemap component!");
            }
        }
        else
        {
            Debug.LogError($"MainTileMap{playerTag} not found!");
        }

        // Find GhostTileMap with P1/P2 tag
        GameObject ghostTileMapObj = GameObject.Find($"GhostTileMap{playerTag}");

        if (ghostTileMapObj != null)
        {
            ghost_tilemap = ghostTileMapObj.GetComponent<Tilemap>();
            if (ghost_tilemap == null)
            {
                Debug.LogError($"GhostTileMap{playerTag} found but no Tilemap component!");
            }
        }
        else
        {
            Debug.LogError($"GhostTileMap{playerTag} not found!");
        }

        // Mark as initialized if both tilemaps were found
        if (main_tilemap != null && ghost_tilemap != null)
        {
            isInitialized = true;
            Debug.Log($"Board Manager initialized for {playerTag} - Main: {main_tilemap.name}, Ghost: {ghost_tilemap.name}");
        }
        else
        {
            Debug.LogError($"Board Manager failed to initialize for {playerTag} - missing tilemaps!");
        }
    }

    /// <summary>
    /// Set the player reference for this Board Manager
    /// </summary>
    /// <param name="playerRef">The Player object to associate with this Board Manager</param>
    public void SetPlayer(Player playerRef)
    {
        player = playerRef;
        Debug.Log($"Board Manager {gameObject.name} now associated with Player {playerRef.gameObject.name}");
    }

    /// <summary>
    /// Check if the Board Manager is ready to use
    /// </summary>
    public bool IsReady()
    {
        return isInitialized && main_tilemap != null && ghost_tilemap != null;
    }

    /// <summary>
    /// Wait for the Board Manager to be initialized (for coroutines)
    /// </summary>
    public System.Collections.IEnumerator WaitForInitialization()
    {
        while (!isInitialized)
        {
            yield return null;
        }
    }

    public void SetTile(Vector3Int position, TileBase tile)
    {
        if (!IsReady())
        {
            Debug.LogWarning("Board Manager not initialized yet. Cannot set tile.");
            return;
        }
        main_tilemap.SetTile(position, tile);
    }

    public void ClearTile(Vector3Int position)
    {
        if (!IsReady())
        {
            Debug.LogWarning("Board Manager not initialized yet. Cannot clear tile.");
            return;
        }
        main_tilemap.SetTile(position, null);
    }

    public bool IsInsideBoard(Vector3Int pos)
    {
        return Bounds.Contains((Vector2Int)pos);
    }

    public bool IsTileOccupied(Vector3Int pos)
    {
        if (!IsReady())
        {
            Debug.LogWarning("Board Manager not initialized yet. Cannot check tile occupation.");
            return false;
        }
        return main_tilemap.HasTile(pos);
    }

    public void ClearAll()
    {
        if (!IsReady())
        {
            Debug.LogWarning("Board Manager not initialized yet. Cannot clear all tiles.");
            return;
        }
        main_tilemap.ClearAllTiles();
    }

    public void SpawnTetromino(Vector2Int spawnPosition, TetrominoData tetromino)
    {
        if (!IsReady())
        {
            Debug.LogWarning("Board Manager not initialized yet. Cannot spawn tetromino.");
            return;
        }

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
            if (!main_tilemap.HasTile(pos))
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
            main_tilemap.SetTile(pos, null);
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

                TileBase tile = main_tilemap.GetTile(from);
                main_tilemap.SetTile(to, tile);
                main_tilemap.SetTile(from, null);
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
                main_tilemap.SetTile(pos, null);
            }
            else
            {
                main_tilemap.SetTile(pos, tile_types[7]);
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
            main_tilemap.SetTile(pos, null);
        }

        for (int y = worldY + 1; y < boardSize.y / 2; y++)
        {
            for (int x = -boardSize.x / 2; x < boardSize.x / 2; x++)
            {
                Vector3Int from = new Vector3Int(x, y, 0);
                Vector3Int to = new Vector3Int(x, y - 1, 0);

                TileBase tile = main_tilemap.GetTile(from);
                main_tilemap.SetTile(to, tile);
                main_tilemap.SetTile(from, null);
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

                TileBase tile = main_tilemap.GetTile(from);
                main_tilemap.SetTile(to, tile);
                main_tilemap.SetTile(from, null);
            }
        }
    }

}
