using TMPro;
using UnityEngine;
using UnityEngine.UI;
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

    public int cost = 300;
    
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

        MainTileMap = gameDisplay.mainTileMap;
        GhostTileMap = gameDisplay.ghostTileMap;

        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        skillAction = playerInput.actions.FindAction("Skill");
        skillAction.performed += ctx => ActivateSkill();

        gameDisplay.costText.text = cost.ToString();
    }

    private void Update()
    {
        cost += 100 * gameManager.inflationCtr;
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

        StartCoroutine(BoardFlip());
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

}
