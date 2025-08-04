using UnityEngine;
using TMPro;

public class FPSmanager : MonoBehaviour
{
    public TextMeshProUGUI fpsText;

    private int[] frameRates = new int[] { 24, 30, 45, 60, 120, -1 }; // -1 means uncapped
    private int currentIndex = 3; // Default to 60 FPS

    void Start()
    {
        SetFrameRate(frameRates[currentIndex]);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Left click
        {
            NavigateLeft();
        }
        else if (Input.GetMouseButtonDown(1)) // Right click
        {
            NavigateRight();
        }
    }

    void NavigateLeft()
    {
        currentIndex = (currentIndex - 1 + frameRates.Length) % frameRates.Length;
        SetFrameRate(frameRates[currentIndex]);
    }

    void NavigateRight()
    {
        currentIndex = (currentIndex + 1) % frameRates.Length;
        SetFrameRate(frameRates[currentIndex]);
    }

    void SetFrameRate(int fps)
    {
        QualitySettings.vSyncCount = 0; // Disable VSync for manual control

        if (fps <= 0)
        {
            Application.targetFrameRate = -1;
            UpdateFPSText("No Limit");
            Debug.Log("Frame rate set to: No Limit");
        }
        else
        {
            Application.targetFrameRate = fps;
            UpdateFPSText($"{fps} FPS");
            Debug.Log($"Frame rate set to: {fps} FPS");
        }
    }

    void UpdateFPSText(string display)
    {
        if (fpsText != null)
        {
            fpsText.text = $"{display}";
        }
    }
}
