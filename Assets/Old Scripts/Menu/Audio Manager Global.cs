using UnityEngine;

public class AudioManagerGlobal : MonoBehaviour
{
    public static bool SFX_state;
    public static bool Music_state;

    private const string SFX_KEY = "SFX_MUTE";
    private const string MUSIC_KEY = "MUSIC_MUTE";

    // Load saved states from PlayerPrefs
    public void Awake()
    {
        LoadStates();
    }
    public static void LoadStates()
    {
        SFX_state = PlayerPrefs.GetInt(SFX_KEY, 0) == 1;
        Music_state = PlayerPrefs.GetInt(MUSIC_KEY, 0) == 1;
    }

    // Save current states to PlayerPrefs
    public static void SaveStates()
    {
        PlayerPrefs.SetInt(SFX_KEY, SFX_state ? 1 : 0);
        PlayerPrefs.SetInt(MUSIC_KEY, Music_state ? 1 : 0);
        PlayerPrefs.Save();
    }
}
