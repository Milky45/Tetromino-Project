using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;

public class EventsPressed : MonoBehaviour
{
    public GameObject MainStartUpCanvas;
    public GameObject MainMenuCanvas;
    public GameObject SettingsCanvas;
    public GameObject CreditsCanvas;
    public GameObject PvPHelpCanvas;
    

    public void OnSoloModePressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("TetrominoGame");
    }

    public void OnVsModePressed()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Vs Lobby");
    }

    public void OnPlayButtonPressed()
    {
        MainStartUpCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);     
    }

    public void OnSettingsButtonPressed()
    {
        
        MainMenuCanvas.SetActive(false);
        SettingsCanvas.SetActive(true);
    }

    public void OnCreditsPressed()
    {
        CreditsCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
    }

    public void OnPvpPressed()
    {
        PvPHelpCanvas.SetActive(true);
        MainMenuCanvas.SetActive(false);
    }

    public void BackSettingsPressed()
    {
        SettingsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }

    public void BackMenuPressed()
    {
        MainMenuCanvas.SetActive(false);
        MainStartUpCanvas.SetActive(true);
        
    }

    public void BackCreditsPressed()
    {
        CreditsCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }

    public void BackPvpPressed()
    {
        PvPHelpCanvas.SetActive(false);
        MainMenuCanvas.SetActive(true);
    }
}
