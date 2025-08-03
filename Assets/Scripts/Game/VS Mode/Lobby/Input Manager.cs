using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager : MonoBehaviour
{
    public InputActionAsset P1Controller; // Drag P1Controller InputActionAsset here
    public InputActionAsset P2Controller; // Drag P2Controller InputActionAsset here

    public PlayerInput player1Input; // Assign your Player 1 PlayerInput component here
    public PlayerInput player2Input; // Assign your Player 2 PlayerInput component here

    private bool player1Joined = false;
    private bool player2Joined = false;

    private void Update()
    {
        foreach (var device in Gamepad.all)
        {
            if (!player1Joined && device.startButton.wasPressedThisFrame)
            {
                AssignPlayer1(device);
            }
            else if (player1Joined && !player2Joined && device.startButton.wasPressedThisFrame)
            {
                AssignPlayer2(device);
            }
        }
    }

    private void AssignPlayer1(Gamepad device)
    {
        player1Input.gameObject.SetActive(true);
        player1Input.actions = P1Controller;
        player1Input.defaultActionMap = "Vs Mode P1"; // <-- important
        player1Input.SwitchCurrentActionMap("Vs Mode P1");
        InputUser.PerformPairingWithDevice(device, player1Input.user);

        player1Joined = true;
        Debug.Log($"[InputManager] Player 1 joined using {device.displayName}");
    }

    private void AssignPlayer2(Gamepad device)
    {
        player2Input.gameObject.SetActive(true);
        player2Input.actions = P2Controller;
        player2Input.defaultActionMap = "Vs Mode P2"; // <-- important
        player2Input.SwitchCurrentActionMap("Vs Mode P2");
        InputUser.PerformPairingWithDevice(device, player2Input.user);

        player2Joined = true;
        Debug.Log($"[InputManager] Player 2 joined using {device.displayName}");
    }
}
