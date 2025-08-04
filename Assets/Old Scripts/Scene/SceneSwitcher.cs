using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadSoloMode()
    {
        SceneManager.LoadScene("Tetromino");
    }
}
