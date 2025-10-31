using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    public TMP_Text timerText;
    private string minutesText;
    private string secondsText;

    public float timeElapsed;

    private bool isRunning = false;

    public void Start()
    {
        isRunning = true; // Start the timer when the script is initialized
    }
    private void Update()
    {
        if (!isRunning) return; // Only tick if running

        timeElapsed += Time.deltaTime;

        int totalSeconds = Mathf.FloorToInt(timeElapsed);
        int mins = totalSeconds / 60;
        int secs = totalSeconds % 60;

        minutesText = mins.ToString("0");
        secondsText = secs.ToString("00");

        timerText.text = minutesText + ":" + secondsText;
    }

    public void StartTimer()
    {
        isRunning = true;
    }

    public void StopTimer()
    {
        isRunning = false;
    }

    public void ResetTimer()
    {
        timeElapsed = 0f;
        minutesText = "0";
        secondsText = "00";
        timerText.text = minutesText + ":" + secondsText;
    }

    public float GetElapsedTime()
    {
        return timeElapsed;
    }
}
