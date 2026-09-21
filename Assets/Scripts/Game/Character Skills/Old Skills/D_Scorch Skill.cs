using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using NUnit.Framework;

public class D_ScorchSkill : MonoBehaviour
{
    [Header("References")]
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public Game_Manager opponent;
    public GameDisplay gameDisplay;
    public ScorchCursor scorchCursor;
    public AudioManager audioManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 40f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Misc")]
    public bool isSec = false;
    public int cost = 400;
    public int maxStacks = 3;
    public int burnCtr = 0;

    private void Start()
    {
        characterManager = GetComponent<CharacterManager>();
        if (characterManager.isPlayer1)
        {
            gameManager = GameObject.Find("Game Manager P1").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 1").GetComponent<PlayerInput>();
            opponent = gameManager.pvp.opponentGameManager;
        }
        else
        {
            gameManager = GameObject.Find("Game Manager P2").GetComponent<Game_Manager>();
            gameDisplay = gameManager.gameDisplay;
            playerInput = GameObject.Find("Player 2").GetComponent<PlayerInput>();
            opponent = gameManager.pvp.opponentGameManager;
        }

        scorchCursor = gameManager.scorchCursor;
        audioManager = gameManager.audioManager;
        scorchCursor.enabled = true;
        isOnCooldown = true;
        cooldownTimer = cooldownTime;
        gameManager.player.maxAmmo = 5;
        
        gameDisplay.UpdateBurnStack(burnCtr);

        // Setup input
        if (isSec == true)
        {
            Debug.Log("Scorch Skill Assigned as Secondary Skill");
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            Debug.Log("Scorch Skill Assigned as Primary Skill");
            skillAction = playerInput.actions.FindAction("Skill");
            gameDisplay.cost1Text.text = cost.ToString();
        }        
        if (skillAction != null)
           skillAction.performed += ctx => ActivateSkill();
    }

    private void Update()
    {
        // cooldown and regen logic
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

                if (burnCtr < maxStacks)
                {
                    burnCtr++;
                    Debug.Log("Burn Stack received");
                    gameDisplay.UpdateBurnStack(burnCtr);
                    cooldownTimer = cooldownTime;
                    isOnCooldown = true;
                }
                else
                {
                    isOnCooldown = false;
                    Debug.Log("Skill cooldown ended and burn stacks full.");
                }
            }
        }
    }

    public void ActivateSkill()
    {
        if (burnCtr < 1 || gameManager.player.score < cost)
        {
            Debug.Log("Not enough burn stacks or chips!");
            return;
        }
        if (opponent == null || opponent.isGameOver) return;

        // Destroy tile on opponent's main board at cursor position
        gameManager.player.score -= cost;
        gameDisplay.UpdateChips(gameManager.player.score);
        gameManager.shaker.ChipsDeductShake();
        gameManager.shaker.CostShake();
        Vector2Int pos = gameManager.scorchCursor.position;
        Vector3Int tilePos = new Vector3Int(pos.x, pos.y, 0);
        gameManager.pvp.opponentGameManager.boardManager.main_tilemap.SetTile(tilePos, null);
        opponent.shaker.boardShake();
        StartCoroutine(opponent.gameDisplay.BackPulse(5f, "#760e00ff"));
        audioManager.sfxSource.PlayOneShot(audioManager.ScorchSfx);
        // Deduct stack & update
        burnCtr = Mathf.Max(0, burnCtr-1);
        gameDisplay.UpdateBurnStack(burnCtr);

        // Start cooldown if not already
        if(!isOnCooldown) {
            isOnCooldown = true;
            cooldownTimer = cooldownTime;
        }

        Debug.Log($"D_Scorch Skill: Burn used. Destroyed tile at {tilePos}.");
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }
}
