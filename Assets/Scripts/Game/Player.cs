using UnityEngine;

public class Player : MonoBehaviour
{
    public Game_Manager gameManager;

    public bool isPlayer1 = true;
    public int lives = 3;
    public int gold = 0;
    public int score = 0;

    public bool holdUsed = false;
    public int comboCount = 0;
    public int attackAmmo = 0;
    public int maxAmmo = 10;
    public bool isInverted = false;

    public bool hasEmpGrenade = false;
    public bool empOnCooldown = false;
    public float empCooldownDuration = 60f; // Duration for EMP cooldown in seconds
    public bool atkOnCooldown = false;
    public float atkCD_Time = 0.5f;
    public float atkTempCD;
    public int lastComboMilestone = 0;
    public int pendingDeadLines = 0;

    public void Awake()
    {
        gameManager = FindFirstObjectByType<Game_Manager>();
        AutoAssignPlayerType();
    }

    /// <summary>
    /// Automatically assigns the player as P1 if no players exist, or P2 if P1 already exists
    /// </summary>
    private void AutoAssignPlayerType()
    {
        // Find all existing Player objects in the scene
        Player[] existingPlayers = FindObjectsByType<Player>(FindObjectsSortMode.None);
        
        // Count how many players are already assigned as P1
        int p1Count = 0;
        foreach (Player player in existingPlayers)
        {
            if (player != this && player.isPlayer1) // Don't count this player, only others
            {
                p1Count++;
            }
        }
        
        // If no other P1 exists, assign this player as P1
        if (p1Count == 0)
        {
            isPlayer1 = true;
            Debug.Log($"Player {gameObject.name} automatically assigned as P1");
        }
        else
        {
            // If P1 already exists, assign this player as P2
            isPlayer1 = false;
            Debug.Log($"Player {gameObject.name} automatically assigned as P2 (P1 already exists)");
        }
    }

    /// <summary>
    /// Manually set the player type (can be called from other scripts if needed)
    /// </summary>
    /// <param name="isP1">True to set as P1, false to set as P2</param>
    public void SetPlayerType(bool isP1)
    {
        isPlayer1 = isP1;
        Debug.Log($"Player {gameObject.name} manually set as {(isP1 ? "P1" : "P2")}");
    }
}
