using UnityEngine;

public class Shaker : MonoBehaviour
{
    public Game_Manager gameManager;
    public Animator board;
    public Animator bullets;
    public Animator chips;
    public Animator cost;
    public Animator EMP;
    public Animator heart1;
    public Animator heart2;
    public Animator heart3;
    public Animator comboText;
    public Animator skill;

    public void boardShake()
    {
        board.Play("Shake Light", 0, 0f);
    }

    public void bulletShake()
    {
        bullets.Play("Shake Intense", 0, 0f);
        Debug.Log("Bullet Shake");
    }

    public void EMPShake()
    {
        EMP.Play("Shake Intense", 0, 0f);
        Debug.Log("EMP Shake");
    }

    public void heart1Shake()
    {
        heart1.Play("Shake Intense", 0, 0f);
        Debug.Log("Heart 1 Shake");
    }

    public void heart2Shake()
    {
        heart2.Play("Shake Intense", 0, 0f);
        Debug.Log("Heart 2 Shake");
    }

    public void heart3Shake()
    {
        heart3.Play("Shake Intense", 0, 0f);
        Debug.Log("Heart 3 Shake");
    }

    public void ChipsShake()
    {
        chips.Play("Shake Intense", 0, 0f);
        Debug.Log("Chips Shake");
    }

    public void ChipsDeductShake()
    {
        chips.Play("Shake Chips Deduct", 0, 0f);
        Debug.Log("Chips Deduct Shake");
    }

    public void CostShake()
    {
        cost.Play("Shake Intense", 0, 0f);
        Debug.Log("Cost Shake");
    }

    public void SkillShake()
    {
        skill.Play("Shake Intense", 0, 0f);
        Debug.Log("Skill Shake");
    }

    public void SkillOnCDShake()
    {
        skill.Play("Shake Skill CD", 0, 0f);
        Debug.Log("Skill On CD Shake");
    }

    public void CostInvalidShake()
    {
        cost.Play("Shake Cost Invalid", 0, 0f);
        Debug.Log("Cost Invalid Shake");
    }

    public void ComboShake()
    {
        comboText.Play("Shake light", 0, 0f);
        Debug.Log("Combo Shake");
    }

    public void ComboInvalidShake()
    {
        comboText.Play("Shake Invalid Combo", 0, 0f);
        Debug.Log("Combo Invalid Shake");
    }

    public void ResetComboText()
    {
        gameManager.gameDisplay.UpdateComboText();
    }
}
