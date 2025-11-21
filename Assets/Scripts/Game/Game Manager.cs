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
    public GameOverManager gameOverManager;
    public ScorchCursor scorchCursor;
    public Shaker shaker;

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
    public bool isSolo = false;

    private int goalScore = 5000;

    public int inflationCtr = 0;
    public bool disableSpawn = false;

    // EMP Cooldown Timer
    private float empCooldownTimer = 0f;

    public AudioManager audioManager;

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
        audioManager = GameObject.Find("Audio Manager").GetComponent<AudioManager>();
    }

    public void Start()
    {
        currentGravityDelay = initialGravityDelay;
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        if (isSolo)
        {
            gameDisplay.LevelUpdate(player.level-1);
        }
        else if (!isSolo)
        {
            gameDisplay.EMP_CD_Update(0f);
            gameDisplay.Ammo_Update(player.attackAmmo);
            gameDisplay.UpdateEMPStateIcon();
            gameDisplay.UpdateHeartIcons(player.lives);
        }
        gameDisplay.UpdateChips(player.score); // Initialize chips display
        gameDisplay.LogTetrominoStatus(nextTetromino, heldTetromino); // Log initial tetromino status
        gameDisplay.UpdateComboText();
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

        if (player.score > goalScore && isSolo)
        {
            currentGravityDelay -= 0.02f;
            gameDisplay.LevelUpdate(player.level);
            player.level++;
            if (currentGravityDelay <= 0)
            {
                currentGravityDelay = 0.02f;
            }
            goalScore += 5000;
        }
        
        if (gravityTime >= g_IncreaseInt && !isSolo)
        {
            gravityTime = 0f;
            currentGravityDelay -= 0.2f;
            currentGravityDelay = Mathf.Max(currentGravityDelay, minGravityDelay);
        }
    }

    public void SpawnNextPiece()
    {
        if (disableSpawn == true)
        {
            return;
        }
        // Note: Don't check isGameOver here - the catching up player needs to spawn pieces
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
        if (isGameOver) return;
        
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
        if (isGameOver) return;

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
                if (!isSolo)
                {
                    gameDisplay.Ammo_Update(player.attackAmmo);
                }
            }
            if (player.comboCount >= 4 && !player.hasEmpGrenade && !player.empOnCooldown && !isSolo)
            {
                player.hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                gameDisplay.UpdateEMPStateIcon();
                shaker.EMPShake();
            }

            if (player.comboCount > 1)
            {
                player.score += 100;
                gameDisplay.UpdateComboText();
                shaker.ComboShake();

                int soundIndex = Mathf.Clamp(player.comboCount, 2, 13);
                PlayComboSFX(soundIndex);
            }
            else
            {
                audioManager.PlaySFX(audioManager.clear1);
            }
            shaker.ChipsShake();
        }
        else
        {
            player.comboCount = 0;
            player.lastComboMilestone = 0;
            shaker.ComboInvalidShake(); 
        }

        gameDisplay.UpdateChips(player.score);
    }

    private void PlayComboSFX(int combo)
    {
        switch (combo)
        {
            case 2: audioManager.PlaySFX(audioManager.clear2); break;
            case 3: audioManager.PlaySFX(audioManager.clear3); break;
            case 4: audioManager.PlaySFX(audioManager.clear4); break;
            case 5: audioManager.PlaySFX(audioManager.clear5); break;
            case 6: audioManager.PlaySFX(audioManager.clear6); break;
            case 7: audioManager.PlaySFX(audioManager.clear7); break;
            case 8: audioManager.PlaySFX(audioManager.clear8); break;
            case 9: audioManager.PlaySFX(audioManager.clear9); break;
            case 10: audioManager.PlaySFX(audioManager.clear10); break;
            case 11: audioManager.PlaySFX(audioManager.clear11); break;
            case 12: audioManager.PlaySFX(audioManager.clear12); break;
            case 13: audioManager.PlaySFX(audioManager.clear13); break;
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

        // Check if player has Yun Jin rocks to block the attack
        if (TryBlockWithYunJinRocks())
        {
            Debug.Log("Attack blocked by Yun Jin rocks!");
            return;
        }

        shaker.boardShake();
        ApplyDeadLine();
    }

    private bool TryBlockWithYunJinRocks()
    {
        // Find the character manager for this player
        GameObject characterManagerObj = GameObject.Find($"Character Manager {(player.isPlayer1 ? "P1" : "P2")}");
        if (characterManagerObj == null)
        {
            return false;
        }

        // Check if this player has Yun Jin skill
        YunJinSkill yunJinSkill = characterManagerObj.GetComponent<YunJinSkill>();
        if (yunJinSkill == null)
        {
            return false;
        }

        // Check if any rocks are active and can block
        if (yunJinSkill.Rock1Active || yunJinSkill.Rock2Active || yunJinSkill.Rock3Active)
        {
            // Use the first available rock to block the attack
            if (yunJinSkill.Rock1Active)
            {
                yunJinSkill.InvisRock(1);
                yunJinSkill.StoneDestroyed();
                Debug.Log("Rock1 blocked the incoming attack!");
                return true;
            }
            else if (yunJinSkill.Rock2Active)
            {
                yunJinSkill.InvisRock(2);
                Debug.Log("Rock2 blocked the incoming attack!");
                return true;
            }
            else if (yunJinSkill.Rock3Active)
            {
                yunJinSkill.InvisRock(3);
                Debug.Log("Rock3 blocked the incoming attack!");
                return true;
            }
        }

        return false;
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
        shaker.boardShake();
        player.lives--;
        Debug.Log($"Player lost a life! Lives remaining: {player.lives}");

        // Update UI to show remaining lives
        if (gameDisplay != null && !isSolo)
        {
            gameDisplay.UpdateHeartIcons(player.lives);
            gameDisplay.Ammo_Update(player.attackAmmo);
            gameDisplay.UpdateEMPStateIcon();
        }

        if (pvp.isSolo && player.lives <= 0)
        {
            GameOver();
        }
        else if (player.lives <= 0 && !pvp.isSolo)
        {
            // Check if opponent is already out of lives
            if (pvp.opponent.lives <= 0)
            {
                // Both players are out of lives - game ends based on score
                GameOver();
            }
            else
            {
                // Only this player is out of lives - check if catch-up is needed
                CheckCatchUpCondition();
            }
        }
        else
        {
            // Reset the board and continue the game
            ResetBoardAfterLifeLoss();
            gameDisplay.UpdateComboText();
            boardManager.ClearAll();
            boardManager.ghost_tilemap.ClearAllTiles();
        }
    }

    private void CheckCatchUpCondition()
    {
        // If this player has higher or equal score, opponent can still catch up
        if (player.score >= pvp.opponent.score)
        {
            isGameOver = true;
            boardManager.ClearAll();
            boardManager.ghost_tilemap.ClearAllTiles();
            
            // Clear pieces
            GameObject Piece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
            Destroy(Piece);
            
            heldTetromino = null;
            player.holdUsed = false;
            player.lastComboMilestone = 0;
            
            // Opponent gets to continue (they're still active)
            // Set opponent's isGameOver to false so they can keep playing
            pvp.opponentGameManager.isGameOver = false;
            
            string playerId = player.isPlayer1 ? "P1" : "P2";
            string opponentId = player.isPlayer1 ? "P2" : "P1";
            Debug.Log($"{playerId} ran out of lives with score {player.score}. {opponentId} can still catch up!");
            
            // Let the PvP system handle the catch-up phase
            StartCoroutine(WaitForCatchUpCompletion());
        }
        else
        {
            // This player has lower score and is out of lives - they lost
            player.isWinner = false;
            pvp.opponent.isWinner = true;
            GameOver();
        }
    }

    private System.Collections.IEnumerator WaitForCatchUpCompletion()
    {
        Player catchingUpPlayer = pvp.opponent;
        Player fallenPlayer = player;
        int targetScore = fallenPlayer.score;
        
        string catchingPlayerId = catchingUpPlayer.isPlayer1 ? "P1" : "P2";
        Debug.Log($"Catch-up phase started! {catchingPlayerId} needs to reach {targetScore} points.");
        
        // Wait while opponent is playing catch-up
        while (catchingUpPlayer.lives > 0 && catchingUpPlayer.score < targetScore)
        {
            yield return null;
        }

        // Catch-up phase ended
        if (catchingUpPlayer.score >= targetScore)
        {
            Debug.Log($"{catchingPlayerId} successfully caught up! Score: {catchingUpPlayer.score}");
            // Continue the game - both players can still compete
            isGameOver = false;
            if (!catchingUpPlayer.gameManager.isGameOver)
            {
                catchingUpPlayer.gameManager.isGameOver = false;
            }
        }
        if (catchingUpPlayer.score > targetScore)
        {
            player.isWinner = false;
            catchingUpPlayer.isWinner = true;
            GameOver();
        }
        else
        {
            player.isWinner = true;
            catchingUpPlayer.isWinner = false;
            GameOver();
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
        boardManager.ClearAll();
        boardManager.ghost_tilemap.ClearAllTiles();
        
        Invoke(nameof(LossRespawn), 3f);
        
        Debug.Log("Board reset after life loss. Game continues!");
    }

    public void LossRespawn()
    {
        GameObject existingPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (existingPiece) Destroy(existingPiece);
        boardManager.ClearAll();
        boardManager.ghost_tilemap.ClearAllTiles();
        SpawnNextPiece();
    }

    public void GameOver()
    {
        isGameOver = true;
        boardManager.ClearAll();
        boardManager.ghost_tilemap.ClearAllTiles();
        boardManager.receivedDeadLineCount = 0;
        // nextDisplayUI.HideAll();
        // holdDisplayUI.HideAll();
        heldTetromino = null;
        player.holdUsed = false;
        player.lastComboMilestone = 0;
        GameObject P1Piece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}");
        if (P1Piece) Destroy(P1Piece);
        GameObject P2Piece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P2" : "P1")}");
        if (P2Piece) Destroy(P2Piece);
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        gameOverManager.TriggerGameOver();
        gameDisplay.UpdateComboText();

        Debug.Log("Game Over!");
    }
}
