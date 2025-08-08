using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Animator cameraAnimator; // Reference to the camera animation
    private void Start()
    {
        // Initialize lobby settings or UI here
        Debug.Log("Lobby Manager initialized.");
    }

    public void cameraAnimation()
    {
        if (cameraAnimator != null)
        {
            cameraAnimator.SetTrigger("Ready"); // Trigger the camera animation
            Debug.Log("Camera animation triggered.");
        }
    }

    public void StartGame()
    {
        // Logic to start the game, e.g., loading the game scene
        Debug.Log("Starting game...");
        // Load the game scene or transition to the game state
        CharacterSelect.ready = true; // Ensure the character select is ready before starting
        SceneManager.LoadScene("VsTetrominoGame"); // Replace with your actual game scene name
    }

    public void ExitLobby()
    {
        // Logic to exit the lobby, e.g., returning to the main menu
        Debug.Log("Exiting lobby...");
        // Load the main menu scene or perform other exit actions
    }
}
