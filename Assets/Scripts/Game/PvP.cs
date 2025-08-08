using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class PvP : MonoBehaviour
{
    public Game_Manager gameManager;

    public Player player;
    public Player opponent;

    PlayerInput playerInput;
    InputAction empGrenadeAction;
    InputAction attackAction;

    private void Awake()
    {
        // assign oponent based on the opposite isPlayer1 value
        opponent = FindObjectsByType<Player>(FindObjectsSortMode.None).FirstOrDefault(p => p != player);

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

        empGrenadeAction = playerInput.actions["EMP"];
        attackAction = playerInput.actions["Attack"];

        empGrenadeAction.performed += ctx => TryUseEmpGrenade();
        attackAction.performed += ctx => TryAttack();
    }
    public void TryUseEmpGrenade()
    {
        if (gameManager.isPaused) return;
        if (player.hasEmpGrenade && !player.empOnCooldown)
        {
            player.hasEmpGrenade = false;
            Debug.Log("EMP Grenade used!");


            gameManager.StartEmpCooldown();

        }
        else
        {
            Debug.Log("Cannot use EMP: Either on cooldown or not available.");
        }
    }

    public void ApplyInvertControlDebuff(float duration)
    {
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
        }

    }

    public void TryAttack()
    {
        if (gameManager.isPaused) return;

        if (player.atkOnCooldown)
        {
            //comboText.color = Color.red;
            //comboText.text = "Attack on Cooldown";
            Debug.Log("Attack is on cooldown!");
            return;
        }

        if (player.attackAmmo > 0)
        {
            player.attackAmmo--;
            //Camera.SetTrigger("Shake");
            var opponentPiece = GameObject.Find($"ActivePiece{(player.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();
            if (opponentPiece != null)
            {
                opponentPiece.Clear(); // Clear its tiles temporarily
            }

            opponent.gameManager.ReceiveDeadLine();

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
