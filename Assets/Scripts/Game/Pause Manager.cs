using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseCanvas;

    public void PausePanel()
    {
        Time.timeScale = 0f;
        pauseCanvas.SetActive(true);
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseCanvas.SetActive(false);
    }

    public void MainMenu()
    {
        Time.timeScale = 1f;
        CharacterSelect.currentlyPlaying = false;
        GameObject Player1 = GameObject.Find("Player 1");
        GameObject Player2 = GameObject.Find("Player 2");
        Destroy(Player1);
        Destroy(Player2);
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
