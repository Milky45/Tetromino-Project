using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LobbyManager : MonoBehaviour
{
    public static LobbyManager Instance; 
    public Animator cameraAnimator; // Reference to the camera animation
    public static int playerCtr = 0;
    public GameObject p1_startPnl;
    public GameObject p2_startPnl;
    public GameObject playBtn;

    public static bool p1Ready = false;
    public static bool p2Ready = false;

    private void Awake()
    {
        Instance = this;
        AudioManager.Instance.LobbyPlayMusic();
        p1_startPnl.SetActive(true);
        p2_startPnl.SetActive(false);
        playBtn.SetActive(false);
    }

    public void P1Panel()
    {
        p1_startPnl.SetActive(true);
    }

    public void P2Panel()
    {
        p1_startPnl.SetActive(false);
        p2_startPnl.SetActive(true);
    }

    public void HideAllPlayerPanel()
    {
        p1_startPnl.SetActive(false);
        p2_startPnl.SetActive(false);
    }

    public void ReadyBtn()
    {
        if (p1Ready && p2Ready)
        {
            playBtn.SetActive(true);
        }
        else
        {
            playBtn.SetActive(false);
        }
    }
    

    public void cameraAnimation()
    {
        if (cameraAnimator != null)
        {
            AudioManager.Instance.LobbyStopMusic();
            AudioManager.Instance.PlaySFX(AudioManager.Instance.GameStart);
            cameraAnimator.SetTrigger("Ready"); // Trigger the camera animation
            Debug.Log("Camera animation triggered.");
        }
    }

    public void StartGame()
    {
        CharacterSelect.currentlyPlaying = true;
        // Logic to start the game, e.g., loading the game scene
        Debug.Log("Starting game...");
        // Load the game scene or transition to the game state // Ensure the character select is ready before starting
        SceneManager.LoadScene("VsTetrominoGame"); // Replace with your actual game scene name
    }

    public void ExitLobby()
    {   
        CharacterSelect.currentlyPlaying = true;
        // Logic to exit the lobby, e.g., returning to the main menu
        Debug.Log("Exiting lobby...");
        // Load the main menu scene or perform other exit actions
    }

    public void DisableAllInputs()
    {
        // Stop new joins
        if (PlayerInputManager.instance != null)
        {
            PlayerInputManager.instance.enabled = false;
        }

        // Disable all PlayerInput components and their actions
        var allPlayerInputs = FindObjectsByType<PlayerInput>(FindObjectsSortMode.None);
        foreach (var playerInput in allPlayerInputs)
        {
            try
            {
                playerInput.DeactivateInput();
            }
            catch { }

            if (playerInput.actions != null)
            {
                playerInput.actions.Disable();
            }

            playerInput.enabled = false;
        }

        // As a safety net, disable any loose InputActionAssets in the scene
        var actionAssets = FindObjectsByType<InputActionAsset>(FindObjectsSortMode.None);
        foreach (var asset in actionAssets)
        {
            asset.Disable();
        }
    }
}
