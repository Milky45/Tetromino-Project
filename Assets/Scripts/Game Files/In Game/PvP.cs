using UnityEngine;
using UnityEngine.InputSystem;

public class PvP : MonoBehaviour
{
    [Header("References")]
    public PlayerManager playerManager;
    public PlayerBoard playerBoard;
    public PlayerStatus playerStatus;
    public PlayerManager opponentPlayerManager;
    public PlayerStatus opponentPlayerStatus;
    public GameDisplay gameDisplay;
    public EmpManager empManager; // flag this for future enhancements

    public bool isInvertImmune = false;
    PlayerInput playerInput;
    InputAction empGrenadeAction;
    InputAction attackAction;

    private void Awake()
    {
        // isSolo = playerManager.isSolo;
        // assign oponent based on the opposite isPlayer1 value
        opponentPlayerManager = playerManager.opponentPlayerManager;
        opponentPlayerStatus = playerManager.opponentPlayerManager.playerStatus;
        // if (!isSolo)
            empGrenadeAction = playerInput.actions["EMP"];
            attackAction = playerInput.actions["Attack"];

            empGrenadeAction.performed += ctx => TryUseEmpGrenade();
            attackAction.performed += ctx => TryAttack();
        
    }

    public void TryUseEmpGrenade()
    {
        if (Game_Manager.isPaused) return;
        if (playerStatus.hasEmpGrenade && !playerStatus.empOnCooldown)
        {
            empManager.EmpAnim.SetTrigger("Throw");
        }
        else
        {
            Debug.Log("Cannot use EMP: Either on cooldown or not available.");
        }
    }

    public void DetonateEmp()
    {
        playerStatus.hasEmpGrenade = false;
        Debug.Log("EMP Grenade used!");
        opponentPlayerManager.pvp.ApplyInvertControlDebuff(10f);
        empManager.StartEmpCooldown();
        gameDisplay.UpdateEMPStateIcon();
    }

    public void ApplyInvertControlDebuff(float duration)
    {
        if (TryBlockEmp() == true)
        {
            Debug.Log("EMP Blocked by Yun Jin's Rocks");
            return;
        }
        playerManager.shaker.boardShake();
        if (isInvertImmune)
        {
            isInvertImmune = !isInvertImmune;
            return;
        }
        if (!playerStatus.isInverted)
        {
            playerStatus.isInverted = true;
            playerManager.invertTimer = duration;
            var activePiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P1" : "P2")}")?.GetComponent<Piece>();
            if (activePiece != null)
                activePiece.Clear();
            // comboText.color = Color.red;
            // comboText.text = "Inverted Controls";
            Debug.Log("Controls inverted!");
            playerManager.audioManager.PlaySFX(playerManager.audioManager.EMP_clip);
            StartCoroutine(gameDisplay.BackPulse(10f, "#763700")); // "#763700"
        }
    }
    
    public bool TryBlockEmp()
    {
        // if(yunJinSkill != null)
        // {
        //     int rockCtr = yunJinSkill.rockCount;
        //     if (rockCtr > 0)
        //     {
        //         for (int i = rockCtr; i > 0; i--)
        //         {
        //             yunJinSkill.InvisRock(i);
        //         }
        //         yunJinSkill.StoneDestroyed();
        //         return true;
        //     }
            
        // }
        return false;
    }

    public void TryAttack()
    {
        if (playerStatus.atkOnCooldown)
        {
            //comboText.color = Color.red;
            //comboText.text = "Attack on Cooldown";
            Debug.Log("Attack is on cooldown!");
            return;
        }

        if (playerStatus.attackAmmo > 0)
        {
            
            // if (packHatSkill != null)
            // {
            //     if (packHatSkill.isSkillActive)
            //     {
            //         packHatSkill.packhatAnim.Play("Firing", 0, 0f);
            //     }
            // }

            playerStatus.attackAmmo--;
            //Camera.SetTrigger("Shake");
            var opponentPiece = GameObject.Find($"ActivePiece{(playerStatus.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();
            if (opponentPiece != null)
            {
                opponentPiece.Clear(); // Clear its tiles temporarily
            }

            opponentPlayerManager.pvp.ReceiveDeadLine();
            playerManager.audioManager.PlaySFX(playerManager.audioManager.attack);

            if (opponentPiece != null)
            {
                opponentPiece.Set(); // Re-set the piece tiles after push
            }

            playerManager.gameDisplay.Ammo_Update(playerStatus.attackAmmo);
            Debug.Log("Attack sent!");


            // Start cooldown
            playerStatus.atkOnCooldown = true;
            Invoke(nameof(ResetAttackCooldown), playerStatus.atkCD_Time);
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
        playerStatus.atkOnCooldown = false;
        //comboText.text = "";
        Debug.Log("Attack cooldown reset.");
    }

    public void ReceiveDeadLine()
    {
        if (playerManager.hardDropLockoutTimer > 0f)
        {
            // Delay deadline, queue it
            playerStatus.pendingDeadLines++;
            Debug.Log("Dead line queued due to Hard Drop lockout");
            return;
        }

        // Check if player has Yun Jin rocks to block the attack
        // if (TryBlockWithYunJinRocks())
        // {
        //     Debug.Log("Attack blocked by Yun Jin rocks!");
        //     return;
        // }

        playerBoard.shaker.boardShake();
        playerBoard.ApplyDeadLine();
    }
}
