using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class ScorchCursor : MonoBehaviour
{
    public Board_Manager board;
    public TileBase cursorImage;
    public Game_Manager gameManager;
    public Vector2Int position;
    public PlayerInput playerInput;
    private InputAction moveLeftAction;
    private InputAction moveRightAction;
    private InputAction moveDownAction;
    private InputAction moveUpAction;

    private float repeatTimerLR = 0f;
    private float repeatTimerUD = 0f;
    private float cursorInitialDelay = 0.15f; // Initial delay before repeat
    private float cursorRepeatRate = 0.05f;  // Delay between repeated moves

    private int holdLR = 0; // -1 for left, +1 for right, 0 for none
    private int holdUD = 0; // -1 for down, +1 for up, 0 for none
    private bool movedLR = false;
    private bool movedUD = false;

    private void Start()
    {
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

        moveDownAction = playerInput.actions.FindAction("Cursor Down");
        moveUpAction = playerInput.actions.FindAction("Cursor Up");
        moveLeftAction = playerInput.actions.FindAction("Cursor Left");
        moveRightAction = playerInput.actions.FindAction("Cursor Right");

        // Set initial position in the center of the board (bottom row, above the lowest bound)
        RectInt bounds = board.Bounds;
        position = new Vector2Int(0, bounds.yMin + 2); // yMin+2 so it's above the bottom but inside play area
        DrawCursor();
    }

    private void Update()
    {
        float moveSens = gameManager != null ? gameManager.GetMovementSensitivity() : 0.1f;
        repeatTimerLR += Time.deltaTime;
        repeatTimerUD += Time.deltaTime;
        Move(moveSens);
    }

    private Vector2Int GetClampedPosition(Vector2Int candidate)
    {
        RectInt bounds = board.Bounds;
        int x = Mathf.Clamp(candidate.x, bounds.xMin, bounds.xMax - 1);
        int y = Mathf.Clamp(candidate.y, bounds.yMin, bounds.yMax - 1);
        return new Vector2Int(x, y);
    }

    private void ClearCursor()
    {
        board.scorch_tilemap.ClearAllTiles();
    }

    private void DrawCursor()
    {
        Vector3Int tilePos = new Vector3Int(position.x, position.y, 0);
        board.scorch_tilemap.SetTile(tilePos, cursorImage);
    }

    public void Move(float moveSens = 0.1f)
    {
        if (moveLeftAction == null || moveRightAction == null || moveDownAction == null || moveUpAction == null)
            return;

        // Determine which direction is being held for each axis
        float left = moveLeftAction.ReadValue<float>();
        float right = moveRightAction.ReadValue<float>();
        float up = moveUpAction.ReadValue<float>();
        float down = moveDownAction.ReadValue<float>();

        int dirLR = 0;
        if (left >= 0.5f) dirLR = -1;
        if (right >=0.5f) dirLR = 1;
        int dirUD = 0;
        if (up >= 0.5f) dirUD = 1;
        if (down >= 0.5f) dirUD = -1;

        // Handle left/right movement with repeating
        if (dirLR != 0)
        {
            if (dirLR != holdLR)
            {
                // Direction changed or just pressed
                movedLR = false;
                repeatTimerLR = 0f;
                holdLR = dirLR;
            }
            if (!movedLR || repeatTimerLR >= moveSens)
            {
                Vector2Int candidate = position + new Vector2Int(holdLR, 0);
                candidate = GetClampedPosition(candidate);
                if (candidate != position)
                {
                    ClearCursor();
                    position = candidate;
                    DrawCursor();
                }
                repeatTimerLR = 0f;
                movedLR = true;
            }
        }
        else // release
        {
            holdLR = 0;
            movedLR = false;
            repeatTimerLR = 0f;
        }

        // Handle up/down movement with repeating
        if (dirUD != 0)
        {
            if (dirUD != holdUD)
            {
                movedUD = false;
                repeatTimerUD = 0f;
                holdUD = dirUD;
            }
            if (!movedUD || repeatTimerUD >= moveSens * 0.12f) // Up/down repeat a bit faster
            {
                Vector2Int candidate = position + new Vector2Int(0, holdUD);
                candidate = GetClampedPosition(candidate);
                if (candidate != position)
                {
                    ClearCursor();
                    position = candidate;
                    DrawCursor();
                }
                repeatTimerUD = 0f;
                movedUD = true;
            }
        }
        else
        {
            holdUD = 0;
            movedUD = false;
            repeatTimerUD = 0f;
        }
    }
}
