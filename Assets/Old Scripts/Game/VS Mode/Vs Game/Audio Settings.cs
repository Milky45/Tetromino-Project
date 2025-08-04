using UnityEngine;
using UnityEngine.UI;

public class AudioSettings : MonoBehaviour
{
    [Header("References")]
    public AudioManager audioManager;
    public Toggle sfxToggle;
    public Toggle musicToggle;

    private void Start()
    {
        AudioManagerGlobal.LoadStates();
        if (audioManager != null)
        {
            // Load SFX state
            if (sfxToggle != null)
            {
                // Set toggle to match SFX source mute state
                sfxToggle.isOn = AudioManagerGlobal.SFX_state;
                ToggleSFX(AudioManagerGlobal.SFX_state);
                // Add listener for toggle changes
                sfxToggle.onValueChanged.AddListener(ToggleSFX);
            }

            // Load Music state (checking both music sources)
            if (musicToggle != null)
            {
                musicToggle.isOn = AudioManagerGlobal.Music_state;
                ToggleMusic(AudioManagerGlobal.Music_state); // apply at start
                musicToggle.onValueChanged.AddListener(ToggleMusic);
            }
        }
        else
        {
            Debug.LogWarning("AudioManager not assigned to AudioSettings");
        }
    }

    private void OnDestroy()
    {
        if (sfxToggle != null)
        {
            sfxToggle.onValueChanged.RemoveListener(ToggleSFX);
        }
        if (musicToggle != null)
        {
            musicToggle.onValueChanged.RemoveListener(ToggleMusic);
        }
    }

    public void ToggleSFX(bool isMuted)
    {
        if (audioManager != null && audioManager.sfxSource != null)
        {
            audioManager.sfxSource.mute = isMuted;
            AudioManagerGlobal.SFX_state = isMuted;
            AudioManagerGlobal.SaveStates();
            Debug.Log($"SFX {(isMuted ? "Muted" : "Unmuted")}");
        }
    }

    public void ToggleMusic(bool isMuted)
    {
        if (audioManager != null)
        {
            audioManager.VsMusicSource.mute = isMuted;
            audioManager.LobbyMusicSource.mute = isMuted;
            AudioManagerGlobal.Music_state = isMuted;
            AudioManagerGlobal.SaveStates();
            Debug.Log($"Music {(isMuted ? "Muted" : "Unmuted")}");
        }
    }

}
