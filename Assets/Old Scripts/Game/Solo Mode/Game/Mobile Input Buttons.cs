using UnityEngine;
using UnityEngine.InputSystem;

public class MobileInputButtons : MonoBehaviour
{
    public GameManagerSolo gameManagerSolo;
    private P1Controller controls;
    private InputAction moveLeft;
    private InputAction moveRight;
    private InputAction moveDown;

    private void Awake()
    {
        controls = new P1Controller();
        moveLeft = controls.SoloMode.MoveLeft;
        moveRight = controls.SoloMode.MoveRight;
        moveDown = controls.SoloMode.MoveDown;

        controls.SoloMode.HardDrop.performed += ctx => HardDrop();
        controls.SoloMode.RotateToLeft.performed += ctx => Rotateleft();
        controls.SoloMode.RotateToRight.performed += ctx => RotateRight();
        controls.SoloMode.Hold.performed += ctx => Switch();
    }

    public void MoveLeft()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        activePiece.TryMove(Vector2Int.left);
    }

    public void MoveRight()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        activePiece.TryMove(Vector2Int.right);
    }

    public void StopLeft()
    {
        // No need to do anything on stop
    }

    public void StopRight()
    {
        // No need to do anything on stop
    }

    public void SoftDrop()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        activePiece.TryMove(Vector2Int.down);
    }

    public void StopSoftDrop()
    {
        // No need to do anything on stop
    }

    public void HardDrop()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        while (activePiece.TryMove(Vector2Int.down))
        {
            continue;
        }
        gameManagerSolo.ResetHold();
        activePiece.LockPiece();
    }

    public void Rotateleft()
    { 
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        activePiece.TryRotate(1);
    }

    public void RotateRight()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        activePiece.TryRotate(-1);
    }

    public void Switch()
    {
        if (gameManagerSolo.isPaused || gameManagerSolo.isGameOver) return;
        var activePiece = GameObject.Find("ActivePiece")?.GetComponent<PieceControllerSolo>();
        if (activePiece == null) return;

        gameManagerSolo.TryHoldPiece(activePiece.data, activePiece);
    }

    private void OnEnable()
    {
        controls.SoloMode.Enable();
    }

    private void OnDisable()
    {
        controls.SoloMode.Disable();
    }
}
