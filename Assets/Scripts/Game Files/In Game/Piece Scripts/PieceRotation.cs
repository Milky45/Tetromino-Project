using UnityEngine;
using UnityEngine.InputSystem;

public class PieceRotation : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PieceHelpers pieceHelpers;
    [SerializeField] private PieceMovement pieceMovement;
    private PlayerInput playerInput;
    public PlayerPiece activePiece;

    [Header("Input Actions")]
    private InputAction rotateLeftAction;
    private InputAction rotateRightAction;

    private void Awake()
    {
        playerInput = playerManager.playerInput;
        rotateLeftAction = playerInput.actions.FindAction("Rotate Left");
        rotateRightAction = playerInput.actions.FindAction("Rotate Right");
    }

    private void Update()
    {
        if (playerManager.playerActivePiece == null) return;

        // Poll single presses without callbacks
        if (rotateLeftAction != null && rotateLeftAction.WasPressedThisFrame())
        {
            TryRotate(-1);
        }
        else if (rotateRightAction != null && rotateRightAction.WasPressedThisFrame())
        {
            TryRotate(1);
        }
    }

    private void TryRotate(int direction)
    {
        if (playerManager.gameMaster != null && playerManager.gameMaster.isPaused) return;
        
        if (playerManager.audioManager != null)
        {
            playerManager.audioManager.PlaySFX(playerManager.audioManager.rotateClip);
        }

        pieceHelpers.ClearActivePiece();
        Vector2Int[] rotatedCells = new Vector2Int[playerManager.playerActivePiece.cells.Length];

        for (int i = 0; i < playerManager.playerActivePiece.cells.Length; i++)
        {
            int x = playerManager.playerActivePiece.cells[i].x;
            int y = playerManager.playerActivePiece.cells[i].y;

            if (playerManager.playerActivePiece.data.tetromino == TetrominoType.I)
            {
                // I piece rotation around center offset
                float fx = x - 0.5f;
                float fy = y - 0.5f;

                int rx = Mathf.RoundToInt(direction * fy + 0.5f);
                int ry = Mathf.RoundToInt(-direction * fx + 0.5f);

                rotatedCells[i] = new Vector2Int(rx, ry);
            }
            else if (playerManager.playerActivePiece.data.tetromino == TetrominoType.O)
            {
                pieceHelpers.Set();
                return;
            }
            else
            {
                // Standard 2D matrix rotation:
                // Clockwise (direction = 1): (x, y) -> (y, -x)
                // Counter-clockwise (direction = -1): (x, y) -> (-y, x)
                rotatedCells[i] = new Vector2Int(direction * y, -direction * x);
            }
        }

        // Try rotating in place first
        if (pieceHelpers.IsValidPosition(playerManager.playerActivePiece.position, rotatedCells))
        {
            pieceMovement.isMoving = true;
            pieceMovement.movingTimer = 1f;
            playerManager.playerActivePiece.cells = rotatedCells;
            pieceHelpers.Set();
            return;
        }

        // Special kicks for I piece (includes vertical floor/ceiling kicks)
        if (playerManager.playerActivePiece.data.tetromino == TetrominoType.I)
        {
            Vector2Int[] iKicks = new Vector2Int[]
            {
                new Vector2Int(1, 0),
                new Vector2Int(-1, 0),
                new Vector2Int(2, 0),
                new Vector2Int(-2, 0),
                new Vector2Int(0, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, 1),
                new Vector2Int(-1, 1),
                new Vector2Int(0, 2)
            };

            foreach (var offset in iKicks)
            {
                Vector2Int testPos = playerManager.playerActivePiece.position + offset;
                if (pieceHelpers.IsValidPosition(testPos, rotatedCells))
                {
                    pieceMovement.isMoving = true;
                    pieceMovement.movingTimer = 1f;
                    playerManager.playerActivePiece.position = testPos;
                    playerManager.playerActivePiece.cells = rotatedCells;
                    pieceHelpers.Set();
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
                new Vector2Int(-1, 1),
                new Vector2Int(0, -1),
                new Vector2Int(1, -1),
                new Vector2Int(-1, -1)
            };

            foreach (var offset in genericKicks)
            {
                Vector2Int testPos = playerManager.playerActivePiece.position + offset;
                if (pieceHelpers.IsValidPosition(testPos, rotatedCells))
                {
                    pieceMovement.isMoving = true;
                    pieceMovement.movingTimer = 1f;
                    playerManager.playerActivePiece.position = testPos;
                    playerManager.playerActivePiece.cells = rotatedCells;
                    pieceHelpers.Set();
                    return;
                }
            }
        }
        
        // Restore the old state if all failed
        pieceHelpers.Set();
    }
}