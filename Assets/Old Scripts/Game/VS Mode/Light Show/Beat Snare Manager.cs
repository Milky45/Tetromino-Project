using System.Collections.Generic;
using UnityEngine;

public class BeatSnare : MonoBehaviour
{
    public AudioSource musicSource;
    public BorderVisuals snareBorder; // A different border set for snare hits

    public float bpm = 148f;
    public float barStartTime = 252.97f; // bar start at 4:12.97
    public float snareDuration = 999f; // Length from 4:12.97 to 6:32.43
    public int beatInBar = 2; // zero-based index, so 2 = 3rd beat of each bar

    private List<float> snareTimestamps = new List<float>();
    private int currentIndex = 0;

    void Start()
    {
        float beatInterval = 60f / bpm;
        float barInterval = beatInterval * 4;
        float snareOffset = beatInterval * beatInBar;

        float currentTime = barStartTime + snareOffset;
        float endTime = barStartTime + snareDuration;

        while (currentTime <= endTime)
        {
            snareTimestamps.Add(currentTime);
            currentTime += barInterval; // jump to next bar’s snare
        }
    }

    void Update()
    {
        if (currentIndex >= snareTimestamps.Count) return;

        if (musicSource.time >= snareTimestamps[currentIndex])
        {
            snareBorder.PulseSnare(); // Custom snare pulse style
            currentIndex++;
        }
    }
}

