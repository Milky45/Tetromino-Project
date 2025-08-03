using Unity.VisualScripting;
using UnityEngine;

public class EMPAnim : MonoBehaviour
{
    public GameManager P1;
    public GameManagerP2 P2;
    public Animator P1Anim;
    public Animator P2Anim;
    public GameObject P1EMP;
    public GameObject P2EMP;
    public SpriteRenderer empP1;
    public SpriteRenderer empP2;

    private void Awake()
    {
        empP1.color = new Color(empP1.color.r, empP1.color.g, empP1.color.b, 0f);
        empP2.color = new Color(empP2.color.r, empP2.color.g, empP2.color.b, 0f);
    }

    public void ThrowP1()
    {
        P1Anim.SetTrigger("Detonate");
    }

    public void P1Appear()
    {
        empP1.color = new Color(empP1.color.r, empP1.color.g, empP1.color.b, 1f);
    }

    public void P2Appear()
    {
        empP2.color = new Color(empP2.color.r, empP2.color.g, empP2.color.b, 1f);
    }

    public void ThrowP2()
    {
        P2Anim.SetTrigger("Detonate");
    }

    public void DetonateP1()
    {
        empP2.color = new Color(empP2.color.r, empP2.color.g, empP2.color.b, 0f);
        P2Anim.SetTrigger("Return");
        P1.ApplyInvertControlDebuff(5f);
    }
        
    public void DetonateP2()
    {
        empP1.color = new Color(empP1.color.r, empP1.color.g, empP1.color.b, 0f);
        P1Anim.SetTrigger("Return");
        P2.ApplyInvertControlDebuff(5f);
    }

    public void CheckRocksP2()
    {
        if(P1.chosenCharacter == 4 && P1.yunJinSkill.rockCount > 0)
        {
            P1.yunJinSkill.InvisRock(3);
            P1.yunJinSkill.InvisRock(2);
            P1.yunJinSkill.InvisRock(1);
            P1.yunJinSkill.rockCount = 0;
            empP2.color = new Color(empP2.color.r, empP2.color.g, empP2.color.b, 0f);
            P1.dontInvert = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("EMP destroyed the rocks");
            return;
        }
    }

    public void CheckRocksP1()
    {
        if (P2.chosenCharacter == 4 && P2.yunJinSkill.rockCount > 0)
        {
            P2.yunJinSkill.InvisRock(3);
            P2.yunJinSkill.InvisRock(2);
            P2.yunJinSkill.InvisRock(1);
            P2.yunJinSkill.rockCount = 0;
            empP1.color = new Color(empP1.color.r, empP1.color.g, empP1.color.b, 0f);
            P2.dontInvert = true;
            AudioManager.Instance.PlaySFX(AudioManager.Instance.attack);
            Debug.Log("EMP destroyed the rocks");
            return;
        }
    }
}
