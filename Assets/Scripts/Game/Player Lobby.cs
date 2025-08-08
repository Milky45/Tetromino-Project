using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerLobby : MonoBehaviour
{
    public GameObject player1Prefab;
    public GameObject player2Prefab;
    private int playerCount = 0;

    private void Start()
    {
        PlayerInputManager.instance.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
        PlayerInputManager.instance.onPlayerJoined += OnPlayerJoined;
    }

    private void Update()
    {
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TryJoin(Keyboard.current);
        }

        foreach (var pad in Gamepad.all)
        {
            if (pad.startButton.wasPressedThisFrame)
            {
                TryJoin(pad);
            }
        }
    }

    private void TryJoin(InputDevice device)
    {
        if (PlayerInput.all.Any(p => p.devices.Contains(device))) return;

        PlayerInput.Instantiate(player1Prefab, playerCount, controlScheme: null, pairWithDevice: device);
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        // Destroy auto placeholder
        Destroy(input.gameObject);

        GameObject selectedPrefab = (playerCount == 0) ? player1Prefab : player2Prefab;

        PlayerInput newInput = PlayerInput.Instantiate(
            selectedPrefab,
            playerIndex: playerCount,
            controlScheme: null,
            pairWithDevice: input.devices[0]
        );

        playerCount++;
    }
}
