using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    private int comboCount = 0;

    [Header("References")]
    [SerializeField] public BoardManager board;
    public GameManagerP2 gameP2;
    public VsGameOverManager vsGameOverManager;
    private P1Controller controls;
    [SerializeField] private TetrominoData[] tetrominoSet;
    public NextDisplayUI nextDisplayUI;
    public HoldDisplayUI holdDisplayUI;
    public TMP_Text comboText;
    public TMP_Text scoreDisplay;
    public AmmoDisplay AmmoBox;
    public EMPAnim empAnim;
    public Animator Camera;

    [Header("Character Skill References")]
    public TetroSkill tetroSkill;
    public ScorchSkill scorchSkill;
    public PackHatSkill packHatSkill;
    public KorSkill korSkill;
    public DodokeSkill dodokeSkill;
    public YunJinSkill yunJinSkill;
    public GameObject deductScoreText;
    public Coroutine deductScoreCoroutine;

    [Header("Gameplay Settings")]
    public int chosenCharacter;
    public bool enableSpawn = true;
    public static float movementSensitivity = 0.10f;
    [SerializeField] private float movementSpeedMultiplier = 1.0f;
    private int lastComboMilestone = 0;
    private int scoreMultiplier = 1;
    private TetrominoData previousTetromino;
    private TetrominoData currentTetromino;
    public TetrominoData nextTetromino;
    public TetrominoData heldTetromino;
    public bool holdUsedThisTurn = false;
    public int P1score = 0;
    public TimerDisplay time;
    public bool isGameOver = false;
    public bool isPaused = false;
    private int pendingDeadLines = 0;
    private float hardDropLockoutTimer = 0f;
    private float lockoutDuration = 0.1f;

    [Header("PVP Settings")]
    [SerializeField] public int attackAmmo = 0;
    private bool attackOnCooldown = false;
    [SerializeField] public static float attackCooldownTime = 1.5f;
    public float atkTempCD = attackCooldownTime;
    private const int maxAmmo = 5;
    public bool hasEmpGrenade = false;
    private bool empOnCooldown = false;
    public static float empCooldownDuration = 30f;
    internal bool isInverted = false;
    internal float invertTimer = 0f;
    public bool dontInvert = false;
    public InputAction SkillAction => controls.VsModeP1.Skill;


    [Header("Gravity Settings")]
    [SerializeField] internal static float initialGravityDelay = 1f;
    [SerializeField] private float gravityIncreaseInterval = 60f;
    [SerializeField] private float minGravityDelay = 0.2f;
    
    private float currentGravityDelay;
    private float elapsedTime = 0f;
    private float gravityTime = 0f;

    public void Start()
    {
        attackCooldownTime = atkTempCD;
        chosenCharacter = CharacterSelector.P1ChosenCharacter;
        movementSensitivity = P1Settings.GetSensitivity();
        currentGravityDelay = initialGravityDelay;
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
        AmmoBox.AmmoUpdateP1();
        AmmoBox.GrenadeUpdateP1();
        SpawnNextPiece();
    }

    public void Update()
    {
        if (isPaused) return;
        float delta = Time.deltaTime;
        elapsedTime += delta;
        gravityTime += delta;

        if (hardDropLockoutTimer > 0f)
        {
            hardDropLockoutTimer -= Time.deltaTime;
        }
        else if (pendingDeadLines > 0)
        {
            
            ApplyDeadLine();
            pendingDeadLines--;
        }

        if (isInverted)
        {
            invertTimer -= delta;
            if (invertTimer <= 0f)
            {
                isInverted = false;
                Debug.Log("Controls returned to normal.");
            }
        }

        if (scoreMultiplier == 1 && elapsedTime >= 240f)
        {
            scoreMultiplier = 2;
            Debug.Log("2x Scores");
        }

        if (gravityTime >= gravityIncreaseInterval)
        {
            gravityTime = 0f;
            currentGravityDelay -= 0.1f;
            currentGravityDelay = Mathf.Max(currentGravityDelay, minGravityDelay);
        }
    }

    public void Awake()
    {
        controls = new P1Controller();
        controls.VsModeP1.Attack.performed += ctx => TryAttack();
        controls.VsModeP1.EMPGrenade.performed += ctx => TryUseEmpGrenade();
        controls.VsModeP1.Pause.performed += ctx => TogglePause();
        controls.VsModeP1.Skill.performed += ctx => TryCharacterSkill();
    }

    public void SpawnNextPiece()
    {
        if (enableSpawn == false)
        {
            return;
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

        nextDisplayUI.ShowNext(nextTetromino.tetromino);

        currentTetromino = current;
        previousTetromino = currentTetromino;

        GameObject pieceObj = new GameObject("ActivePieceP1");
        PieceController controller = pieceObj.AddComponent<PieceController>();
        controller.board = board;
        controller.data = currentTetromino;
        controller.position = new Vector2Int(0, board.Bounds.yMax - 4);
        controller.gameManager = this;
    }

    public void TryHoldPiece(TetrominoData current, PieceController controller)
    {
        if (isPaused) return;
        var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();

        if (activePiece != null)
            activePiece.Clear();

        if (holdUsedThisTurn)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.invalid);
            comboText.color = Color.red;
            comboText.text = "SWAP LOCKED";
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

        AudioManager.Instance.PlaySFX(AudioManager.Instance.holdClip);
        holdUsedThisTurn = true;
        holdDisplayUI.ShowHold(heldTetromino.tetromino);

        Destroy(controller.gameObject);
    }

    public void SpawnHeldPiece(TetrominoData data)
    {
        GameObject pieceObj = new GameObject("ActivePieceP1");
        PieceController controller = pieceObj.AddComponent<PieceController>();
        controller.board = board;
        controller.data = data;
        controller.position = new Vector2Int(0, board.Bounds.yMax - 4);
        controller.gameManager = this;

        currentTetromino = data;
    }

    public void ResetHold()
    {
        holdUsedThisTurn = false;
    }

    public void ComboCount()
    {
        int linesCleared = board.ClearLines();
        P1score += 100 * linesCleared;

        if (linesCleared > 0)
        {
            comboCount += linesCleared;

            int milestone = comboCount / 2;
            if (milestone > lastComboMilestone)
            {
                int ammoToAdd = milestone - lastComboMilestone;
                for (int i = 0; i < ammoToAdd; i++)
                {
                    if (attackAmmo < maxAmmo)
                    {
                        attackAmmo++;
                    }
                }
                lastComboMilestone = milestone;
                AmmoBox.AmmoUpdateP1();
            }
            if (comboCount >= 4 && !hasEmpGrenade && !empOnCooldown)
            {
                hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                AmmoBox.GrenadeUpdateP1();
            }

            if (comboCount > 1)
            {
                P1score += 100;
                comboText.color = Color.yellow;
                comboText.text = $"Combo x{comboCount}";

                int soundIndex = Mathf.Clamp(comboCount, 2, 13);
                PlayComboSFX(soundIndex);
            }
            else
            {
                AudioManager.Instance.PlaySFX(AudioManager.Instance.clear1);
            }
        }
        else
        {
            comboCount = 0;
            lastComboMilestone = 0;
            comboText.text = "";
        }

        scoreDisplay.text = $"{P1score}";
    }

    public void TryUseEmpGrenade()
    {
        if (isPaused) return;
        if (hasEmpGrenade && !empOnCooldown)
        {
            hasEmpGrenade = false;
            empOnCooldown = true;
            Debug.Log("EMP Grenade used!");

            empAnim.ThrowP1();

            Invoke(nameof(ResetEmpCooldown), empCooldownDuration);

            AmmoBox.GrenadeUpdateP1();
        }
        else
        {
            Debug.Log("Cannot use EMP: Either on cooldown or not available.");
        }
    }

    private void ResetEmpCooldown()
    {
        empOnCooldown = false;
        comboText.text = "";
        Debug.Log("EMP cooldown reset!");
    }

    public void TryAttack()
    {
        if (isPaused) return;

        if (attackOnCooldown)
        {
            comboText.color = Color.red;
            comboText.text = "Attack on Cooldown";
            Debug.Log("Attack is on cooldown!");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.invalid);
            return;
        }

        if (attackAmmo > 0)
        {
            attackAmmo--;
            Camera.SetTrigger("Shake");
            var opponentPiece = GameObject.Find("ActivePieceP2")?.GetComponent<PieceControllerP2>();
            if (opponentPiece != null)
            {
                opponentPiece.Clear(); // Clear its tiles temporarily
            }

            gameP2.ReceiveDeadLine();

            if (opponentPiece != null)
            {
                opponentPiece.Set(); // Re-set the piece tiles after push
            }

            AmmoBox.AmmoUpdateP1();
            Debug.Log("Attack sent!");
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);


            // Start cooldown
            attackOnCooldown = true;
            Invoke(nameof(ResetAttackCooldown), attackCooldownTime);
        }
        else
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.invalid);
            comboText.color = Color.red;
            comboText.text = "No Ammo";
            Debug.Log("No ammo!");
        }
    }

    private void ResetAttackCooldown()
    {
        attackOnCooldown = false;
        comboText.text = "";
        Debug.Log("Attack cooldown reset.");
    }

    public void TryCharacterSkill()
    {
        switch (chosenCharacter)
        {
            case 1: tetroSkill.BallAnim(); break;
            case 2:
                {
                    if (korSkill.ActivateSkill() == false)
                    {
                        Debug.Log("Kor skill is on cooldown");
                        return;
                    }
                    StartCoroutine(KorSkillRoutine());
                    break;
                }
                
            case 3:
                {
                    if (packHatSkill.ActivateSkill() == false)
                    {
                        Debug.Log("PackHat skill is on cooldown");
                        return;
                    }
                    StartCoroutine(PackHatSkillRoutine());
                    break;
                }
            case 4:
                {
                    yunJinSkill.StartSkillChargeDetection(SkillAction, (charge) =>
                    {
                        yunJinSkill.ActivateSkill(charge);
                    });
                    break;
                }
            case 5:
                dodokeSkill.ActivateSkill();
                break;

            case 6:
                {
                    if (P1score == 0 || P1score < 500)
                    {
                        AudioManager.Instance.PlaySFX(AudioManager.Instance.invalid);
                        comboText.color = Color.red;
                        comboText.text = "Not enough points (500)";
                        Debug.Log("Scorch skill requires 500 points to activate");
                        return;
                    }
                    if (scorchSkill.ActivateSkill() == false)
                    {
                        Debug.Log("Scorch skill is on cooldown");
                        return;
                    }
                    board.ScorchSkillClearBottomLines();
                    P1score -= 500;
                    scoreDisplay.text = $"{P1score}";
                    StartCoroutine(DeductScoreDisplayRoutine());
                    break;
                }
            default: Debug.Log("No skill available for this character!"); break;
        }
    }

    public IEnumerator DeductScoreDisplayRoutine()
    {
        Debug.Log("Should fade out");
        deductScoreText.SetActive(true);
        TextMeshProUGUI scoreText = deductScoreText.GetComponent<TextMeshProUGUI>();
        Color baseColor = scoreText.color;
        scoreText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);

        float fadeDuration = 1f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            scoreText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        deductScoreText.SetActive(false);
    }

    private IEnumerator PackHatSkillRoutine()
    {
        Debug.Log("PackHat skill activated: No attack cooldown for 10 seconds!");
        packHatSkill.Transform();
        float originalCooldown = attackCooldownTime;
        attackCooldownTime = 0f;

        float duration = 10f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        packHatSkill.NormalState();
        attackCooldownTime = originalCooldown;
        Debug.Log("PackHat skill expired: Attack cooldown restored.");
    }

    private IEnumerator KorSkillRoutine()
    {
        Debug.Log("Kor skill activated: Gravity frozen for 10 seconds!");

        float duration = 10f;
        float originalGravityDelay = currentGravityDelay;

        // Freeze gravity by setting delay to an extremely high value
        currentGravityDelay = float.MaxValue;

        float timer = 0f;
        while (timer < duration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        // Restore original gravity delay
        currentGravityDelay = originalGravityDelay;
        Debug.Log("Kor skill expired: Gravity restored.");
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

    public void TogglePause()
    {
        if (isGameOver) return; // Don't allow pause if already game over
        GamePause.Instance?.TogglePause();
    }

    public void GameOver()
    {
        isGameOver = true;
        vsGameOverManager.OnPlayer1Fail();
        board.ClearAll();
        board.ghostTilemap.ClearAllTiles();
        nextDisplayUI.HideAll();
        holdDisplayUI.HideAll();
        heldTetromino = null;
        holdUsedThisTurn = false;
        GameObject existingPiece = GameObject.Find("ActivePieceP1");
        if (existingPiece) Destroy(existingPiece);
        int randomIndex = Random.Range(0, tetrominoSet.Length);
        nextTetromino = tetrominoSet[randomIndex];
    }

    public void ReceiveDeadLine()
    {
        if (chosenCharacter == 4 && yunJinSkill.rockCount > 0)
        {
            yunJinSkill.Camera.SetTrigger("Shake");
            yunJinSkill.InvisRock(yunJinSkill.rockCount);
            yunJinSkill.rockCount--;
            return;
        }

        if (hardDropLockoutTimer > 0f)
        {
            // Delay deadline, queue it
            pendingDeadLines++;
            Debug.Log("Dead line queued due to Hard Drop lockout");
            return;
        }

        ApplyDeadLine();
    }

    private void ApplyDeadLine()
    {
        var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();

        if (activePiece != null)
            activePiece.Clear();

        board.PushUp();
        board.AddDeadLine();

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

    public void TriggerHardDropLockout()
    {
        hardDropLockoutTimer = lockoutDuration;
    }

    public float GetGravityDelay()
    {
        return currentGravityDelay;
    }

    public void ApplyInvertControlDebuff(float duration)
    {
        if (dontInvert == true)
        {
            yunJinSkill.Camera.SetTrigger("Shake");
            Debug.Log("EMP blocked");
            dontInvert = false;
            return;
        }

        if (!isInverted)
        {
            yunJinSkill.Camera.SetTrigger("Shake");
            isInverted = true;
            invertTimer = duration;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.EMP_clip);
            var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();
            if (activePiece != null)
                activePiece.Clear();
            comboText.color = Color.red;
            comboText.text = "Inverted Controls";
            Debug.Log("Controls inverted!");
        }

    }

    public void OnEnable()
    {
        controls.VsModeP1.Enable();
    }

    public void OnDisable()
    {
        controls.VsModeP1.Disable();
    }

    public float GetMovementSpeedMultiplier()
    {
        return movementSpeedMultiplier;
    }

    public float GetMovementSensitivity()
    {
        return movementSensitivity;
    }
}
