using UnityEngine;

public class GameMaster : MonoBehaviour
{
    [Header("Player Managers")]
    [SerializeField] private PlayerManager player1Manager;
    [SerializeField] private PlayerManager player2Manager;

    [Header("Game State")]
    [SerializeField] private bool isGameOver = false;

    [Header("Game Settings")]
    [SerializeField] private int scorePerLine = 100;
    [SerializeField] private int initialLevel = 1;
    [SerializeField] private float levelSpeedMultiplier = 0.8f;

    [Header("Game Debugging")]
    public bool disableSpawnForP1 = false;
    public bool disableSpawnForP2 = false;

}
