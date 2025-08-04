using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class YunJinSkill : MonoBehaviour
{
    [Header("Cooldown Settings")]
    public float cooldownTime = 60f;
    private float cooldownTimer = 0f;
    public bool isOnCooldown = true;

    [Header("Cooldown UI")]
    public TextMeshProUGUI cooldownText;
    private string cooldownTempText;
    private string originalText;
    private Color originalTextColor;

    [Header("Rocks Settings")]
    public TextMeshProUGUI RockDurUI;
    public SpriteRenderer Rock1;
    public SpriteRenderer Rock2;
    public SpriteRenderer Rock3;
    public bool Rock1Active = false;
    public bool Rock2Active = false;
    public bool Rock3Active = false;

    [Header("Fragile Reference")]
    public Fragile fragile;

    [Header("Yun Jin Skill Settings")]
    public SpriteRenderer PowerBar;
    public Sprite PowerBar_0;
    public Sprite PowerBar_1;
    public Sprite PowerBar_2;
    public Sprite PowerBar_3;
    public bool isYunJinP1 = true;
    public Animator P1YunJinAnim;
    public Animator P2YunJinAnim;
    public Animator Rock1Anim;
    public Animator Rock2Anim;
    public Animator Rock3Anim;
    public Animator Camera;
    public float charge;
    public float RockDur = 15f;
    private bool ActiveRock = false;
    public bool Fragile = false;
    public float chargeTime = 0f;
    public float maxCharge = 3f; // Max charge duration (3 seconds)
    public int rockCount; // Number of rocks to animate

    private bool isCharging = false;
    public enum SkillStrength { Light, Medium, Heavy }
    public SkillStrength strength;

    private void Start()
    {
        originalText = cooldownText.text;
        cooldownTimer = cooldownTime + 20f;
    }

    void Update()
    {
        if (ActiveRock == true)
        {
            RockDur -= Time.deltaTime;
            RockDurUI.text = Mathf.CeilToInt(RockDur).ToString();
            if (RockDur <= 0f)
            {
                ActiveRock = false;
                Rock1Active = false;
                Rock2Active = false;
                Rock3Active = false;
                ReturnRockIdle();
                RockDurUI.text = ""; // Reset UI text
                Fragile = false; // Reset Fragile state
            }
        }
        if (isOnCooldown)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownTimer = Mathf.Max(cooldownTimer, 0f);


            cooldownTempText = Mathf.CeilToInt(cooldownTimer).ToString();
            if (cooldownText != null)
            {
                cooldownText.text = cooldownTempText;
                cooldownText.color = Color.red; // Set to red while counting down
            }

            if (cooldownTimer <= 0f)
            {
                ReturnRockColor();
                isOnCooldown = false;

                if (cooldownText != null)
                {
                    cooldownText.text = "OK"; // Should be "0"
                    cooldownText.color = Color.green; // Restore original color
                }
            }
        }
    }

    public void InvisRock(int RockCtr)
    {
        switch (RockCtr)
        {
            case 1:
                Rock1.color = new Color(Rock1.color.r, Rock1.color.g, Rock1.color.b, 0f);
                Rock1Active = false;
                Fragile = false; // Reset Fragile state. Assuming that rock 1 is the final rock that blocks the attack
                Debug.Log("Rock1 blocked the attack");
                break;
            case 2:
                Rock2.color = new Color(Rock2.color.r, Rock2.color.g, Rock2.color.b, 0f);
                Rock2Active = false;
                Debug.Log("Rock2 blocked the attack");
                break;
            case 3:
                Rock3.color = new Color(Rock3.color.r, Rock3.color.g, Rock3.color.b, 0f);
                Rock3Active = false;
                Debug.Log("Rock3 blocked the attack");
                break;
        }
    }

    public void ActivateSkill(float tempCharge)
    {
        if (Fragile == false)
        { 
            charge = tempCharge;
            if (isYunJinP1 == true)
            {
                P1YunJinAnim.SetTrigger("Attack");
            }
            else if (isYunJinP1 == false)
            {
                P2YunJinAnim.SetTrigger("Attack");
            }
        }
    }

    public void ActivateFragile()
    {
        if (Fragile == false)
        {
            return;
        }
        fragile.RocksPush();
        Camera.SetTrigger("Shake");
        Debug.Log("Rocks pushed forward");
        if (strength == SkillStrength.Heavy)
        {
            cooldownTimer += 10f;
        }
        else if (strength == SkillStrength.Medium)
        {
            cooldownTimer += 7f;
        }
        else if (strength == SkillStrength.Light)
        {
            cooldownTimer += 5f;
        }
    }

    public void ExecuteSkill()
    {
        if (Fragile == true)
        {
            Fragile = false; // Reset Fragile state
            return;
        }
        Camera.SetTrigger("Shake");
        if (charge < 1f) strength = SkillStrength.Light;
        else if (charge < 2f) strength = SkillStrength.Medium;
        else if (charge >= 2f) strength = SkillStrength.Heavy;

        switch (strength)
        {
            case SkillStrength.Light:
                cooldownTimer += 5f;
                //RockDur = 6f;
                Rock1Anim.SetTrigger("Rock 1");
                Rock1Active = true; // Activate Rock1 for light strength
                rockCount = 1; // Set rock count for light strength
                break;
            case SkillStrength.Medium:
                cooldownTimer += 10f;
                //RockDur = 10f;
                Rock1Anim.SetTrigger("Rock 1");
                Rock2Anim.SetTrigger("Rock 2");
                Rock1Active = true; // Activate Rock1 for medium strength
                Rock2Active = true; // Activate Rock2 for medium strength
                rockCount = 2; // Set rock count for medium strength
                break;
            case SkillStrength.Heavy:
                cooldownTimer += 15f;
                //RockDur = 15f;
                Rock1Anim.SetTrigger("Rock 1");
                Rock2Anim.SetTrigger("Rock 2");
                Rock3Anim.SetTrigger("Rock 3");
                Rock1Active = true;
                Rock2Active = true;
                Rock3Active = true; // Activate Rock3 for heavy strength
                rockCount = 3; // Set rock count for heavy strength
                break;
        }
        cooldownTimer += cooldownTime;
        RockDur = cooldownTimer - 10f;
        isOnCooldown = true;
        ActiveRock = true;
        Fragile = true; // Set Fragile state to true when skill is executed

        Debug.Log($"Skill used: {strength}, Cooldown: {cooldownTime}s");
    }

    public void ReturnToIdle()
    {
        if (isYunJinP1 == true)
        {
            P1YunJinAnim.SetTrigger("Idle");
        }
        else if (isYunJinP1 == false)
        { 
            P2YunJinAnim.SetTrigger("Idle");
        }

    }

    public void ReturnRockColor()
    {
        Rock1.color = new Color(Rock1.color.r, Rock1.color.g, Rock1.color.b, 1f);
        Rock2.color = new Color(Rock2.color.r, Rock2.color.g, Rock2.color.b, 1f);
        Rock3.color = new Color(Rock3.color.r, Rock3.color.g, Rock3.color.b, 1f);
    }

    public void ReturnRockIdle()
    {
        Debug.Log("Returning rocks to idle state");
        if (strength == SkillStrength.Heavy)
        {
            Rock1Anim.SetTrigger("Return");
            Rock2Anim.SetTrigger("Return");
            Rock3Anim.SetTrigger("Return");
        }
        else if (strength == SkillStrength.Medium)
        {
            Rock1Anim.SetTrigger("Return");
            Rock2Anim.SetTrigger("Return");
        }
        else if (strength == SkillStrength.Light)
        {
            Rock1Anim.SetTrigger("Return");
        }
        rockCount = 0; // Reset rock count after returning to idle
    }

    public void StartSkillChargeDetection(InputAction skillAction, System.Action<float> onReleased)
    {
        if (Fragile == true)
        {
            if (isYunJinP1 == true)
            {
                P1YunJinAnim.SetTrigger("Attack");
            }
            else if (isYunJinP1 == false)
            {
                P2YunJinAnim.SetTrigger("Attack");
            }
            return;
        }

        if (isOnCooldown)
        {
            Debug.Log("Skill is on cooldown.");
            return;
        }
        StartCoroutine(DetectChargeCoroutine(skillAction, onReleased));
    }

    private IEnumerator DetectChargeCoroutine(InputAction skillAction, System.Action<float> onReleased)
    {
        isCharging = true;
        chargeTime = 0f;

        while (skillAction.ReadValue<float>() > 0f)
        {
            chargeTime += Time.deltaTime;
            chargeTime = Mathf.Min(chargeTime, maxCharge);
            if (chargeTime < 1f)
            {
                PowerBar.sprite = PowerBar_1;
            }
            else if (chargeTime < 2f)
            {
                PowerBar.sprite = PowerBar_2;
            }
            else if(chargeTime >= 2f)
            {
                PowerBar.sprite = PowerBar_3;
            }
            yield return null;
        }

        isCharging = false;
        PowerBar.sprite = PowerBar_0;
        onReleased?.Invoke(chargeTime);
        Debug.Log($"ChargeTime: {chargeTime}s");
    }

}


