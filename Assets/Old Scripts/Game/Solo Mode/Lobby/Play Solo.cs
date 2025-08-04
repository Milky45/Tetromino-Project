using UnityEngine;
using UnityEngine.SceneManagement;

public class PlaySolo : MonoBehaviour
{
    public void PlayPressed()
    {
        // Load the game scene
        SceneManager.LoadScene("TetrominoGame");
    }
}
