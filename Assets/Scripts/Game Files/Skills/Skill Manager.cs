using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class SkillManager : MonoBehaviour
{
    [Header("References")]
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public AudioManager audioManager;
    public CharSkills charSkills;
    public Collider2D characterCollider;
    public Board_Manager boardManager;
    public TriggerSkills triggerSkills;

    [Header("Tilemap References")]
    public Transform opponentMainTileMap;
    public Transform opponentGhostTileMap;

    [Header("Character Skill Animation")]
    public Animator characterSkillAnim;

    [Header("Cooldown UI")]
    public GameDisplay gameDisplay;
    private Coroutine pulseRoutine;

    // for tetro skill
    [Header("Blinding Overlay")]
    public GameObject blindOverlay;
    public GameObject blindBall;
    public Animator animBall;
    public SpriteRenderer BO_Renderer;
    public SpriteRenderer ballRenderer;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Character Identifiers")]
    public int characterID;
    public string characterName;
    public string characterDescription;
    public string characterRole;
    public GameObject characterPrefab;
    public Animator skillanimator;

    [Header("Misc")]
    public bool isPlayer1 = true;
    public bool isChar1 = true;
    public bool is2PhaseSkill = false;
    public int skillPhase = 0;
    public float skillDuration = 10f;
    public Color skillColor;
    // for scorch skill
    public int burnCtr = 0;
    public int maxBurnStacks = 0;
    // for yun jin skill
    [Header("Rocks Summons Settings")]
    public int rockCtr = 0;
    public int maxRockCount = 0;
    public float rockPushForce = 0f;

    [Header("Cooldown Settings")]
    private bool isOnCooldown = false;
    private float cooldownTimer = 0f;
    private float cooldownTime = 0f;

    private void Awake()
    {
        string playerTag = isPlayer1 ? "P1" : "P2";
        // assign the input action for the skill button
        if (isChar1)
        {
            skillAction = playerInput.actions["Primary Skill"];
            Debug.Log($"SkillManager: {playerTag}'s {characterName} skill assigned as Primary Skill button.");
        }
        else
        {
            skillAction = playerInput.actions["Secondary Skill"];
            Debug.Log($"SkillManager: {playerTag}'s {characterName} skill assigned as Secondary Skill button.");
        }
        // get all the references from the chaar skill scriptable object
        cooldownTime = charSkills.cooldownTime;
        skillColor = charSkills.skillColor;
        characterID = charSkills.characterID;
        characterName = charSkills.characterName;
        characterDescription = charSkills.characterDescription;
        characterRole = charSkills.characterRole;
        characterPrefab = charSkills.characterPrefab;
        skillanimator = charSkills.skillanimator;

        opponentMainTileMap = gameManager.pvp.opponentPlayerManager.gameDisplay.mainTileMap.transform;
        opponentGhostTileMap = gameManager.pvp.opponentPlayerManager.gameDisplay.ghostTileMap.transform;

        maxBurnStacks = charSkills.maxBurnStack;

        maxRockCount = charSkills.maxRockCount;
        rockPushForce = charSkills.rockPushForce;
    }

    void Update()
    {
        if (skillAction.triggered)
        {
            ActivateSkill();
        }
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);
            if(isChar1)
            {
                gameDisplay.Skill2CooldownUpdate(cooldownTimer);
            }
            else
            {
                gameDisplay.Skill1CooldownUpdate(cooldownTimer);
            }

            if (cooldownTimer <= 0f)
            {
                if (skillPhase == 0)
                {
                    isOnCooldown = false;
                    Debug.Log("Skill cooldown ended. Skill is ready to use.");
                }
            }
        }
    }
    private void ActivateSkill() // This method can be called when the player presses the skill button
    {
        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown. Please wait.");
        }
        else if (gameManager.player.score < charSkills.skillCost)
        {
            Debug.Log("Not enough chips to use this skill.");
        }
        else
        {
            Debug.Log("Skill activated!");
            
            switch (charSkills.characterID)
            {
                case 0:
                    // Activate skill for tetro
                    animBall.SetTrigger("Activate");
                    break;
                case 1:
                    triggerSkills.ScorchSkill();
                    break;
                case 2:
                    triggerSkills.PackHatSkill();
                    break;
                case 3:
                    triggerSkills.DodokeSkill();
                    break;
                case 4:
                    triggerSkills.YunJinSkill();
                    break;
                default:
                    Debug.LogWarning("Unknown character ID. No skill activated.");
                    break;
            }
            if (skillPhase != 1)
            {
                isOnCooldown = true;
                cooldownTimer = cooldownTime;
            }
            PlaySkillSound();
        }
    }

    private void PlaySkillSound()
    {

    }
}
