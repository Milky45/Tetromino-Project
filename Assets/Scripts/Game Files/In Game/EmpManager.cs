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
        playerManager.gameDisplay.UpdateEMPStateIcon();
    }
}