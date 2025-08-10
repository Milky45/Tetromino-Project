using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using static TetrominoType;

public class Piece : MonoBehaviour
{
    public Board_Manager board;
    public TetrominoData data;
    public Game_Manager gameManager;

    public Vector2Int position;
    public Vector2Int[] cells;

    private float moveTimer = 0f;
    private float gravityDelay = 1.0f;
    private float gravityTimer = 0f;
    private Vector2Int[] ghostCells;

    // PlayerInput component for this piece
    private PlayerInput playerInput;
    
    // Input actions from the PlayerInput component
    private InputAction moveLeftAction;
    private InputAction moveRightAction;
    private InputAction moveDownAction;
    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;
    private InputAction holdAction;
    private InputAction hardDropAction;

    // Store delegate references for proper cleanup
    private System.Action<InputAction.CallbackContext> rotateLeftCallback;
    private System.Action<InputAction.CallbackContext> rotateRightCallback;
    private System.Action<InputAction.CallbackContext> holdCallback;
    private System.Action<InputAction.CallbackContext> hardDropCallback;

    private float repeatTimerLR = 0f; // For Left/Right
    private float repeatTimerDown = 0f; // For Down
    public void Start()
    {
        // Get the PlayerInput component from this GameObject or its parent
        if (gameManager.player.isPlayer1)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P1");
            playerInput = playerObj.GetComponent<PlayerInput>();
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P2");
            playerInput = playerObj.GetComponent<PlayerInput>();
        }

        // Get input actions from the PlayerInput component
        moveLeftAction = playerInput.actions.FindAction("Move Left");
        moveRightAction = playerInput.actions.FindAction("Move Right");
        moveDownAction = playerInput.actions.FindAction("Move Down");
        rotateLeftAction = playerInput.actions.FindAction("Rotate Left");
        rotateRightAction = playerInput.actions.FindAction("Rotate Right");
        holdAction = playerInput.actions.FindAction("Hold");
        hardDropAction = playerInput.actions.FindAction("Hard Drop");

        // Create delegate references for proper cleanup
        rotateLeftCallback = ctx => TryRotate(1);
        rotateRightCallback = ctx => TryRotate(-1);
        holdCallback = ctx => gameManager.TryHoldPiece(data, this);
        hardDropCallback = ctx => HardDrop();

        // Subscribe to input events
        if (rotateLeftAction != null)
            rotateLeftAction.performed += rotateLeftCallback;
        if (rotateRightAction != null)
            rotateRightAction.performed += rotateRightCallback;
        if (holdAction != null)
            holdAction.performed += holdCallback;
        if (hardDropAction != null)
            hardDropAction.performed += hardDropCallback;

        cells = new Vector2Int[data.cells.Length];
        ghostCells = new Vector2Int[data.cells.Length];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = data.cells[i];
        }

        Set();
    }

    private void Update()
    {
        if (!isActiveAndEnabled) return;
        if (gameManager.isPaused) return;
        if (gameManager.isTimeStopped == true) return;

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

        // Handle horizontal movement with invert logic
        Vector2Int horizontalMove = Vector2Int.zero;

        if (moveLeftAction != null && moveLeftAction.ReadValue<float>() > 0)
        {
            horizontalMove = Vector2Int.left;
        }


        if (moveRightAction != null && moveRightAction.ReadValue<float>() > 0)
        { horizontalMove = Vector2Int.right; }

        // Apply inverted control if active
        if (horizontalMove != Vector2Int.zero && repeatTimerLR >= gameManager.GetMovementSensitivity())
        {
            if (gameManager.player.isInverted)
                horizontalMove *= -1;

            TryMove(horizontalMove);
            repeatTimerLR = 0f;
        }

        // Soft Drop (faster than gravity)
        if (moveDownAction != null && moveDownAction.ReadValue<float>() > 0)
        {
            if (repeatTimerDown >= (gameManager.GetMovementSensitivity() * 0.1f)) // Soft drop is 10x faster than horizontal movement
            {
                TryMove(Vector2Int.down);
                AudioManager.Instance.PlaySFX(AudioManager.Instance.moveClip);
                repeatTimerDown = 0f;
            }
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
        // PlayerInput component will be set up in Start()
    }

    public void LockPiece()
    {
        // Prevent locking and spawning a new piece if game is already over
        if (gameManager.isGameOver)
        {
            Debug.Log("Destroyed");
            Destroy(this.gameObject);
            return;
        }

        gameManager.ComboCount();

        foreach (Vector2Int cell in cells)
        {
            int yPos = position.y + cell.y;
            if (yPos > 7)
            {
                gameManager.LoseLife();
                Destroy(this.gameObject); // Just in case
                return;
            }
        }

        Destroy(this.gameObject); // Remove current piece
        gameManager.SpawnNextPiece(); // Don't do this if game is ending!
    }

    public bool TryMove(Vector2Int direction)
    {
        if (direction != Vector2Int.down)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.moveClip);
        }
        
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

    private void TryRotate(int direction)
    {
        if (gameManager.isPaused) return;
        if (gameManager.isTimeStopped == true) return;
        
        AudioManager.Instance.PlaySFX(AudioManager.Instance.rotateClip);
        Clear();
        Vector2Int[] rotatedCells = new Vector2Int[cells.Length];

        for (int i = 0; i < cells.Length; i++)
        {
            int x = cells[i].x;
            int y = cells[i].y;

            if (data.tetromino == TetrominoType.I)
            {
                // I piece rotates around its center (0.5 offset)
                float fx = x - 0.5f;
                float fy = y - 0.5f;

                int rx = Mathf.RoundToInt(-direction * fy + 0.5f);
                int ry = Mathf.RoundToInt(direction * fx + 0.5f);

                rotatedCells[i] = new Vector2Int(rx, ry);
            }
            else if (data.tetromino == TetrominoType.O) // o block shouldn't rotate
            {
                Set();
                return;
            }
            else
            {
                // Normal rotation (standard SRS)
                rotatedCells[i] = new Vector2Int(-direction * y, direction * x);
            }
        }

        // Try rotating in place first
        if (IsValidPosition(position, rotatedCells))
        {
            cells = rotatedCells;
            Set();
            return;
        }

        // Special horizontal kicks for I piece
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
        
        // Restore the old state if all failed
        Set();
    }

    private void HardDrop()
    {
        if (gameManager.isPaused) return;
        if (gameManager.isTimeStopped == true) return;

        while (TryMove(Vector2Int.down))
        {
            // Keep moving down while it's valid
            continue;
        }

        // Lock the piece in place when it can't move further
        gameManager.TriggerHardDropLockout();
        gameManager.ResetHold();
        LockPiece();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.dropClip);
    }

    public void Set()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(position.x + cell.x, position.y + cell.y, 0);
            board.main_tilemap.SetTile(tilePos, data.tile);
        }
    }

    public void Clear()
    {
        foreach (Vector2Int cell in cells)
        {
            Vector3Int tilePos = new Vector3Int(position.x + cell.x, position.y + cell.y, 0);
            board.main_tilemap.SetTile(tilePos, null);
        }
    }

    public bool IsValidPosition(Vector2Int pos, Vector2Int[] testCells = null)
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
            board.ghost_tilemap.SetTile(tilePos, data.ghostTile);
        }
    }

    public void ClearGhost()
    {
        board.ghost_tilemap.ClearAllTiles(); // MUCH cleaner
    }

    public void OnEnable()
    {
        // PlayerInput component handles enabling/disabling automatically
        // No need to manually enable/disable actions
    }

    public void OnDisable()
    {
        // Unsubscribe from input events to prevent callbacks on destroyed objects
        if (rotateLeftAction != null && rotateLeftCallback != null)
            rotateLeftAction.performed -= rotateLeftCallback;
        if (rotateRightAction != null && rotateRightCallback != null)
            rotateRightAction.performed -= rotateRightCallback;
        if (holdAction != null && holdCallback != null)
            holdAction.performed -= holdCallback;
        if (hardDropAction != null && hardDropCallback != null)
            hardDropAction.performed -= hardDropCallback;
    }
}
