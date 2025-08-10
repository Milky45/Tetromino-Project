using UnityEngine;

public class Fragile : MonoBehaviour
{
    [Header("References")]
    public YunJinSkill yunJinSkill;
    public Animator RocksAnim;

    public Animator Rock1Anim;
    public Animator Rock2Anim;
    public Animator Rock3Anim;
    public Game_Manager gameManager;
    public Game_Manager opponent;
    public SpriteRenderer Rock1;
    public SpriteRenderer Rock2;
    public SpriteRenderer Rock3;

    private void Awake()
    {
        yunJinSkill = GetComponent<YunJinSkill>();
    }

    public void RocksPush()
    {

        RocksAnim.SetTrigger("Push");
    }

    public void ReturnAllRocks()
    {
        RocksAnim.SetTrigger("Return");
    }

    public void Rock1Check()
    {
        if (yunJinSkill.Rock1Active == true)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("Rock1 disappears");
            if (Rock1 != null)
            {
                Rock1.color = new Color(Rock1.color.r, Rock1.color.g, Rock1.color.b, 0f);
            }
            Debug.Log("Opponent receives deadline from Rock1");
            var activePiece = GameObject.Find($"ActivePiece{(gameManager.player.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();

            if (opponent != null)
            {
                if (activePiece != null) activePiece.Clear();
                opponent.ReceiveDeadLine();
                if (activePiece != null) activePiece.Set();
                opponent.player.attackAmmo--;
                if (opponent.player.attackAmmo < 0)
                {
                    opponent.player.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
            }
            if (opponent != null && opponent.gameDisplay != null)
            {
                opponent.gameDisplay.Ammo_Update(opponent.player.attackAmmo);
            }

            yunJinSkill.Rock1Active = false; // Reset Rock1Active after checking
            yunJinSkill.Fragile = false; // Reset Fragile state after checking
        }
    }

    public void Rock2Check()
    {
        if (yunJinSkill.Rock2Active == true)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("Rock2 disappears");
            if (Rock2 != null)
            {
                Rock2.color = new Color(Rock2.color.r, Rock2.color.g, Rock2.color.b, 0f);
            }

            Debug.Log("Opponent receives deadline from Rock2");
            var activePiece = GameObject.Find($"ActivePiece{(gameManager.player.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();

            if (opponent != null)
            {
                if (activePiece != null) activePiece.Clear();
                opponent.ReceiveDeadLine();
                if (activePiece != null) activePiece.Set();
                opponent.player.attackAmmo--;
                if (opponent.player.attackAmmo < 0)
                {
                    opponent.player.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
            }
            if (opponent != null && opponent.gameDisplay != null)
            {
                opponent.gameDisplay.Ammo_Update(opponent.player.attackAmmo);
            }

            yunJinSkill.Rock2Active = false; // Reset Rock1Active after checking
            yunJinSkill.Fragile = false; // Reset Fragile state after checking
        }
    }

    public void Rock3Check()
    {
        if (yunJinSkill.Rock3Active == true)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("Rock3 disappears");
            if (Rock3 != null)
            {
                Rock3.color = new Color(Rock3.color.r, Rock3.color.g, Rock3.color.b, 0f);
            }

            Debug.Log("Opponent receives deadline from Rock3");
            var activePiece = GameObject.Find($"ActivePiece{(gameManager.player.isPlayer1 ? "P2" : "P1")}")?.GetComponent<Piece>();

            if (opponent != null)
            {
                if (activePiece != null) activePiece.Clear();
                opponent.ReceiveDeadLine();
                if (activePiece != null) activePiece.Set();
                opponent.player.attackAmmo--;
                if (opponent.player.attackAmmo < 0)
                {
                    opponent.player.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
            }
            if (opponent != null && opponent.gameDisplay != null)
            {
                opponent.gameDisplay.Ammo_Update(opponent.player.attackAmmo);
            }

            yunJinSkill.Rock3Active = false; // Reset Rock1Active after checking
            yunJinSkill.Fragile = false; // Reset Fragile state after checking
        }
    }

}
