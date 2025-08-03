using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseBtnEventsSolo : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject OptionsCanvas;
    public GameObject AudioPanel;
    public GameObject Player;
    public GameObject gamePanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void OnOptionsClicked()
    {
        pauseMenu.SetActive(false);
        OptionsCanvas.SetActive(true);
    }

    public void OnAudioClicked()
    {
        Player.SetActive(false);
        gamePanel.SetActive(false);
        AudioPanel.SetActive(true);
    }

    public void OnPlayerClicked()
    {
        Player.SetActive(true);
        gamePanel.SetActive(false);
        AudioPanel.SetActive(false);
    }

    public void OnGameClicked()
    {
        Player.SetActive(false);
        gamePanel.SetActive(true);
        AudioPanel.SetActive(false);
    }

    public void OnBackClicked()
    {
        OptionsCanvas.SetActive(false);
        pauseMenu.SetActive(true);
    }

    public void OnRestartClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void OnVsLobbyClicked()
    {
        SceneManager.LoadScene("Vs Lobby");
    }
}
