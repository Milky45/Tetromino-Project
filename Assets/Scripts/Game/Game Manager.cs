using UnityEngine;
using UnityEngine.InputSystem;

public class Game_Manager : MonoBehaviour
{
    // References to other managers
    public Board_Manager boardManager;
    public Player player;
    public PvP pvp;
    public PlayerInput playerInput;
    public GameDisplay gameDisplay; // Reference to GameDisplay for UI updates

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
    public float currentGravityDelay;
    private float initialGravityDelay = 0.8f; // initial gravity delay
    private float minGravityDelay = 0.25f; // minimum gravity delay
    private float moveSens = 0.1f;
    public static bool isPaused;
    public bool isTimeStopped = false;
    public bool isGameOver;

    public int inflationCtr = 0;

    // EMP Cooldown Timer
    private float empCooldownTimer = 0f;

    private void Awake()
    {
        // Find the PlayerInput by tag "P1" or "P2"
        if (player.isPlayer1)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P1");
            playerInput = playerObj.GetComponent<PlayerInput>();
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P2");
            playerInput = playerObj.GetComponent<PlayerInput>();
        }

    }

    public void Start()
    {
        currentGravityDelay = initialGravityDelay;
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        gameDisplay.EMP_CD_Update(0f); // Initialize EMP cooldown display
        gameDisplay.Ammo_Update(player.attackAmmo); // Initialize ammo display
        gameDisplay.UpdateChips(player.score); // Initialize chips display
        gameDisplay.UpdateHeartIcons(player.lives);
        gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log initial tetromino status

        gameDisplay.UpdateEMPStateIcon();
        SpawnNextPiece();
    }

    public void Update()
    {
        if (isPaused) return;
        if (isTimeStopped) return;

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

        // Update EMP cooldown timer
        if (player.empOnCooldown)
        {
            empCooldownTimer -= delta;
            empCooldownTimer = Mathf.Max(empCooldownTimer, 0f);
            
            // Update the UI display
            if (gameDisplay != null)
            {
                gameDisplay.EMP_CD_Update(empCooldownTimer);
            }
            
            // Check if cooldown is finished
            if (empCooldownTimer <= 0f)
            {
                ResetEmpCooldown();
            }
        }

        if (gravityTime >= g_IncreaseInt)
        {
            gravityTime = 0f;
            currentGravityDelay -= 0.2f;
            currentGravityDelay = Mathf.Max(currentGravityDelay, minGravityDelay);
            inflationCtr++;
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
        controller.data = currentTetromino;
        controller.position = new Vector2Int(0, boardManager.Bounds.yMax - 4);
        controller.gameManager = this;
        controller.board = boardManager;
        gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after next changes
    }

    public void TryHoldPiece(TetrominoData current, Piece controller)
    {
        if (isPaused) return;
        if (isTimeStopped) return;
        
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
            gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after hold
            SpawnNextPiece();
        }
        else
        {
            TetrominoData temp = heldTetromino;
            heldTetromino = current;
            gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after swap
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
        controller.data = data;
        controller.position = new Vector2Int(0, boardManager.Bounds.yMax - 4);
        controller.gameManager = this;
        controller.board = boardManager;
        currentTetromino = data;
        
        gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after held piece spawn
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
                gameDisplay.Ammo_Update(player.attackAmmo);
            }
            if (player.comboCount >= 4 && !player.hasEmpGrenade && !player.empOnCooldown)
            {
                player.hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                gameDisplay.UpdateEMPStateIcon();
            }

            if (player.comboCount > 1)
            {
                player.score += 100;
                //playercomboText.color = Color.yellow;
                //comboText.text = $"Combo x{comboCount}";

                int soundIndex = Mathf.Clamp(player.comboCount, 2, 13);
                PlayComboSFX(soundIndex);
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.clear1);
            }
        }
        else
        {
            player.comboCount = 0;
            player.lastComboMilestone = 0;
            //comboText.text = "";
        }

        gameDisplay.UpdateChips(player.score);
    }

    private void PlayComboSFX(int combo)
    {
        switch (combo)
        {
            case 2: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear2); break;
            case 3: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear3); break;
            case 4: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear4); break;
            case 5: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear5); break;
            case 6: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear6); break;
            case 7: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear7); break;
            case 8: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear8); break;
            case 9: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear9); break;
            case 10: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear10); break;
            case 11: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear11); break;
            case 12: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear12); break;
            case 13: AudioManager.Instance.PlaySFX(AudioManager.Instance.clear13); break;
            default: break;
        }
    }


    public void ReceiveDeadLine()
    {
        if (HD_Timer > 0f)
        {
            // Delay deadline, queue it
            player.pendingDeadLines++;
            Debug.Log("Dead line queued due to Hard Drop lockout");
            return;
        }

        ApplyDeadLine();
    }

    public void StartEmpCooldown()
    {
        player.empOnCooldown = true;
        empCooldownTimer = player.empCooldownDuration;
        Debug.Log($"EMP cooldown started for {player.empCooldownDuration} seconds!");
        gameDisplay.UpdateEMPStateIcon();
    }

    public void ResetEmpCooldown()
    {
        player.empOnCooldown = false;
        empCooldownTimer = 0f;
        
        // Update the UI display to show 0 or clear the text
        if (gameDisplay != null)
        {
            gameDisplay.EMP_CD_Update(0f);
        }
        gameDisplay.UpdateEMPStateIcon();
        
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


    public void LoseLife()
    {
        player.lives--;
        Debug.Log($"Player lost a life! Lives remaining: {player.lives}");
        
        // Update UI to show remaining lives
        if (gameDisplay != null)
        {
            gameDisplay.UpdateHeartIcons(player.lives);
        }
        
        if (player.lives <= 0)
        {
            // No more lives, game is truly over
            GameOver();
        }
        else
        {
            // Reset the board and continue the game
            ResetBoardAfterLifeLoss();
        }
    }
    
    private void ResetBoardAfterLifeLoss()
    {
        GameObject existingPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);
        // Clear the board
        boardManager.ClearAll();
        boardManager.ghost_tilemap.ClearAllTiles();
        
        // Reset game state
        heldTetromino = null;
        player.holdUsed = false;
        player.comboCount = 0;
        player.lastComboMilestone = 0;
        player.attackAmmo = 0;
        player.hasEmpGrenade = false;
        
        
        // Clear any existing piece

        
        // Reset pending deadlines
        player.pendingDeadLines = 0;
        
        // Spawn a new piece to continue the game
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log after board reset
        
        Invoke(nameof(SpawnNextPiece), 3f);
        
        Debug.Log("Board reset after life loss. Game continues!");
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
        
        Debug.Log("Game Over! No more lives remaining.");
    }
}
