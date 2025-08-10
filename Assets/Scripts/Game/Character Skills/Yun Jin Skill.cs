using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class YunJinSkill : MonoBehaviour
{
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public CharacterManager characterManager;
    public PlayerInput playerInput;

    private InputAction skillAction;
    private System.Action<UnityEngine.InputSystem.InputAction.CallbackContext> onSkillStarted;

    [Header("Cooldown Settings")]
    public float cooldownTime = 20f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Rocks Settings")]
    public bool Rock1Active = false;
    public bool Rock2Active = false;
    public bool Rock3Active = false;

    [Header("Fragile Reference")]
    public Fragile fragile;

    [Header("Yun Jin Skill Settings")]
    public Animator YunJinAnim;
    public float charge;
    public float RockDur = 15f;
    private bool ActiveRock = false;
    public bool Fragile = false;
    public float chargeTime = 0f;
    public float maxCharge = 3f; // Max charge duration (3 seconds)
    public int rockCount; // Number of rocks to animate
    private bool isCharging = false;

    public int cost = 500;

    public enum SkillStrength { Light, Medium, Heavy }
    public SkillStrength strength;

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
        cooldownTimer = cooldownTime + 20;
        fragile = GetComponent<Fragile>();
        YunJinAnim = GetComponent<Animator>();

        gameManager.player.maxAmmo = 7;

        skillAction = playerInput.actions.FindAction("Skill");
        // Start charge on press; if rocks are active, perform rock push instead (even during cooldown)
        if (skillAction != null)
        {
            onSkillStarted = ctx => HandleSkillActionPress();
            skillAction.started += onSkillStarted;
        }
        if (gameDisplay.PowerBar != null) gameDisplay.PowerBar.sprite = gameDisplay.PowerBar_0;

        gameDisplay.costText.text = cost.ToString();
    }

    void Update()
    {
        if (ActiveRock == true)
        {
            RockDur -= Time.deltaTime;
            gameDisplay.RockDurUI.text = Mathf.CeilToInt(RockDur).ToString();
            if (RockDur <= 0f)
            {
                ActiveRock = false;
                Rock1Active = false;
                Rock2Active = false;
                Rock3Active = false;
                Fragile = false;
                ReturnRockIdle();
                gameDisplay.RockDurUI.text = ""; // Reset UI text
                 // Reset Fragile state
            }
        }
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            if (gameDisplay != null)
            {
                gameDisplay.SkillCooldownUpdate(cooldownTimer);
            }

            if (cooldownTimer <= 0f)
            {
                ReturnRockColor();
                isOnCooldown = false;
            }
        }

        // If all rocks are gone after a push or block, end the active rock state immediately
        if (ActiveRock && !Rock1Active && !Rock2Active && !Rock3Active)
        {
            ActiveRock = false;
            gameDisplay.RockDurUI.text = "";
            Fragile = false;
            ReturnRockIdle();
        }
    }

    public void InvisRock(int RockCtr)
    {
        switch (RockCtr)
        {
            case 1:
                gameDisplay.Rock1.color = new Color(gameDisplay.Rock1.color.r, gameDisplay.Rock1.color.g, gameDisplay.Rock1.color.b, 0f);
                Rock1Active = false;
                Fragile = false; // Reset Fragile state. Assuming that rock 1 is the final rock that blocks the attack
                Debug.Log("Rock1 blocked the attack");
                break;
            case 2:
                gameDisplay.Rock2.color = new Color(gameDisplay.Rock2.color.r, gameDisplay.Rock2.color.g, gameDisplay.Rock2.color.b, 0f);
                Rock2Active = false;
                Debug.Log("Rock2 blocked the attack");
                break;
            case 3:
                gameDisplay.Rock3.color = new Color(gameDisplay.Rock3.color.r, gameDisplay.Rock3.color.g, gameDisplay.Rock3.color.b, 0f);
                Rock3Active = false;
                Debug.Log("Rock3 blocked the attack");
                break;
        }
    }

    public void ActivateSkill(float tempCharge)
    {
        // Only trigger the attack (rocks spawn) when not currently in Fragile state
        if (Fragile) return;

        if (gameManager.isTimeStopped) return;
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }

        // Deduct cost on release when we actually commit to the skill
        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);

        charge = tempCharge;
        YunJinAnim.SetTrigger("Attack");
    }

    public void ActivateFragile()
    {
        if (Fragile == false)
        {
            return;
        }
        fragile.RocksPush();
        Debug.Log("Rocks pushed forward");
        if (strength == SkillStrength.Heavy)
        {
            cooldownTimer += 10f;
        }
        else if (strength == SkillStrength.Medium)
        {
            cooldownTimer += 7f;
        }
        else if (strength == SkillStrength.Light)
        {
            cooldownTimer += 5f;
        }
        Fragile = false;
    }

    public void ExecuteSkill()
    {
        // Perform the ground smash outcome: spawn and activate rocks based on charge
        if (charge < 0.5f) strength = SkillStrength.Light;
        else if (charge < 1f) strength = SkillStrength.Medium;
        else if (charge >= 1f) strength = SkillStrength.Heavy;

        // Reset remaining cooldown addition accumulator
        cooldownTimer = 0f;

        switch (strength)
        {
            case SkillStrength.Light:
                cooldownTimer += 7f; // extra cooldown for light
                RockDur = 6f;
                fragile.Rock1Anim.SetTrigger("Rock 1");
                Rock1Active = true; // Activate Rock1 for light strength
                rockCount = 1; // Set rock count for light strength
                break;
            case SkillStrength.Medium:
                cooldownTimer += 10f; // extra cooldown for medium
                RockDur = 10f;
                fragile.Rock1Anim.SetTrigger("Rock 1");
                fragile.Rock2Anim.SetTrigger("Rock 2");
                Rock1Active = true; // Activate Rock1 for medium strength
                Rock2Active = true; // Activate Rock2 for medium strength
                rockCount = 2; // Set rock count for medium strength
                break;
            case SkillStrength.Heavy:
                cooldownTimer += 15f; // extra cooldown for heavy
                RockDur = 15f;
                fragile.Rock1Anim.SetTrigger("Rock 1");
                fragile.Rock2Anim.SetTrigger("Rock 2");
                fragile.Rock3Anim.SetTrigger("Rock 3");
                Rock1Active = true;
                Rock2Active = true;
                Rock3Active = true; // Activate Rock3 for heavy strength
                rockCount = 3; // Set rock count for heavy strength
                break;
        }
        cooldownTimer += cooldownTime; // base cooldown + strength-based extra
        isOnCooldown = true;
        ActiveRock = true;
        Fragile = true; // Set Fragile state to true when skill is executed

        Debug.Log($"Skill used: {strength}, Cooldown: {cooldownTime}s");
    }

    public void ReturnRockColor()
    {
        gameDisplay.Rock1.color = new Color(gameDisplay.Rock1.color.r, gameDisplay.Rock1.color.g, gameDisplay.Rock1.color.b, 1f);
        gameDisplay.Rock2.color = new Color(gameDisplay.Rock2.color.r, gameDisplay.Rock2.color.g, gameDisplay.Rock2.color.b, 1f);
        gameDisplay.Rock3.color = new Color(gameDisplay.Rock3.color.r, gameDisplay.Rock3.color.g, gameDisplay.Rock3.color.b, 1f);
    }

    public void ReturnRockIdle()
    {
        Debug.Log("Returning rocks to idle state");
        fragile.Rock1Anim.SetTrigger("Return");
        fragile.Rock2Anim.SetTrigger("Return");
        fragile.Rock3Anim.SetTrigger("Return");
        rockCount = 0; // Reset rock count after returning to idle
    }

    public void DestroyAllRocks()
    {
        gameDisplay.Rock1.color = new Color(gameDisplay.Rock1.color.r, gameDisplay.Rock1.color.g, gameDisplay.Rock1.color.b, 0f);
        gameDisplay.Rock2.color = new Color(gameDisplay.Rock2.color.r, gameDisplay.Rock2.color.g, gameDisplay.Rock2.color.b, 0f);
        gameDisplay.Rock3.color = new Color(gameDisplay.Rock3.color.r, gameDisplay.Rock3.color.g, gameDisplay.Rock3.color.b, 0f);
        Rock1Active = false;
        Rock2Active = false;
        Rock3Active = false;
        ActiveRock = false;
        Fragile = false;
        RockDur = 0f;
        gameDisplay.RockDurUI.text = "";
        ReturnRockIdle();
    }

    public void StartSkillChargeDetection(InputAction skillAction, System.Action<float> onReleased)
    {
        if (isCharging) return;
        StartCoroutine(DetectChargeCoroutine(skillAction, onReleased));
    }

    private IEnumerator DetectChargeCoroutine(InputAction skillAction, System.Action<float> onReleased)
    {
        isCharging = true;
        chargeTime = 0f;

        while (skillAction.ReadValue<float>() > 0f)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxCharge);
            if (chargeTime < 0.5f)
            {
                gameDisplay.PowerBar.sprite = gameDisplay.PowerBar_1;
            }
            else if (chargeTime < 1f)
            {
                gameDisplay.PowerBar.sprite = gameDisplay.PowerBar_2;
            }
            else if(chargeTime >= 1f)
            {
                gameDisplay.PowerBar.sprite = gameDisplay.PowerBar_3;
            }
            yield return null;
        }

        isCharging = false;
        gameDisplay.PowerBar.sprite = gameDisplay.PowerBar_0;
        onReleased?.Invoke(chargeTime);
        Debug.Log($"ChargeTime: {chargeTime}s");
    }

    private void HandleSkillActionPress()
    {
        // If rocks are active, pressing the skill triggers the push regardless of cooldown
        if (Fragile)
        {
            ActivateFragile();
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            return;
        }
        if (gameManager.isTimeStopped) return;
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }

        StartSkillChargeDetection(skillAction, ActivateSkill);
    }

    private void OnDisable()
    {
        if (skillAction != null)
        {
            if (onSkillStarted != null)
            {
                skillAction.started -= onSkillStarted;
            }
        }
    }
}


