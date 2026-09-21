using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class DodokeSkill : MonoBehaviour
{
    [Header("Board Target")]
    public Transform MainTileMap;
    public Transform GhostTileMap;
    public float SkillDuration = 10f;
    public float Temp_Z_Position;

    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 25f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Misc")]
    public bool isSec = false;
    public int cost = 300;
    
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

        MainTileMap = gameDisplay.mainTileMap;
        GhostTileMap = gameDisplay.ghostTileMap;

        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        if (isSec == true)
        {
            Debug.Log("Dodoke Skill Assigned as Secondary Skill");
            skillAction = playerInput.actions.FindAction("Secondary Skill");
            gameDisplay.cost2Text.text = cost.ToString();
        }
        else if(isSec == false)
        {
            Debug.Log("Dodoke Skill Assigned as Primary Skill");
            skillAction = playerInput.actions.FindAction("Skill");
            gameDisplay.cost1Text.text = cost.ToString();
        }        
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
            Debug.Log($"Remaining Time for skill: {cooldownTimer}");
        }
    }
    
    public void ActivateSkill()
    {
        if (gameManager.isGameOver) return;
        cost += 100 * gameManager.inflationCtr;
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

        StartCoroutine(BoardFlip());
        gameManager.pvp.opponentGameManager.shaker.boardShake();
        StartCoroutine(gameManager.pvp.opponentGameManager.gameDisplay.BackPulse(10f, "#720076ff"));
    }

    public IEnumerator BoardFlip()
    {
        Debug.Log("Board flip started!");

        // Store original position and rotation
        Vector3 originalPosition = MainTileMap.position;
        Quaternion originalRotation = MainTileMap.rotation;
        Quaternion ghostOriginalRotation = GhostTileMap.rotation;

        // Flip Z position and rotation
        MainTileMap.position = new Vector3(originalPosition.x, originalPosition.y, -5f);
        MainTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);

        if (GhostTileMap != null)
        {
            GhostTileMap.position = MainTileMap.position;
            GhostTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);
        }

        yield return new WaitForSeconds(SkillDuration);

        // Restore position and rotation
        MainTileMap.position = originalPosition;
        MainTileMap.rotation = originalRotation;

        if (GhostTileMap != null)
        {
            GhostTileMap.position = originalPosition;
            GhostTileMap.rotation = ghostOriginalRotation;
        }

        Debug.Log("Board flip ended!");
    }

    private void OnDisable()
    {
        skillAction.performed -= ctx => ActivateSkill();
    }
}
