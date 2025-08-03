using UnityEngine;

[CreateAssetMenu(fileName = "NewBeatMap", menuName = "Audio/Beat Map")]
public class BeatMap : ScriptableObject
{
    public float[] beatTimes; // In seconds (e.g. [0.5f, 1.2f, 1.8f...])
}
