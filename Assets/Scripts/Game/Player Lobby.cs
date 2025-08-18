using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PlayerLobby : MonoBehaviour
{
    public static PlayerLobby instance;
    public static int playerCount = 0;

    private void Start()
    {
        UpdateLobbyPanels();
    }

    public static void UpdateLobbyPanels()
    {
        if (LobbyManager.Instance == null) return;

        if (playerCount <= 0)
        {
            LobbyManager.Instance.P1Panel();
        }
        else if (playerCount == 1)
        {
            LobbyManager.Instance.P2Panel();
        }
        else
        {
            LobbyManager.Instance.HideAllPlayerPanel();
        }
    }
}
