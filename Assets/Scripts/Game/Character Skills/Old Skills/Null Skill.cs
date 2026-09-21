using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class NullSkill : MonoBehaviour
{
    [Header("References")]
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public CharacterManager characterManager;
    public Board_Manager boardManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 30f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Misc")]
    public bool isSec = false;
    public int cost = 500;

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
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        gameManager.player.maxAmmo = 5;

        if (isSec == true)
        {
            Debug.Log("Null Skill Assigned as Secondary Skill");
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            Debug.Log("Null Skill Assigned as Primary Skill");
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
        gameManager.shaker.ChipsDeductShake();
        gameManager.shaker.CostShake();

        Buff();
    }

    public void Buff()
    {
        // Freeze gravity for a short duration and add half of current ammo
        StartCoroutine(FreezeGravityAndAddAmmoCoroutine(12f));
        gameManager.shaker.boardShake();
        StartCoroutine(gameDisplay.BackPulse(12f, "#720076ff"));
    }

    private IEnumerator FreezeGravityAndAddAmmoCoroutine(float durationSeconds)
    {
        gameManager.player.attackAmmo += 2;
        if (gameManager.player.attackAmmo > gameManager.player.maxAmmo)
        {
            gameManager.player.attackAmmo = gameManager.player.maxAmmo;
        }
        gameDisplay.Ammo_Update(gameManager.player.attackAmmo);

        // Freeze gravity by setting a very large delay and restore it after duration
        float originalDelay = gameManager.currentGravityDelay;
        gameManager.currentGravityDelay = float.MaxValue;
        Debug.Log($"Gravity frozen for {durationSeconds} seconds. Ammo + 2.");

        yield return new WaitForSeconds(durationSeconds);

        gameManager.currentGravityDelay = originalDelay;
        Debug.Log("Gravity restored.");
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }
}
