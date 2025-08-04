using UnityEngine;
using UnityEngine.InputSystem;

public class GamePauseSolo : MonoBehaviour
{
    public static GamePauseSolo Instance { get; private set; }
    public GameManagerSolo gameManager;
    public bool IsPaused { get; private set; } = false;
    [SerializeField] private GameObject pauseMenuUI;
    void Start()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void TogglePause()
    {
        IsPaused = !IsPaused;
        gameManager.isPaused = IsPaused;
        if (IsPaused == true)
        {
            Time.timeScale = 0f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
            AudioManager.Instance.PauseMusic();
            Debug.Log("Game Paused");
            pauseMenuUI.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            AudioManager.Instance.UnpauseMusic();
            Debug.Log("Game Resumed");
            pauseMenuUI.SetActive(false);
        }
    }
}
