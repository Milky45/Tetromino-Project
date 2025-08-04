using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeatSyncManager : MonoBehaviour
{
    public BorderVisuals borderVisuals;

    public float bpm = 148f;
    public float startTimeInSeconds = 64.86f;
    public float endTimeInSeconds = 999f;

    public float barStartTime = 240f; // 4:00
    public float barEndTime = 999f;   // Ensure this is longer than your actual song duration

    private AudioSource musicSource;
    private List<float> regularBeatTimestamps = new List<float>();
    private List<float> barBeatTimestamps = new List<float>();
    private int currentRegularIndex = 0;
    private int currentBarIndex = 0;
    private bool isInitialized = false;

    void Start()
    {
        StartCoroutine(InitializeBeatSync());
    }

    IEnumerator InitializeBeatSync()
    {
        // Wait until AudioManager and VsMusicSource are ready
        while (AudioManager.Instance == null || AudioManager.Instance.VsMusicSource == null)
            yield return null;

        musicSource = AudioManager.Instance.VsMusicSource;

        // Sanity check
        if (musicSource == null)
        {
            Debug.LogError("BeatSyncManager: musicSource is not assigned!");
            yield break;
        }

        float beatInterval = 60f / bpm;

        // Regular beat pulses
        float time = startTimeInSeconds;
        while (time <= endTimeInSeconds)
        {
            regularBeatTimestamps.Add(time);
            time += beatInterval;
        }

        // Bar-start pulses (1 every 4 beats)
        float barTime = barStartTime;
        float barInterval = beatInterval * 4;
        while (barTime <= barEndTime)
        {
            barBeatTimestamps.Add(barTime);
            barTime += barInterval;
        }

        isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized || musicSource == null) return;

        float musicTime = musicSource.time;

        // Regular beats
        if (currentRegularIndex < regularBeatTimestamps.Count && musicTime < barStartTime)
        {
            if (musicTime >= regularBeatTimestamps[currentRegularIndex])
            {
                borderVisuals.PulseColor();
                currentRegularIndex++;
            }
        }

        // Bar-start beats
        if (musicTime >= barStartTime && currentBarIndex < barBeatTimestamps.Count)
        {
            if (musicTime >= barBeatTimestamps[currentBarIndex])
            {
                borderVisuals.PulseHeavy();
                currentBarIndex++;
            }
        }
    }

    public void RestartBeatSync()
    {
        currentRegularIndex = 0;
        currentBarIndex = 0;
        if (musicSource != null)
        {
            musicSource.time = 0f;
            musicSource.Play();
        }
    }
}
