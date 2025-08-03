using UnityEngine;
using UnityEngine.SceneManagement; // <-- Required to load scenes
using TMPro;

public class PlayerReady : MonoBehaviour
{
    public bool p1ready = false;
    public bool p2ready = false;

    public TextMeshProUGUI P1Status;
    public TextMeshProUGUI P2Status;

    [Header("Optional")]
    public GameObject playButton;

    private void Start()
    {
        UpdateStatusText();
    }

    public void OnClickP1Ready()
    {
        if (CharacterSelector.P1ChosenCharacter == 0)
        {
            Debug.Log("P1 must select a character before readying.");
            return;
        }

        p1ready = !p1ready;
        UpdateStatusText();
    }


    public void OnClickP2Ready()
    {
        if (CharacterSelector.P2ChosenCharacter == 0)
        {
            Debug.Log("P2 must select a character before readying.");
            return;
        }

        p2ready = !p2ready;
        UpdateStatusText();
    }

    public void UpdateStatusText()
    {
        P1Status.text = p1ready ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";
        P2Status.text = p2ready ? "<color=green>Ready</color>" : "<color=red>Not Ready</color>";

        if (playButton != null)
        {
            playButton.SetActive(p1ready && p2ready);
        }
    }

    public void OnClickPlay()
    {
        if (!p1ready || !p2ready)
        {
            Debug.Log("Both players must be ready to start!");
            return;
        }

        // Load the VS game scene
        SceneManager.LoadScene("VsTetrominoGame"); // Make sure this name matches exactly
    }
}
