using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioSource VsMusicSource;
    public AudioSource LobbyMusicSource;

    [Header("Sound Effects")]
    public AudioSource sfxSource;
    public AudioClip moveClip;
    public AudioClip rotateClip;
    public AudioClip dropClip;
    public AudioClip holdClip;
    public AudioClip clear1;
    public AudioClip clear2;
    public AudioClip clear3;
    public AudioClip clear4;
    public AudioClip clear5;
    public AudioClip clear6;
    public AudioClip clear7;
    public AudioClip clear8;
    public AudioClip clear9;
    public AudioClip clear10;
    public AudioClip clear11;
    public AudioClip clear12;
    public AudioClip clear13;
    public AudioClip invalid;
    public AudioClip gameOverClip;
    public AudioClip attack;
    public AudioClip EMP_clip;
    public AudioClip Blind;

    public void Awake()
    {
        Debug.Log("AudioManager Awake");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Destroy");
            return;
        }

        // Apply the global mute states (DO NOT overwrite them here)
        if (sfxSource != null)
            sfxSource.mute = AudioManagerGlobal.SFX_state;

        if (VsMusicSource != null)
            VsMusicSource.mute = AudioManagerGlobal.Music_state;

        if (LobbyMusicSource != null)
            LobbyMusicSource.mute = AudioManagerGlobal.Music_state;
    }


    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void VSPlayMusic()
    {
        if (!VsMusicSource.isPlaying || LobbyMusicSource.isPlaying)
        {
            LobbyMusicSource.Stop();
            VsMusicSource.Play();
        }
    }
    public void LobbyPlayMusic()
    {
        if (!LobbyMusicSource.isPlaying || VsMusicSource.isPlaying)
        {
            VsMusicSource.Stop();
            LobbyMusicSource.Play();
        }
            
    }

    public void VSStopMusic()
    {
        VsMusicSource.Stop();
    }

    public void LobbyStopMusic()
    {
        LobbyMusicSource.Stop();
    }

    public void PauseMusic()
    {
        if (VsMusicSource.isPlaying)
        {
            VsMusicSource.Pause();
        }
    }

    public void UnpauseMusic()
    {
        if (!VsMusicSource.isPlaying)
        {
            VsMusicSource.UnPause();
        }
    }
}
