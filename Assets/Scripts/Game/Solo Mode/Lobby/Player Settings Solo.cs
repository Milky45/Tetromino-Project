using UnityEngine;
using TMPro;

public class PlayerSettingsSolo : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI MoveSens_Text;
    public TextMeshProUGUI SavedText;

    public static float chosenSens = 0.1f; // Default sensitivity value
    private readonly float[] moveDelays = {
        0.10f, 0.09f, 0.08f, 0.07f, 0.06f,
        0.05f, 0.04f, 0.03f, 0.02f, 0.01f
    };

    private static int currentIndex = 1; // Default to "0.09f"

    private void Start()
    {
        SavedText.text = "";
        // Load the current movementSensitivity from the game manager
        float currentSensitivity = GameManagerSolo.movementSensitivity;
        
        // Find the matching index in moveDelays array
        bool foundMatch = false;
        for (int i = 0; i < moveDelays.Length; i++)
        {
            if (Mathf.Approximately(currentSensitivity, moveDelays[i]))
            {
                currentIndex = i;
                foundMatch = true;
                break;
            }
        }
       
        if (!foundMatch)
        {
            currentIndex = 1;
        }
        
        UpdateDisplay();
    }

    public void OnClickLeft()
    {
        currentIndex = (currentIndex - 1 + moveDelays.Length) % moveDelays.Length;
        Debug.Log($"Current Index: {currentIndex}");
        UpdateDisplay();
        SavedText.text = "";
    }

    public void OnClickRight()
    {
        currentIndex = (currentIndex + 1) % moveDelays.Length;
        Debug.Log($"Current Index: {currentIndex}");
        UpdateDisplay();
        SavedText.text = "";
    }

    private void UpdateDisplay()
    {
        MoveSens_Text.text = (currentIndex + 1).ToString(); // Display "1" through "10"
    }

    public void OnClickSubmit()
    {
        chosenSens = moveDelays[currentIndex]; // Set chosen sensitivity
        GameManagerSolo.movementSensitivity = chosenSens; // Update game manager sensitivity
        PlayerPrefs.SetInt("SoloSensitivityIndex", currentIndex); // Save index
        PlayerPrefs.Save(); // Ensure it's written to disk
        Debug.Log($"Solo Move Sensitivity Set: {MoveSens_Text.text} (Delay = {chosenSens}, Array Index = {currentIndex})");
        SavedText.text = "Saved!";
    }

    public static float GetSensitivity()
    {
        return chosenSens;
    }

}
