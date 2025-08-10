using UnityEngine;

public class YunJinEvents : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Game_Manager gameManager;
    public YunJinSkill yunJinSkill;
    public Fragile fragile;

    private void Start()
    {
        if (gameManager.player.isPlayer1)
        {
            yunJinSkill = GameObject.Find("Character Manager P1").GetComponent<YunJinSkill>();
            fragile = GameObject.Find("Character Manager P1").GetComponent<Fragile>();
        }
        else
        {
            yunJinSkill = GameObject.Find("Character Manager P2").GetComponent<YunJinSkill>();
            fragile = GameObject.Find("Character Manager P2").GetComponent<Fragile>();
        }

        if (yunJinSkill == null)
        {
            Debug.LogWarning("YunJinSkill is null");
        }
        else if (fragile == null)
        {
            Debug.LogWarning("Fragile is null");
        }
    }

    public void Rock1Check()
    {
        fragile.Rock1Check();
    }

    public void Rock2Check()
    {
        fragile.Rock2Check();
    }

    public void Rock3Check()
    {
        fragile.Rock3Check();
    }

    public void ExecuteSkill()
    {
        yunJinSkill.ExecuteSkill();
    }

    public void ReturnAllRocks()
    {
        fragile.ReturnAllRocks();
    }

    public void DestroyAllRocks()
    {
        yunJinSkill.DestroyAllRocks();
    }
}
