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




}
