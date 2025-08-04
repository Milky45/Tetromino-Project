using UnityEngine;
using TMPro;

public class PlayerSettings : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI MoveSens_Text;

    // Movement sensitivity values (1-10) mapped to actual moveDelay values
    private readonly float[] moveDelays = {
        0.15f, 0.14f, 0.13f, 0.12f, 0.11f, 0.10f, 0.09f, 0.08f, 0.07f, 0.06f,
        0.05f, 0.04f, 0.03f, 0.02f, 0.01f
    };

    private static int currentIndex = 5; // Default to "0.09f"

    private void Start()
    {
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

        // If no match found, set to index 1 (0.09f)
        if (!foundMatch)
        {
            currentIndex = 6;
        }

        UpdateDisplay();
    }


    public void OnClickLeft()
    {
        currentIndex = (currentIndex - 1 + moveDelays.Length) % moveDelays.Length;

        Debug.Log($"Current Index: {currentIndex}");
        UpdateDisplay();
    }

    public void OnClickRight()
    {
        currentIndex = (currentIndex + 1) % moveDelays.Length;
        Debug.Log($"Current Index: {currentIndex}");
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        MoveSens_Text.text = (currentIndex + 1).ToString(); // Display "1" through "10"
    }

    public void OnClickSubmit()
    {
        GameManagerSolo.movementSensitivity = moveDelays[currentIndex];
        PlayerPrefs.SetInt("P1SensitivityIndex", currentIndex); // Save index
        PlayerPrefs.Save(); // Ensure it's written to disk
        Debug.Log($"GameManagerSolo Move Sensitivity Set: {MoveSens_Text.text} (Delay = {GameManagerSolo.movementSensitivity}, Array Index = {currentIndex})");
    }
}

