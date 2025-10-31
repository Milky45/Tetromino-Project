using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;
    public bool isMenu = true;
    public AudioManager audioManager;

    public void PausePanel()
    {
        if (!isMenu)
        {
            Time.timeScale = 0f;
        }
        pauseCanvas.SetActive(true);
        audioManager.VS_PauseMusic();
    }

    public void ResumeGame()
    {
        if (!isMenu)
        {
            Time.timeScale = 1f;
        }
        pauseCanvas.SetActive(false);
        audioManager.VS_UnPauseMusic();
    }

    public void MainMenu()
    {
        if (!isMenu)
        {
            Time.timeScale = 1f;
        }    
        CharacterSelect.currentlyPlaying = false;
        GameObject Player1 = GameObject.Find("Player 1");
        GameObject Player2 = GameObject.Find("Player 2");
        Destroy(Player1);
        Destroy(Player2);
        SceneManager.LoadScene("MainMenu");
    }
    
    public void VsLobby()
    {
        if (!isMenu)
        {
            Time.timeScale = 1f;
        }    
        CharacterSelect.currentlyPlaying = false;
        GameObject Player1 = GameObject.Find("Player 1");
        GameObject Player2 = GameObject.Find("Player 2");
        Destroy(Player1);
        Destroy(Player2);
        SceneManager.LoadScene("VS Lobby");
    }

    public void RestartScene()
    {
        if (!isMenu)
        {
            Time.timeScale = 1f;
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
