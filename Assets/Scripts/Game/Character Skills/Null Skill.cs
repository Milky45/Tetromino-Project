using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class NullSkill : MonoBehaviour
{
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public Board_Manager boardManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    public int cost = 500;

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
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        gameManager.player.maxAmmo = 12;

        skillAction = playerInput.actions.FindAction("Skill");
        skillAction.performed += ctx => ActivateSkill();

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

        Buff();
    }

    public void Buff()
    {
        // Freeze gravity for a short duration and add half of current ammo
        StartCoroutine(FreezeGravityAndAddAmmoCoroutine(12f));
    }

    private IEnumerator FreezeGravityAndAddAmmoCoroutine(float durationSeconds)
    {
        // Add half of current ammo (floor), clamped to max
        int currentAmmo = gameManager.player.attackAmmo;
        int ammoToAdd = currentAmmo / 2;
        gameManager.player.attackAmmo = Mathf.Min(gameManager.player.maxAmmo, currentAmmo + ammoToAdd);
        gameDisplay.Ammo_Update(gameManager.player.attackAmmo);

        // Freeze gravity by setting a very large delay and restore it after duration
        float originalDelay = gameManager.currentGravityDelay;
        gameManager.currentGravityDelay = float.MaxValue;
        Debug.Log($"Gravity frozen for {durationSeconds} seconds. Ammo +{ammoToAdd}.");

        yield return new WaitForSeconds(durationSeconds);

        gameManager.currentGravityDelay = originalDelay;
        Debug.Log("Gravity restored.");
    }

}
