using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ControllerSelector : MonoBehaviour
{
    public string playerTag; // "P1" or "P2"
    public Vector3 leftPosition;
    public Vector3 rightPosition;

    public InputAction moveAction;
    public InputAction confirmAction;
    public InputUser inputUser;

    private bool isOnLeft = true;
    private bool isConfirmed = false;

    public bool IsConfirmed => isConfirmed;
    public bool IsLeft => isOnLeft;

    public void Initialize(InputDevice device, InputActionAsset inputActions)
    {
        // Set up player input device
        inputUser = InputUser.PerformPairingWithDevice(device);
        inputUser.AssociateActionsWithUser(inputActions);

        // Extract player-specific actions
        moveAction = inputActions.FindAction("Move");
        confirmAction = inputActions.FindAction("Confirm");

        moveAction.Enable();
        confirmAction.Enable();

        Debug.Log($"{playerTag} has joined with {device.displayName}");
    }

    void Update()
    {
        if (isConfirmed || moveAction == null || confirmAction == null) return;

        float move = moveAction.ReadValue<float>();

        if (move < -0.5f)
        {
            isOnLeft = true;
            transform.position = leftPosition;
        }
        else if (move > 0.5f)
        {
            isOnLeft = false;
            transform.position = rightPosition;
        }

        if (confirmAction.triggered)
        {
            isConfirmed = true;
            Debug.Log($"{playerTag} locked in on {(isOnLeft ? "Left" : "Right")} side!");
        }
    }
}
