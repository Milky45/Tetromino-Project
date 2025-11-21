using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

public class YunJinSkill : MonoBehaviour
{
    [Header("References")]
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public CharacterManager characterManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    private float cooldownTimer = 0f;
    public float cooldownTime = 45f;
    public bool isOnCooldown = true;

    [Header("Rocks Settings")]
    public bool Rock1Active = false;
    public bool Rock2Active = false;
    public bool Rock3Active = false;

    [Header("Fragile Reference")]
    public Fragile fragile;

    [Header("Yun Jin Skill Settings")]
    public Animator YunJinAnim;
    private bool ActiveRock = false;
    public bool Fragile = false;

    [Header("Misc")]
    public int rockCount; // Number of rocks to animate
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

        fragile = GetComponent<Fragile>();
        
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        gameManager.player.maxAmmo = 5;

        if (isSec == true)
        {
            YunJinAnim = characterManager.charSecDisplay.GetComponent<Animator>();
            Debug.Log("Yun Jin Skill Assigned as Secondary Skill");
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            YunJinAnim = GetComponent<Animator>();
            Debug.Log("Yun Jin Skill Assigned as Primary Skill");
            skillAction = playerInput.actions.FindAction("Skill");
            gameDisplay.cost1Text.text = cost.ToString();
        }        
        skillAction.performed += ctx => ActivateSkill();
    }

    void Update()
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
                ReturnRockColor();
                isOnCooldown = false;
            }
        }

        // If all rocks are gone after a push or block, end the active rock state immediately
        if (ActiveRock && !Rock1Active && !Rock2Active && !Rock3Active)
        {
            ActiveRock = false;
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
                // Only reset Fragile if this is the last rock (when all rocks are gone)
                if (!Rock2Active && !Rock3Active)
                {
                    Fragile = false;
                }
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

    public void StoneDestroyed()
    {
        isOnCooldown = true;
        ReturnRockIdle();
    }

    public void ActivateSkill()
    {
        if (isOnCooldown) return;
        if (gameManager.isGameOver) return;
        // Only trigger the attack (rocks spawn) when not currently in Fragile state

        if (gameManager.isTimeStopped) return;
        if (gameManager.player.score < cost)
        {
            Debug.Log("Not enough chips to activate this skill");
            return;
        }

        YunJinAnim.SetTrigger("Attack");
    }

    public void ActivateFragile()
    {
        if (gameManager.pvp.opponentGameManager.isGameOver) return;
        if (Fragile == false)
        {
            return;
        }
        fragile.RocksPush();
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        Fragile = false;
    }
    
    public void ExecuteSkill()
    {
        if (Fragile == true)
        {
            ActivateFragile();
            return;
        }
        // Deduct cost on release when we actually commit to the skill
        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);
        gameManager.shaker.ChipsDeductShake();
        gameManager.shaker.CostShake();

        fragile.Rock1Anim.SetTrigger("Rock 1");
        fragile.Rock2Anim.SetTrigger("Rock 2");
        fragile.Rock3Anim.SetTrigger("Rock 3");
        Rock1Active = true;
        Rock2Active = true;
        Rock3Active = true; // Activate Rock3 for heavy strength
        rockCount = 3; // Set rock count for heavy strength
        ActiveRock = true;
        Fragile = true; // Set Fragile state to true when skill is executed

        Debug.Log($"Yun Jin Skill used!");
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
        ReturnRockIdle();
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }
}


