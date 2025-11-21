using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PackHatSkill : MonoBehaviour
{
    [Header("References")]
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public CharacterManager characterManager;
    public Animator packhatAnim;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 35f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Misc")]
    public bool isSec = false;
    public bool isSkillActive = false;
    public int cost = 500;
    
    // Tracks the temporary zero-attack-cooldown effect
    private Coroutine zeroAtkCooldownRoutine;
    private void Start()
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
        
        gameManager.player.maxAmmo = 5;
        cooldownTimer = cooldownTime + 10;
        isOnCooldown = true;

        if (isSec == true)
        {
            Debug.Log("PackHat Skill Assigned as Secondary Skill");
            packhatAnim = characterManager.charSecDisplay.GetComponent<Animator>();
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            Debug.Log("PackHat Skill Assigned as Primary Skill");
            packhatAnim = GetComponent<Animator>();
            skillAction = playerInput.actions.FindAction("Skill");
            gameDisplay.cost1Text.text = cost.ToString();
        }
        if (skillAction != null)
           skillAction.performed += ctx => ActivateSkill();

    }

    private void Update()
    {
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            if(isSec)
            {
                gameDisplay.Skill2CooldownUpdate(cooldownTimer);
            }
            else
            {
                gameDisplay.Skill1CooldownUpdate(cooldownTimer);
            }

            if (cooldownTimer <= 0f)
            {
                isOnCooldown = false;
                Debug.Log("Skill cooldown ended. Skill is ready to use.");
            }
        }
    }

    public void ActivateSkill()
    {
        if (gameManager.isGameOver) return;
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
        gameManager.shaker.ChipsDeductShake();
        gameManager.shaker.CostShake();

        // Turn attack cooldown to 0 for 10 seconds when skill is activated
        EnableZeroAttackCooldownForTenSeconds();
        StartCoroutine(gameDisplay.BackPulse(10f, "#bb6400ff"));
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
        isSkillActive = true;
        playerRef.atkOnCooldown = false;
        playerRef.atkTempCD = originalCooldown;
        playerRef.atkCD_Time = 0f;
        Debug.Log($"Attack cooldown set to 0 for {durationSeconds} seconds.");

        yield return new WaitForSeconds(durationSeconds);

        // Restore original attack cooldown
        isSkillActive = false;
        playerRef.atkCD_Time = playerRef.atkTempCD > 0f ? playerRef.atkTempCD : originalCooldown;
        Debug.Log("Attack cooldown restored.");
        packhatAnim.SetTrigger("Return");

        zeroAtkCooldownRoutine = null;
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }
}