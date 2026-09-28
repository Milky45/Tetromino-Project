using UnityEngine;
using UnityEngine.InputSystem;

public class PieceMovement : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerBoard playerBoard;
    [SerializeField] private PieceHelpers pieceHelpers;
    private PlayerInput playerInput;
    public PlayerPiece activePiece;

    [Header("Input Actions")]
    [SerializeField] private InputAction moveLeftAction;
    [SerializeField] private InputAction moveRightAction;
    [SerializeField] private InputAction moveDownAction;
    [SerializeField] private InputAction holdAction;
    [SerializeField] private InputAction hardDropAction;

    [Header("Active Piece State")]
    public bool isMoving = false;

    [Header("Piece Settings")]
    public float movingTimer = 1f;
    private float gravityDelay = 1.0f;
    public float gravityTimer = 0f;

    [Header("Input Handling Delays")]
    [SerializeField] private float dasDelay = 0.15f; // Delayed Auto-Shift delay before auto-repeat
    [SerializeField] private float arrRate = 0.03f;  // Auto Repeat Rate delay
    private float repeatTimerLR = 0f;
    private float repeatTimerDown = 0f;

    private void Awake()
    {
        playerInput = playerManager.playerInput;
        moveLeftAction = playerInput.actions.FindAction("Move Left");
        moveRightAction = playerInput.actions.FindAction("Move Right");
        moveDownAction = playerInput.actions.FindAction("Move Down");
        holdAction = playerInput.actions.FindAction("Hold");
        hardDropAction = playerInput.actions.FindAction("Hard Drop");
    }

    private void Update()
    {
        if (activePiece == null || gameMasterIsPaused()) return;

        gravityTimer += Time.deltaTime;

        if (isMoving)
        {
            movingTimer -= Time.deltaTime;
            if (movingTimer <= 0f)
            {
                isMoving = false;
                movingTimer = 0f;
            }
        }

        HandleInput();

        gravityDelay = playerBoard != null ? playerBoard.currentgravityDelay : 1.0f;
        if (gravityDelay <= 0f) gravityDelay = 1.0f;

        if (gravityTimer >= gravityDelay)
        {
            if (!TryMove(Vector2Int.down))
            {
                if (!isMoving)
                {
                    if (playerManager != null && playerManager.playerStatus != null)
                    {
                        playerManager.playerStatus.holdUsed = false;
                    }
                    if (pieceHelpers != null)
                    {
                        pieceHelpers.LockPiece();
                    }
                    return;
                }
            }
            
            // RESET GRAVITY TIMER
            gravityTimer = 0f; 
        }
    }

    private void HandleInput()
    {
        // --- HARD DROP ---
        if (hardDropAction != null && hardDropAction.WasPressedThisFrame())
        {
            HardDrop();
            return;
        }

        // --- HOLD PIECE ---
        if (holdAction != null && holdAction.WasPressedThisFrame())
        {
            playerManager.TryHoldPiece(activePiece.data, activePiece);
            return;
        }

        // --- LEFT / RIGHT MOVEMENT ---
        if (moveLeftAction != null && moveLeftAction.WasPressedThisFrame())
        {
            TryMove(Vector2Int.left);
            repeatTimerLR = dasDelay;
        }
        else if (moveRightAction != null && moveRightAction.WasPressedThisFrame())
        {
            TryMove(Vector2Int.right);
            repeatTimerLR = dasDelay;
        }
        else if ((moveLeftAction != null && moveLeftAction.IsPressed()) || (moveRightAction != null && moveRightAction.IsPressed()))
        {
            repeatTimerLR -= Time.deltaTime;
            if (repeatTimerLR <= 0f)
            {
                if (moveLeftAction.IsPressed()) TryMove(Vector2Int.left);
                else if (moveRightAction.IsPressed()) TryMove(Vector2Int.right);

                repeatTimerLR = arrRate;
            }
        }

        // --- SOFT DROP (MOVE DOWN) ---
        if (moveDownAction != null && moveDownAction.IsPressed())
        {
            repeatTimerDown -= Time.deltaTime;
            if (repeatTimerDown <= 0f)
            {
                if (TryMove(Vector2Int.down))
                {
                    gravityTimer = 0f; // Reset gravity timer when manually dropping down
                }
                repeatTimerDown = arrRate;
            }
        }
        else
        {
            repeatTimerDown = 0f;
        }
    }

    private void HardDrop()
    {
        while (TryMove(Vector2Int.down))
        {
            // Keep dropping down until reaching the floor/stack
        }

        playerManager.playerStatus.holdUsed = false;
        pieceHelpers.LockPiece();

        if (playerManager.audioManager != null)
        {
            playerManager.audioManager.PlaySFX(playerManager.audioManager.dropClip);
        }
    }

    private bool gameMasterIsPaused()
    {
        return playerManager != null && playerManager.gameMaster != null && playerManager.gameMaster.isPaused;
    }

    public bool TryMove(Vector2Int direction)
    {
        if (activePiece == null) return false;
        ClearActivePiece();

        Vector2Int newPos = activePiece.position + direction;

        if (pieceHelpers.IsValidPosition(newPos))
        {   
            if (direction != Vector2Int.down)
            {
                isMoving = true;
                movingTimer = 1f;
            }
            ClearActivePiece();
            activePiece.position = newPos;
            pieceHelpers.Set();
            
            return true;
        }

        pieceHelpers.Set();
        return false;
    }

    public void ClearGhostPiece()
    {
        playerBoard.ghost_tilemap.ClearAllTiles();
    }

    public void ClearActivePiece()
    {
        if (activePiece != null)
        {
            foreach (Vector2Int cell in activePiece.cells)
            {
                Vector3Int tilePos = new Vector3Int(activePiece.position.x + cell.x, activePiece.position.y + cell.y, 0);
                playerBoard.main_tilemap.SetTile(tilePos, null);
            }
        }
    }
}