using UnityEngine;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Users;

public class ControllerJoinManager : MonoBehaviour
{
    public GameObject controllerPrefab; // The visual sprite for controller (shared prefab)
    public Transform leftSlot, rightSlot; // World positions for left/right side

    private ControllerSelector controller1;
    private ControllerSelector controller2;

    private bool player1Joined = false;
    private bool player2Joined = false;

    void Update()
    {
        if (!player1Joined)
        {
            // Wait for any button press to join P1
            if (Gamepad.all[1].allControls.OfType<ButtonControl>().Any(ctrl => ctrl.wasPressedThisFrame))
            {
                AssignPlayer(Gamepad.all[0], true);
                player1Joined = true;
            }
        }
        else if (!player2Joined && Gamepad.all.Count > 1)
        {
            if (Gamepad.all[1].allControls.OfType<ButtonControl>().Any(ctrl => ctrl.wasPressedThisFrame))
            {
                AssignPlayer(Gamepad.all[1], false);
                player2Joined = true;
            }
        }

        if (player1Joined && player2Joined && controller1.IsConfirmed && controller2.IsConfirmed)
        {
            Debug.Log("Both players confirmed. Load VS Scene!");
            // Save chosen sides and proceed
        }
    }

    void AssignPlayer(Gamepad device, bool isPlayer1)
    {
        GameObject instance = Instantiate(controllerPrefab);
        ControllerSelector selector = instance.GetComponent<ControllerSelector>();

        // Create and bind a new InputUser
        var user = InputUser.CreateUserWithoutPairedDevices();
        InputUser.PerformPairingWithDevice(device, user);

        var actions = new Controller(); // your generated InputAction wrapper
        user.AssociateActionsWithUser(actions);
        actions.Enable();

        selector.inputUser = user;
        selector.moveAction = actions.VsModeP1.MoveRight; // or custom map per player
        selector.confirmAction = actions.VsModeP1.HardDrop; // or custom map per player
        selector.playerTag = isPlayer1 ? "P1" : "P2";

        selector.leftPosition = leftSlot.position;
        selector.rightPosition = rightSlot.position;

        if (isPlayer1) controller1 = selector;
        else controller2 = selector;
    }
}
