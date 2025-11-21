using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PvP : MonoBehaviour
{
    public Game_Manager gameManager;
    public GameDisplay gameDisplay;
    public EmpEvents emp_events;
    public YunJinEvents yunJinEvents;
    public YunJinSkill yunJinSkill;
    public PackHatSkill packHatSkill;
    public GameObject charManagerObj;


    public Game_Manager opponentGameManager;

    public Player player;
    public Player opponent;

    public bool isInvertImmune = false;
    public bool isSolo;

    PlayerInput playerInput;
    InputAction empGrenadeAction;
    InputAction attackAction;

    private void Awake()
    {
        isSolo = gameManager.isSolo;
        // assign oponent based on the opposite isPlayer1 value
        opponent = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p != player);

        if (player.isPlayer1)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P1");
            playerInput = playerObj.GetComponent<PlayerInput>();
            yunJinEvents = GameObject.Find("Character Manager P1").GetComponent<YunJinEvents>();
        }
        else
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("P2");
            playerInput = playerObj.GetComponent<PlayerInput>();
            yunJinEvents = GameObject.Find("Character Manager P2").GetComponent<YunJinEvents>();
        }

        if (!isSolo)
        {
            empGrenadeAction = playerInput.actions["EMP"];
            attackAction = playerInput.actions["Attack"];

            empGrenadeAction.performed += ctx => TryUseEmpGrenade();
            attackAction.performed += ctx => TryAttack();
        }
    }

    private void Start()
    {
        if (isSolo) { return; }
        yunJinSkill = yunJinEvents.yunJinSkill;
        packHatSkill = charManagerObj.GetComponent<PackHatSkill>();
    }

    public void TryUseEmpGrenade()
    {
        if (Game_Manager.isPaused) return;
        if (gameManager.isTimeStopped) return;
        if (opponentGameManager.isGameOver) return;

        if (player.hasEmpGrenade && !player.empOnCooldown)
        {
            emp_events.EmpAnim.SetTrigger("Throw");
        }
        else
        {
            Debug.Log("Cannot use EMP: Either on cooldown or not available.");
        }
    }

    public void DetonateEmp()
    {
        player.hasEmpGrenade = false;
        Debug.Log("EMP Grenade used!");
        opponent.gameManager.pvp.ApplyInvertControlDebuff(10f);

        gameManager.StartEmpCooldown();
        gameDisplay.UpdateEMPStateIcon();
    }

    public void ApplyInvertControlDebuff(float duration)
    {
        if (TryBlockEmp() == true)
        {
            Debug.Log("EMP Blocked by Yun Jin's Rocks");
            return;
        }
        gameManager.shaker.boardShake();
        if (isInvertImmune)
        {
            isInvertImmune = !isInvertImmune;
            return;
        }
        if (!player.isInverted)
        {
            player.isInverted = true;
            gameManager.invertTimer = duration;
            var activePiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();
            if (activePiece != null)
                activePiece.Clear();
            // comboText.color = Color.red;
            // comboText.text = "Inverted Controls";
            Debug.Log("Controls inverted!");
            gameManager.audioManager.PlaySFX(gameManager.audioManager.EMP_clip);
            StartCoroutine(gameDisplay.BackPulse(10f, "#763700")); // "#763700"
        }
    }
    
    public bool TryBlockEmp()
    {
        if(yunJinSkill != null)
        {
            int rockCtr = yunJinSkill.rockCount;
            if (rockCtr > 0)
            {
                for (int i = rockCtr; i > 0; i--)
                {
                    yunJinSkill.InvisRock(i);
                }
                yunJinSkill.StoneDestroyed();
                return true;
            }
            
        }
        return false;
    }

    public void TryAttack()
    {
        if (Game_Manager.isPaused) return;
        if (gameManager.isTimeStopped) return;
        if (opponentGameManager.isGameOver) return;

        if (player.atkOnCooldown)
        {
            //comboText.color = Color.red;
            //comboText.text = "Attack on Cooldown";
            Debug.Log("Attack is on cooldown!");
            return;
        }

        if (player.attackAmmo > 0)
        {
            
            if (packHatSkill != null)
            {
                if (packHatSkill.isSkillActive)
                {
                    packHatSkill.packhatAnim.Play("Firing", 0, 0f);
                }
            }

            player.attackAmmo--;
            //Camera.SetTrigger("Shake");
            var opponentPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();
            if (opponentPiece != null)
            {
                opponentPiece.Clear(); // Clear its tiles temporarily
            }

            opponent.gameManager.ReceiveDeadLine();
            gameManager.audioManager.PlaySFX(gameManager.audioManager.attack);

            if (opponentPiece != null)
            {
                opponentPiece.Set(); // Re-set the piece tiles after push
            }

            gameManager.gameDisplay.Ammo_Update(player.attackAmmo);
            Debug.Log("Attack sent!");


            // Start cooldown
            player.atkOnCooldown = true;
            Invoke(nameof(ResetAttackCooldown), player.atkCD_Time);
        }
        else
        {
            //comboText.color = Color.red;
            //comboText.text = "No Ammo";
            Debug.Log("No ammo!");
        }
    }

    private void ResetAttackCooldown()
    {
        player.atkOnCooldown = false;
        //comboText.text = "";
        Debug.Log("Attack cooldown reset.");
    }
}
