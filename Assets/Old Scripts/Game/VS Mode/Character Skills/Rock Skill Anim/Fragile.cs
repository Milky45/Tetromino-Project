using UnityEngine;

public class Fragile : MonoBehaviour
{
    [Header("References")]
    public YunJinSkill yunJinSkill;
    public Animator P1RocksAnim;
    public Animator P2RocksAnim;
    public GameManager P1;
    public GameManagerP2 P2;
    public SpriteRenderer Rock1;
    public SpriteRenderer Rock2;
    public SpriteRenderer Rock3;

    [Header("Settings")]
    public bool isYunJinP1;

    public void ReturnRockIdle()
    {
        if (isYunJinP1 == true)
        {
            P1RocksAnim.SetTrigger("Return");
        }
        else if (isYunJinP1 == false)
        {
            P2RocksAnim.SetTrigger("Return");
        }
    }
    public void RocksPush()
    {
        if(isYunJinP1 == true)
        {
            P1RocksAnim.SetTrigger("Push");
        }
        else if (isYunJinP1 == false)
        {
            P2RocksAnim.SetTrigger("Push");
        }
    }

    public void Rock1Check()
    {
        if (yunJinSkill.Rock1Active == true)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("Rock1 disappears");
            Rock1.color = new Color(Rock1.color.r, Rock1.color.g, Rock1.color.b, 0f);
            if (isYunJinP1 == true)
            {
                Debug.Log("P2 receives deadline from Rock1");
                var activePiece = GameObject.Find("ActivePieceP2")?.GetComponent<PieceControllerP2>();

                if (activePiece != null)
                    activePiece.Clear();
                P2.ReceiveDeadLine();
                activePiece.Set();
                P1.attackAmmo--;
                if (P1.attackAmmo < 0)
                {
                    P1.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P1.AmmoBox.AmmoUpdateP1();
                

            }
            else if (isYunJinP1 == false)
            {
                Debug.Log("P1 receives deadline from Rock1");
                var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();

                if (activePiece != null)
                    activePiece.Clear();
                P1.ReceiveDeadLine();
                activePiece.Set();
                P2.attackAmmo--;
                if (P2.attackAmmo < 0)
                {
                    P2.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P2.AmmoBox.AmmoUpdateP2();
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
            Rock2.color = new Color(Rock2.color.r, Rock2.color.g, Rock2.color.b, 0f);
            if (isYunJinP1 == true)
            {
                Debug.Log("P2 receives deadline from Rock2");
                var activePiece = GameObject.Find("ActivePieceP2")?.GetComponent<PieceControllerP2>();
                if (activePiece != null)
                    activePiece.Clear();
                P2.ReceiveDeadLine();
                activePiece.Set();
                P1.attackAmmo--;
                if (P1.attackAmmo < 0)
                {
                    P1.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P1.AmmoBox.AmmoUpdateP1();
            }
            else if (isYunJinP1 == false)
            {
                Debug.Log("P1 receives deadline from Rock2");
                var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();
                if (activePiece != null)
                    activePiece.Clear();
                P1.ReceiveDeadLine();
                activePiece.Set();
                P2.attackAmmo--;
                if (P2.attackAmmo < 0)
                {
                    P2.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P2.AmmoBox.AmmoUpdateP2();
            }
            yunJinSkill.Rock2Active = false; // Reset Rock2Active after checking
        }
    }

    public void Rock3Check()
    {
        if (yunJinSkill.Rock3Active == true)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("Rock3 disappears");
            Rock3.color = new Color(Rock3.color.r, Rock3.color.g, Rock3.color.b, 0f);
            if (isYunJinP1 == true)
            {
                Debug.Log("P2 receives deadline from Rock3");
                var activePiece = GameObject.Find("ActivePieceP2")?.GetComponent<PieceControllerP2>();
                if (activePiece != null)
                    activePiece.Clear();
                P2.ReceiveDeadLine();
                activePiece.Set();
                P1.attackAmmo--;
                if (P1.attackAmmo < 0)
                {
                    P1.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P1.AmmoBox.AmmoUpdateP1();
            }
            else if (isYunJinP1 == false)
            {
                Debug.Log("P1 receives deadline from Rock3");
                var activePiece = GameObject.Find("ActivePieceP1")?.GetComponent<PieceController>();
                if (activePiece != null)
                    activePiece.Clear();
                P1.ReceiveDeadLine();
                activePiece.Set();
                P2.attackAmmo--;
                if (P2.attackAmmo < 0)
                {
                    P2.attackAmmo = 0; // Ensure attackAmmo does not go below 0
                }
                P2.AmmoBox.AmmoUpdateP2();
            }
            yunJinSkill.Rock3Active = false; // Reset Rock3Active after checking
        }
    }

}
