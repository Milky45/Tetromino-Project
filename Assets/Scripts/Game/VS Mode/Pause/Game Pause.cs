using UnityEngine;

public class GamePause : MonoBehaviour
{
    public static GamePause Instance { get; private set; }
    public GameManagerP2 gameP2;
    public GameManager gameP1;
    public GameObject pauseBtn;
    public bool IsPaused { get; private set; } = false;

    [SerializeField] private GameObject pauseMenuUI; // Assign your Pause Menu UI Canvas in the inspector

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void TogglePause()
    {
        IsPaused = !IsPaused;
        gameP1.isPaused = IsPaused;
        gameP2.isPaused = IsPaused;
        if (IsPaused == true)
        {
            pauseBtn.SetActive(false);
            Time.timeScale = 0f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
            AudioManager.Instance.PauseMusic();
            Debug.Log("Game Paused");
            pauseMenuUI.SetActive(true);
        }
        else
        {
            pauseBtn.SetActive(true);
            Time.timeScale = 1f;
            if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
            AudioManager.Instance.UnpauseMusic();
            Debug.Log("Game Resumed");
            pauseMenuUI.SetActive(false);
        }
    }

}
