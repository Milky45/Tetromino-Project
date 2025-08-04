using UnityEngine;
using UnityEngine.InputSystem;

public class PieceControllerSolo : MonoBehaviour
{
    public BoardManager board;
    public TetrominoData data;
    public GameManagerSolo gameManager;

    public Vector2Int position;
    public Vector2Int[] cells;

    private float moveTimer = 0f;

    private float gravityDelay = 1.0f;
    private float gravityTimer = 0f;
    private Vector2Int[] ghostCells;

    P1Controller controls;
    InputAction moveLeft;
    InputAction moveRight;
    InputAction moveDown;

    private float repeatTimerLR = 0f; // For Left/Right
    private float repeatTimerDown = 0f; // For Down
    public void Start()
    {
        cells = new Vector2Int[data.cells.Length];
        ghostCells = new Vector2Int[data.cells.Length];
        cells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = data.cells[i];
        }

        Set();
    }

    public void Update()
    {
        if (!isActiveAndEnabled || gameManager.isPaused) return;

        moveTimer += Time.deltaTime;
        gravityTimer += Time.deltaTime;
        repeatTimerLR += Time.deltaTime;
        repeatTimerDown += Time.deltaTime;

        ClearGhost();
        Clear();

        gravityDelay = gameManager.GetGravityDelay();

        if (gravityTimer >= gravityDelay)
        {
            if (!TryMove(Vector2Int.down))
            {
                gameManager.ResetHold();
                LockPiece();
                return;
            }
            gravityTimer = 0f;
        }

        // Move Left
        if (moveLeft.ReadValue<float>() > 0 && repeatTimerLR >= gameManager.GetMovementSensitivity())
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.moveClip);
            TryMove(Vector2Int.left);
            repeatTimerLR = 0f;
        }

        // Move Right
        if (moveRight.ReadValue<float>() > 0 && repeatTimerLR >= gameManager.GetMovementSensitivity())
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.moveClip);
            TryMove(Vector2Int.right);
            repeatTimerLR = 0f;
        }

        // Soft Drop (faster than gravity)
        if (moveDown.ReadValue<float>() > 0 && repeatTimerDown >= (gameManager.GetMovementSensitivity() * 0.1f)) // Soft drop is 10x faster than horizontal movement
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.moveClip);
            TryMove(Vector2Int.down);
            repeatTimerDown = 0f;
        }
        else
        {
            repeatTimerDown = 0f; // Reset timer when not holding down
        }

        Set();
        DrawGhost();
    }

    public void Awake()
    {
        controls = new P1Controller();
        moveLeft = controls.SoloMode.MoveLeft;
        moveRight = controls.SoloMode.MoveRight;
        moveDown = controls.SoloMode.MoveDown;

        controls.SoloMode.HardDrop.performed += ctx => HardDrop();
        controls.SoloMode.RotateToLeft.performed += ctx => TryRotate(1);
        controls.SoloMode.RotateToRight.performed += ctx => TryRotate(-1);
        controls.SoloMode.Hold.performed += ctx => gameManager.TryHoldPiece(data, this);
        controls.SoloMode.Pause.performed += ctx => gameManager.TogglePause();
    }

    public void LockPiece()
    {
        gameManager.ComboCount();
        foreach (Vector2Int cell in cells)
        {
            int yPos = position.y + cell.y;
            if (yPos > 7)
            {
                gameManager.GameOver();
                Destroy(this.gameObject); // Just in case
                return;
            }
        }
        AudioManager.Instance.PlaySFX(AudioManager.Instance.dropClip);
        Destroy(this.gameObject); // Remove piece
        gameManager.SpawnNextPiece();
    }

    public bool TryMove(Vector2Int direction)
    {
        Clear();

        Vector2Int newPos = position + direction;

        if (IsValidPosition(newPos))
        {
            Clear();
            position = newPos;
            Set();
            return true;
        }

        Set();
        return false;
    }

    public void TryRotate(int direction)
    {
        Clear();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.rotateClip);
        Vector2Int[] rotatedCells = new Vector2Int[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            int x = cells[i].x;
            int y = cells[i].y;

            if (data.tetromino == TetrominoType.I)
            {
                float fx = x - 0.5f;
                float fy = y - 0.5f;

                int rx = Mathf.RoundToInt(-direction * fy + 0.5f);
                int ry = Mathf.RoundToInt(direction * fx + 0.5f);

                rotatedCells[i] = new Vector2Int(rx, ry);
            }
            else if (data.tetromino == TetrominoType.O)
            {
                Set();
                return;
            }
            else
            {
                rotatedCells[i] = new Vector2Int(-direction * y, direction * x);
            }
        }
        if (IsValidPosition(position, rotatedCells))
        {
            cells = rotatedCells;
            Set();
            return;
        }
        if (data.tetromino == TetrominoType.I)
        {
            Vector2Int[] iKicks = new Vector2Int[]
            {
            new Vector2Int(2, 0),
            new Vector2Int(-2, 0),
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            };

            foreach (var offset in iKicks)
            {
                Vector2Int testPos = position + offset;
                if (IsValidPosition(testPos, rotatedCells))
                {
                    position = testPos;
                    cells = rotatedCells;
                    Set();
                    return;
                }
            }
        }
        else // Wall kicks for other tetrominoes
        {
            Vector2Int[] genericKicks = new Vector2Int[]
            {
            new Vector2Int(1, 0),
            new Vector2Int(-1, 0),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(-1, 1)
            };

            foreach (var offset in genericKicks)
            {
                Vector2Int testPos = position + offset;
                if (IsValidPosition(testPos, rotatedCells))
                {
                    position = testPos;
                    cells = rotatedCells;
                    Set();
                    return;
                }
            }
        }
        Set();
    }

    private void HardDrop()
    {
        while (TryMove(Vector2Int.down))
        {
            continue;
        }

        // Lock the piece in place when it can't move further
        gameManager.ResetHold();
        LockPiece();
    }

    private void Set()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(position.x + cell.x, position.y + cell.y, 0);
            board.tilemap.SetTile(tilePos, data.tile);
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

    private bool IsValidPosition(Vector2Int pos, Vector2Int[] testCells = null)
    {
        Vector2Int[] checkCells = testCells ?? cells;

        foreach (Vector2Int cell in checkCells)
        {
            Vector3Int tilePos = new Vector3Int(pos.x + cell.x, pos.y + cell.y, 0);

            if (!board.IsInsideBoard(tilePos) || board.IsTileOccupied(tilePos))
            {
                return false;
            }
        }
        return true;
    }

    public void DrawGhost()
    {
        Vector2Int ghostPos = position;

        // Temporarily clear the board to not collide with itself
        Clear();

        // Drop the ghost down until it's no longer valid
        while (IsValidPosition(ghostPos + Vector2Int.down))
        {
            ghostPos += Vector2Int.down;
        }

        // Redraw the active piece
        Set();

        // Draw ghost at final ghost position
        for (int i = 0; i < cells.Length; i++)
        {
            Vector3Int tilePos = new Vector3Int(ghostPos.x + cells[i].x, ghostPos.y + cells[i].y, 0);
            board.ghostTilemap.SetTile(tilePos, data.ghostTile);
        }
    }

    public void ClearGhost()
    {
        board.ghostTilemap.ClearAllTiles(); // MUCH cleaner
    }

    public void OnEnable()
    {
        controls.SoloMode.Enable();
    }

    public void OnDisable()
    {
        controls.SoloMode.Disable();
    }
}
