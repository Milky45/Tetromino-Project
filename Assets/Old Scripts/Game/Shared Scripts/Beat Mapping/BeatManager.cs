using UnityEngine;

public class BeatManager : MonoBehaviour
{
    public BeatMap beatMap;
    public AudioSource music;
    public Color[] beatColors;
    public SpriteRenderer[] borders;

    private int beatIndex = 0;


    private void Update()
    {
        if (!music.isPlaying || beatIndex >= beatMap.beatTimes.Length)
            return;

        if (music.time >= beatMap.beatTimes[beatIndex])
        {
            Color newColor = beatColors[Random.Range(0, beatColors.Length)];
            foreach (var border in borders)
            {
                border.color = newColor;
            }

            beatIndex++;
        }
    }

    public void ResetBeats()
    {
        beatIndex = 0;
    }
}

