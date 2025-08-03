using TMPro;
using UnityEngine;

public class RoomSettingsSoloMode : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI Gravity_Text;
    public TextMeshProUGUI CRT_Text;
    public TextMeshProUGUI ProgressiveMode_Text;
    public CRTFilterEffect crtFilter;
    public TextMeshProUGUI SavedText;

    // Settings options and values
    private readonly string[] gravityOptions = { "Easy", "Normal", "Hard", "Insane", "Crazy" };
    private readonly float[] gravityDelays = { 0.8f, 0.7f, 0.6f, 0.5f, 0.4f };

    // Progressive Mode options
    private readonly string[] progressiveModeOptions = { "Time Chase", "Classic" };

    // Internal state tracking
    private int gravityIndex = 0; // Default: Easy
    private int progressiveModeIndex = 1; // Default: Enabled
    private bool crtEnabled;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Load the current gravity delay from game manager
        SavedText.text = "";
        float gravity = GameManagerSolo.initialGravityDelay;
        
        // Find the matching index in gravityDelays array
        bool foundMatch = false;
        for (int i = 0; i < gravityDelays.Length; i++)
        {
            if (Mathf.Approximately(gravity, gravityDelays[i]))
            {
                gravityIndex = i;
                foundMatch = true;
                break;
            }
        }
        
        // If no match found, set to Easy (index 0)
        if (!foundMatch)
        {
            gravityIndex = 0;
        }

        // Load the current CRT filter state
        if (crtFilter != null)
        {
            crtEnabled = crtFilter.enabled;
            UpdateCRTDisplay();
        }

        // Load progressive mode state
        progressiveModeIndex = GameManagerSolo.progressiveGravityEnabled ? 1 : 0;
        
        UpdateDisplayTexts();
    }

    public void OnClickLeftGravity() => CycleOption(ref gravityIndex, gravityOptions.Length, -1, Gravity_Text);

    public void OnClickRightGravity() => CycleOption(ref gravityIndex, gravityOptions.Length, 1, Gravity_Text);

    public void OnClickLeftProgressiveMode() => CycleOption(ref progressiveModeIndex, progressiveModeOptions.Length, -1, ProgressiveMode_Text);

    public void OnClickRightProgressiveMode() => CycleOption(ref progressiveModeIndex, progressiveModeOptions.Length, 1, ProgressiveMode_Text);

    public void OnClickLeftCRT()
    {
        crtEnabled = !crtEnabled;
        if (crtFilter != null)
        {
            crtFilter.enabled = crtEnabled;
        }
        UpdateCRTDisplay();
        SavedText.text = "";
    }

    public void OnClickRightCRT()
    {
        crtEnabled = !crtEnabled;
        if (crtFilter != null)
        {
            crtFilter.enabled = crtEnabled;
        }
        UpdateCRTDisplay();
        SavedText.text = "";
    }

    private void CycleOption(ref int index, int length, int direction, TextMeshProUGUI display)
    {
        index = (index + direction + length) % length;
        UpdateDisplayTexts();
        SavedText.text = "";
    }

    private void UpdateDisplayTexts()
    {
        Gravity_Text.text = gravityOptions[gravityIndex];
        ProgressiveMode_Text.text = progressiveModeOptions[progressiveModeIndex];
    }

    private void UpdateCRTDisplay()
    {
        CRT_Text.text = crtEnabled ? "On" : "Off";
    }

    public void OnClickSubmit()
    {
        float gravity = gravityDelays[gravityIndex];

        // Apply settings
        GameManagerSolo.initialGravityDelay = gravity;
        GameManagerSolo.progressiveGravityEnabled = progressiveModeIndex == 1;

        Debug.Log($"Settings Applied:\nGravity = {gravity}s\nProgressive Mode = {progressiveModeOptions[progressiveModeIndex]}");
        SavedText.text = "Saved!";
    }
}
