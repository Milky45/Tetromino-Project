using UnityEngine;
using TMPro;

public class TimerDisplay : MonoBehaviour
{
    public TMP_Text minutes;
    public TMP_Text seconds;
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

        minutes.text = mins.ToString("0");
        seconds.text = secs.ToString("00");
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
        minutes.text = "0";
        seconds.text = "00";
    }

    public float GetElapsedTime()
    {
        return timeElapsed;
    }
}
