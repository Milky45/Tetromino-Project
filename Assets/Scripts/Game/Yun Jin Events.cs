using UnityEngine;

public class YunJinEvents : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public YunJinSkill yunJinSkill;
    public Fragile fragile;
    public GameObject targetObj;

    private void Start()
    {
        yunJinSkill = targetObj.GetComponent<YunJinSkill>();
        fragile = targetObj.GetComponent<Fragile>();

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
    public void Execute_Skill()
    {   
        Debug.Log("Yun Jin Events executed");
        // eventCtr++;
        // Debug.Log($"Event count: {eventCtr}");
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
