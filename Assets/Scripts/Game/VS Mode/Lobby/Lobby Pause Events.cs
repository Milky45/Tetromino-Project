using UnityEngine;
using UnityEngine.SceneManagement;


public class LobbyPauseEvents : MonoBehaviour
{
    public GameObject PauseCanvas;
    public GameObject VsLobbyCanvas;

    public void OnPauseClicked()
    {
        PauseCanvas.SetActive(true);
        VsLobbyCanvas.SetActive(false);
    }

    public void OnResumeClicked()
    {
        PauseCanvas.SetActive(false);
        VsLobbyCanvas.SetActive(true);
    }

    public void OnMainMenuClicked()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
