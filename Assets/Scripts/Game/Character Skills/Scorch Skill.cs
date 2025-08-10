using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScorchSkill : MonoBehaviour
{
    public CharacterManager characterManager;
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public Board_Manager boardManager;

    [Header("Input")]
    public PlayerInput playerInput;
    private InputAction skillAction;

    [Header("Cooldown Settings")]
    public float cooldownTime = 40f;
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
        boardManager = gameManager.boardManager;
        isOnCooldown = true;
        cooldownTimer = cooldownTime;

        gameManager.player.maxAmmo = 15;

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

        ClearBottomLines();
    }

    public void ClearBottomLines()
    {
        var activePiece = GameObject.Find($"ActivePiece{(gameManager.player.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();
        if (activePiece != null)
            activePiece.Clear();
        int linesToRemove = 5;
        int linesCleared = 0;
        int y = -boardManager.boardSize.y / 2;

        while (linesCleared < linesToRemove && y < boardManager.boardSize.y / 2)
        {
            bool hasAnyTile = false;
            bool isDeadLine = true;

            for (int x = -boardManager.boardSize.x / 2; x < boardManager.boardSize.x / 2; x++)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                if (boardManager.main_tilemap.HasTile(pos))
                {
                    hasAnyTile = true;

                    if (boardManager.main_tilemap.GetTile(pos) != boardManager.tile_types[7])
                    {
                        isDeadLine = false;
                    }
                }
            }

            if (hasAnyTile)
            {
                boardManager.ClearLineFromWorldY(y);
                linesCleared++;

                if (isDeadLine)
                {
                    boardManager.receivedDeadLineCount--;
                    boardManager.receivedDeadLineCount = Mathf.Max(boardManager.receivedDeadLineCount, 0);
                }
            }
            else
            {
                y++;
                continue;
            }
        }if (activePiece != null)
        {
            activePiece.Set();
        }
    }
}
