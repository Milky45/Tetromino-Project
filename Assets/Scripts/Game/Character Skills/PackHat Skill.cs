using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PackHatSkill : MonoBehaviour
{
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public Animator packhatAnim;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;


    [Header("Cooldown Settings")]
    public float cooldownTime = 35f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    public int cost = 500;
    
    // Tracks the temporary zero-attack-cooldown effect
    private Coroutine zeroAtkCooldownRoutine;
    private void Awake()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager.isPlayer1)
        {
            gameManager = GameObject.Find("Game Manager P1").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
        }
        else
        {
            gameManager = GameObject.Find("Game Manager P2").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 2").GetComponent<PlayerInput>();
        }
        packhatAnim = GetComponent<Animator>();
        cooldownTimer = cooldownTime + 10;
        isOnCooldown = true;

        skillAction = playerInput.actions.FindAction("Skill");
        skillAction.performed += ctx => ActivateSkill();

        gameManager.player.maxAmmo = 5;
        gameDisplay.costText.text = cost.ToString();
    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            gameDisplay.SkillCooldownUpdate(cooldownTimer);

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
            }
            Debug.Log($"Remaining Time for skill: {cooldownTimer}");
        }
    }

    public void ActivateSkill()
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            //AudioManager.Instance.sfxSource.PlayOneShot(AudioManager.Instance.invalid);
            return;

        }
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }
        if (gameManager.isTimeStopped) return;
        
        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        packhatAnim.SetTrigger("Activate");

        // Turn attack cooldown to 0 for 10 seconds when skill is activated
        EnableZeroAttackCooldownForTenSeconds();
    }

    // Public method to set attack cooldown to 0 for 10 seconds
    public void EnableZeroAttackCooldownForTenSeconds()
    {
        if (gameManager == null || gameManager.player == null)
        {
            Debug.LogWarning("Cannot apply zero attack cooldown: missing Game_Manager or Player reference.");
            return;
        }

        if (zeroAtkCooldownRoutine != null)
        {
            StopCoroutine(zeroAtkCooldownRoutine);
        }
        zeroAtkCooldownRoutine = StartCoroutine(ZeroAttackCooldownCoroutine(10f));
    }

    private IEnumerator ZeroAttackCooldownCoroutine(float durationSeconds)
    {
        Player playerRef = gameManager.player;
        float originalCooldown = playerRef.atkCD_Time;

        // Clear any currently active attack cooldown and set to zero
        playerRef.atkOnCooldown = false;
        playerRef.atkTempCD = originalCooldown;
        playerRef.atkCD_Time = 0f;
        Debug.Log($"Attack cooldown set to 0 for {durationSeconds} seconds.");

        yield return new WaitForSeconds(durationSeconds);

        // Restore original attack cooldown
        playerRef.atkCD_Time = playerRef.atkTempCD > 0f ? playerRef.atkTempCD : originalCooldown;
        Debug.Log("Attack cooldown restored.");
        packhatAnim.SetTrigger("Return");

        zeroAtkCooldownRoutine = null;
    }

}