using UnityEngine;
using TMPro;

public class P1Settings : MonoBehaviour
{
    [Header("References")]
    public TextMeshProUGUI MoveSens_Text;
    public TextMeshProUGUI ShiftInterval_Text;
    public GameObject P1Setup;
    public GameObject button;

    public static float chosenSens = 0.1f; // Default sensitivity value
    private readonly float[] moveDelays = 
        {0.15f, 0.14f, 0.13f, 0.12f, 0.11f, 0.10f, 0.09f, 0.08f, 0.07f, 0.06f, 0.05f, 0.04f, 0.03f, 0.02f, 0.01f};
    private static int currentIndex = 6; // Default to "0.09f"

    public static int chosenInterval = 2; // Default interval value
    private readonly int[] ShiftInterval = { 1, 2, 3 };
    private static int currentIntervalIndex = 0; // Default to "1"

    private void Start()
    {
        // Load the current movementSensitivity from the game manager
        float currentSensitivity = GameManager.movementSensitivity;
        int currentGearShift = GearAnimatorP1.ShiftInterval;

        // Find the matching index in moveDelays array
        bool foundMatchSens = false;
        for (int i = 0; i < moveDelays.Length; i++)
        {
            if (Mathf.Approximately(currentSensitivity, moveDelays[i]))
            {
                currentIndex = i;
                foundMatchSens = true;
                break;
            }
        }

        bool foundMatchShift = false;
        for (int i = 0; i < ShiftInterval.Length; i++)
        {
            if (Mathf.Approximately(currentGearShift, ShiftInterval[i]))
            {
                currentIntervalIndex = i;
                foundMatchShift = true;
                break;
            }
        }

        if (!foundMatchSens)
        {
            currentIndex = 1;
        }

        if (!foundMatchShift)
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
    }

    public void OnClickRight()
    {
        currentIndex = (currentIndex + 1) % moveDelays.Length;
        Debug.Log($"Current Index: {currentIndex}");
        UpdateDisplay();
    }

    public void OnShiftLeft()
    {
        currentIntervalIndex = (currentIntervalIndex - 1 + ShiftInterval.Length) % ShiftInterval.Length;
        Debug.Log($"Current Interval Index: {currentIntervalIndex}");
        UpdateDisplay();
    }
    public void OnShiftRight()
    {
        currentIntervalIndex = (currentIntervalIndex + 1) % ShiftInterval.Length;
        Debug.Log($"Current Interval Index: {currentIntervalIndex}");
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        MoveSens_Text.text = (currentIndex + 1).ToString(); // Display "1" through "10"
        ShiftInterval_Text.text = (currentIntervalIndex + 1).ToString(); // Display the corresponding interval value
    }

    public void OnClickSubmit()
    {
        chosenSens = moveDelays[currentIndex]; // Set chosen sensitivity
        chosenInterval = ShiftInterval[currentIntervalIndex]; // Set chosen interval
        GameManager.movementSensitivity = chosenSens; // Update game manager sensitivity
        GearAnimatorP1.ShiftInterval = chosenInterval; // Update the interval for the animator
        PlayerPrefs.SetInt("P1SensitivityIndex", currentIndex); // Save index
        PlayerPrefs.SetInt("P1ShiftInterval", currentIntervalIndex); // Save interval
        PlayerPrefs.Save(); // Ensure it's written to disk
        Debug.Log($"P1 Move Sensitivity Set: {MoveSens_Text.text} (Delay = {chosenSens}, Array Index = {currentIndex})");
        Debug.Log($"P1 Shift Interval Set: {ShiftInterval[currentIntervalIndex]}");
    }

    public static float GetSensitivity()
    {
        Debug.Log($"P1 GetSensitivity: {chosenSens}");
        return chosenSens;
    }
    public static int GetShiftInterval()
    {
        Debug.Log($"P1 GetShiftInterval {chosenInterval}");
        return chosenInterval;
    }

    public void OnClickBack()
    {
        button.SetActive(true);
        P1Setup.SetActive(false);
    }

    public void OnClickSettings()
    {
        button.SetActive(false);
        P1Setup.SetActive(true);
    }
}
