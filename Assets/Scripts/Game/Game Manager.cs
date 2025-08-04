using UnityEngine;
using UnityEngine.InputSystem;

public class Game_Manager : MonoBehaviour
{
    // References to other managers
    public Board_Manager boardManager;
    public Player player;
    public PvP pvp;
    public PlayerInput playerInput;

    // Tetromino Data
    private TetrominoData heldTetromino;
    private TetrominoData nextTetromino;
    private TetrominoData currentTetromino;
    private TetrominoData previousTetromino;

    [SerializeField] private TetrominoData[] tetrominoSet;

    // timers
    private float timeElapsed;
    private float gravityTime;
    private float HD_Timer; //hard drop lockout
    private float lockoutDuration = 0.1f;
    public float invertTimer = 0f;
    private float g_IncreaseInt = 60f; // gravity increase interval
    private float currentGravityDelay;
    private float initialGravityDelay = 1.0f; // initial gravity delay
    private float minGravityDelay = 0.3f; // minimum gravity delay
    private float moveSens = 0.1f;
    public bool isPaused;
    public bool isGameOver;

    private void Awake()
    {
        // Initialize managers in the object hierarchy
        playerInput = GetComponent<PlayerInput>();
        boardManager = FindFirstObjectByType<Board_Manager>();
        pvp = FindFirstObjectByType<PvP>();
        player = FindFirstObjectByType<Player>();
    }

    public void Start()
    {
        currentGravityDelay = initialGravityDelay;
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        //AmmoBox.AmmoUpdateP1();
        //AmmoBox.GrenadeUpdateP1();
        SpawnNextPiece();
    }

    public void Update()
    {
        if (isPaused) return;
        float delta = Time.deltaTime;
        timeElapsed += delta;
        gravityTime += delta;

        if (HD_Timer > 0f)
        {
            HD_Timer -= Time.deltaTime;
        }
        else if (player.pendingDeadLines > 0)
        {

            ApplyDeadLine();
            player.pendingDeadLines--;
        }

        if (player.isInverted)
        {
            invertTimer -= delta;
            if (invertTimer <= 0f)
            {
                player.isInverted = false;
                Debug.Log("Controls returned to normal.");
            }
        }

        if (gravityTime >= g_IncreaseInt)
        {
            gravityTime = 0f;
            currentGravityDelay -= 0.1f;
            currentGravityDelay = Mathf.Max(currentGravityDelay, minGravityDelay);
        }
    }

    public void SpawnNextPiece()
    {
        // Clear any existing active piece before spawning a new one
        var existingPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece != null)
        {
            // Don't clear the piece if it's about to be destroyed (it's already locked)
            // The locked piece should remain on the board
            Destroy(existingPiece);
        }

        TetrominoData current = nextTetromino;

        int attempts = 0;
        do
        {
            int randomIndex = Random.Range(0, tetrominoSet.Length);
            nextTetromino = tetrominoSet[randomIndex];
            attempts++;
            if (attempts > 10) break;
        }
        while (nextTetromino == current);

        currentTetromino = current;
        previousTetromino = currentTetromino;

        GameObject pieceObj = new GameObject($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        pieceObj.transform.parent = this.transform; // Make it a child of Game_Manager
        Piece controller = pieceObj.AddComponent<Piece>();
        controller.board = boardManager;
        controller.data = currentTetromino;
        controller.position = new Vector2Int(0, boardManager.Bounds.yMax - 4);
        controller.gameManager = this;
    }

    public void TryHoldPiece(TetrominoData current, Piece controller)
    {
        if (isPaused) return;
        var activePiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();

        if (activePiece != null)
            activePiece.Clear();

        if (player.holdUsed)
        {
            // comboText.color = Color.red;
            // comboText.text = "SWAP LOCKED";
            Debug.Log("Hold already used this turn!");
            return;
        }
        controller.Clear();

        if (heldTetromino == null)
        {
            heldTetromino = current;
            SpawnNextPiece();
        }
        else
        {
            TetrominoData temp = heldTetromino;
            heldTetromino = current;
            SpawnHeldPiece(temp);
        }

        player.holdUsed = true;
        //holdDisplayUI.ShowHold(heldTetromino.tetromino);

        Destroy(controller.gameObject);
    }

    public void SpawnHeldPiece(TetrominoData data)
    {
        // Clear any existing active piece before spawning a new one
        var existingPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece != null)
        {
            // Don't clear the piece if it's about to be destroyed (it's already locked)
            // The locked piece should remain on the board
            Destroy(existingPiece);
        }

        GameObject pieceObj = new GameObject($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        pieceObj.transform.parent = this.transform; // Make it a child of Game_Manager
        Piece controller = pieceObj.AddComponent<Piece>();
        controller.board = boardManager;
        controller.data = data;
        controller.position = new Vector2Int(0, boardManager.Bounds.yMax - 4);
        controller.gameManager = this;

        currentTetromino = data;
    }
    private void ApplyDeadLine()
    {
        var activePiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();

        if (activePiece != null)
            activePiece.Clear();

        boardManager.PushUp();
        boardManager.AddDeadLine();

        if (activePiece != null)
        {
            if (!activePiece.IsValidPosition(activePiece.position))
            {
                activePiece.LockPiece(); // Lock if overlapping right away
            }
            else
            {
                if (!activePiece.TryMove(Vector2Int.down))
                {
                    activePiece.LockPiece(); // Lock if resting
                }
                else
                {
                    activePiece.Set(); // Update ghost/position
                }
            }
        }
    }

     public void ComboCount()
    {
        int linesCleared = boardManager.ClearLines();
        player.score += 100 * linesCleared;

        if (linesCleared > 0)
        {
            player.comboCount += linesCleared;

            int milestone = player.comboCount / 2;
            if (milestone > player.lastComboMilestone)
            {
                int ammoToAdd = milestone - player.lastComboMilestone;
                for (int i = 0; i < ammoToAdd; i++)
                {
                    if (player.attackAmmo < player.maxAmmo)
                    {
                        player.attackAmmo++;
                    }
                }
                player.lastComboMilestone = milestone;
                //AmmoBox.AmmoUpdateP1();
            }
            if (player.comboCount >= 4 && !player.hasEmpGrenade && !player.empOnCooldown)
            {
                player.hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                //AmmoBox.GrenadeUpdateP1();
            }

            if (player.comboCount > 1)
            {
                player.score += 100;
                //playercomboText.color = Color.yellow;
                //comboText.text = $"Combo x{comboCount}";

                //nt soundIndex = Mathf.Clamp(comboCount, 2, 13);
                //PlayComboSFX(soundIndex);
            }
            else
            {
                //AudioManager.Instance.PlaySFX(AudioManager.Instance.clear1);
            }
        }
        else
        {
            player.comboCount = 0;
            player.lastComboMilestone = 0;
            //comboText.text = "";
        }

        //scoreDisplay.text = $"{player.score}";
    }

    public void ReceiveDeadLine()
    {
        // if (chosenCharacter == 4 && yunJinSkill.rockCount > 0)
        // {
        //     yunJinSkill.Camera.SetTrigger("Shake");
        //     yunJinSkill.InvisRock(yunJinSkill.rockCount);
        //     yunJinSkill.rockCount--;
        //     return;
        // }

        if (HD_Timer > 0f)
        {
            // Delay deadline, queue it
            player.pendingDeadLines++;
            Debug.Log("Dead line queued due to Hard Drop lockout");
            return;
        }

        ApplyDeadLine();
    }

    public void ResetEmpCooldown()
    {
        player.empOnCooldown = false;
        //comboText.text = "";
        Debug.Log("EMP cooldown reset!");
    }
    public void TriggerHardDropLockout()
    {
        HD_Timer = lockoutDuration;
    }

    public float GetGravityDelay()
    {
        return currentGravityDelay;
    }

    public void ResetHold()
    {
        player.holdUsed = false;
    }

    public float GetMovementSensitivity()
    {
        return moveSens;
    }


    public void GameOver()
    {
        isGameOver = true;
        boardManager.ClearAll();
        boardManager.ghost_tilemap.ClearAllTiles();
        // nextDisplayUI.HideAll();
        // holdDisplayUI.HideAll();
        heldTetromino = null;
        player.holdUsed = false;
        GameObject existingPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
    }
}
