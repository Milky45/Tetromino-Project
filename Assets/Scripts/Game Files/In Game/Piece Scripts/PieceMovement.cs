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
    private float gravityTimer = 0f;
    private float repeatTimerLR = 0f; // For Left/Right
    private float repeatTimerDown = 0f; // For Down

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
        gravityTimer += Time.deltaTime;
        repeatTimerLR += Time.deltaTime;
        repeatTimerDown += Time.deltaTime;

        if (isMoving)
        {
            movingTimer -= Time.deltaTime;
            if (movingTimer <= 0f)
            {
                isMoving = false;
                movingTimer = 0f;
            }
        }

        ClearGhostPiece();
        ClearActivePiece();

        gravityDelay = playerBoard.currentgravityDelay;

        if (gravityTimer >= gravityDelay)
        {
            if (!TryMove(Vector2Int.down))
            {
                if (!isMoving)
                {
                    playerManager.playerStatus.holdUsed = false;
                    pieceHelpers.LockPiece();
                    return;
                }
            }
            gravityTimer = 0f;
            
        }
    }

    public bool TryMove(Vector2Int direction)
    {
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
        playerBoard.ghost_tilemap.ClearAllTiles(); // MUCH cleaner
    }

    public void ClearActivePiece()
    {
        foreach (Vector2Int cell in activePiece.cells)
        {
            Vector3Int tilePos = new Vector3Int(activePiece.position.x + cell.x, activePiece.position.y + cell.y, 0);
            playerBoard.main_tilemap.SetTile(tilePos, null);
        }
    }

    
}
