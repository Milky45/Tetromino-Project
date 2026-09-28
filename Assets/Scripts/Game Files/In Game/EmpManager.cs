using UnityEngine;

public class EmpManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerStatus playerStatus;
    [SerializeField] private PlayerManager playerManager;

    [Header("EMP Settings")]
    public float empCooldownTimer;

    [Header("Animator")]
    public Animator EmpAnim;

    public void StartEmpCooldown()
    {
        playerStatus.empOnCooldown = true;
        empCooldownTimer = playerStatus.empCooldownDuration;
        Debug.Log($"EMP cooldown started for {playerStatus.empCooldownDuration} seconds!");
        if (playerManager != null && playerManager.gameDisplay != null)
        {
            playerManager.gameDisplay.UpdateEMPStateIcon();
        }
    }

    private void Update()
    {
        if (playerStatus != null && playerStatus.empOnCooldown)
        {
            empCooldownTimer -= Time.deltaTime;
            if (empCooldownTimer <= 0f)
            {
                empCooldownTimer = 0f;
                playerStatus.empOnCooldown = false;
                if (playerManager != null && playerManager.gameDisplay != null)
                {
                    playerManager.gameDisplay.UpdateEMPStateIcon();
                }
                Debug.Log("EMP Cooldown finished!");
            }
        }
    }
}