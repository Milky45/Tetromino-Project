using UnityEngine;

public class MenuAwake : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject OptionsCanvas;
    public GameObject AudioPanel;
    public GameObject P1panel;
    public GameObject P2panel;
    public GameObject VsPanel;

    void Awake()
    {
        AudioManagerGlobal.LoadStates();
    }

    public void OnOptionsClicked()
    {
        pauseMenu.SetActive(false);
        OptionsCanvas.SetActive(true);
    }

    public void OnAudioClicked()
    {
        P1panel.SetActive(false);
        P2panel.SetActive(false);
        VsPanel.SetActive(false);
        AudioPanel.SetActive(true);
    }

    public void OnP1Clicked()
    {
        P1panel.SetActive(true);
        P2panel.SetActive(false);
        VsPanel.SetActive(false);
        AudioPanel.SetActive(false);
    }

    public void OnP2Clicked()
    {
        P1panel.SetActive(false);
        P2panel.SetActive(true);
        VsPanel.SetActive(false);
        AudioPanel.SetActive(false);
    }

    public void OnVsClicked()
    {
        P1panel.SetActive(false);
        P2panel.SetActive(false);
        VsPanel.SetActive(true);
        AudioPanel.SetActive(false);
    }

    public void OnBackClicked()
    {
        OptionsCanvas.SetActive(false);
        pauseMenu.SetActive(true);
    }
}
