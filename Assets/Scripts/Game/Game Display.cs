using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class GameDisplay : MonoBehaviour
{
    public Game_Manager gameManager;

    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI empCDText;
    public TextMeshProUGUI skillCDText;
    public TextMeshProUGUI chipsText;
    public TextMeshProUGUI costText;

    public Transform mainTileMap;
    public Transform ghostTileMap;

    public SpriteRenderer[] heartIcons = new SpriteRenderer[3];
    public GameObject[] NextBlock = new GameObject[7];
    public GameObject[] HeldBlock = new GameObject[8];

    public SpriteRenderer backBase;

    public SpriteRenderer EMP_Icon;

    public SpriteRenderer PowerBar;
    public Sprite PowerBar_0;
    public Sprite PowerBar_1;
    public Sprite PowerBar_2;
    public Sprite PowerBar_3;
    public SpriteRenderer Rock1;
    public SpriteRenderer Rock2;
    public SpriteRenderer Rock3;
    public TextMeshProUGUI RockDurUI;

    public void UpdateChips(int chips)
    {
        chipsText.text = $"{chips}";
    }

    public void UpdateHeartIcons(int health)
    {
        for (int i = 0; i < heartIcons.Length; i++)
        {
            if (i < health)
            {
                heartIcons[i].enabled = true;
            }
            else
            {
                heartIcons[i].enabled = false;
            }
        }
    }

    public void EMP_CD_Update(float timeLeft)
    {
        timeLeft = Mathf.Max(timeLeft, 0f);

        if (timeLeft <= 0f)
        {
            empCDText.text = "";
        }
        else
        {
            empCDText.text = $"{timeLeft:F0}";
        }
    }

    public void SkillCooldownUpdate(float cooldownTime)
    {
        cooldownTime = Mathf.Max(cooldownTime, 0f);

        if (cooldownTime <= 0f)
        {
            skillCDText.text = "";
        }
        else
        {
            skillCDText.text = $"{cooldownTime:F0}"; // Display with one decimal place
        }
    }

    public void Ammo_Update(int ammoCount)
    {
        ammoText.text = $"{ammoCount}/{gameManager.player.maxAmmo}";
    }

    public void LogTetrominoStatus(TetrominoData next, TetrominoData held)
    {
        string nextType = next != null ? next.tetromino.ToString() : "None";
        string heldType = held != null ? held.tetromino.ToString() : "None";
        // Clear previous blocks
        foreach (var block in NextBlock)
        {
            block.SetActive(false);
        }
        foreach (var block in HeldBlock)
        {
            block.SetActive(false);
        }

        // switch case for next tetromino
        switch (next.tetromino)
        {
            case TetrominoType.I:
                NextBlock[0].SetActive(true);
                break;
            case TetrominoType.J:
                NextBlock[1].SetActive(true);
                break;
            case TetrominoType.L:
                NextBlock[2].SetActive(true);
                break;
            case TetrominoType.O:
                NextBlock[3].SetActive(true);
                break;
            case TetrominoType.S:
                NextBlock[4].SetActive(true);
                break;
            case TetrominoType.T:
                NextBlock[5].SetActive(true);
                break;
            case TetrominoType.Z:
                NextBlock[6].SetActive(true);
                break;
            default:
                Debug.LogWarning($"Unknown TetrominoType: {next.tetromino}");
                break;
        }

        if (held != null)
        {
            switch (held.tetromino)
            {
                case TetrominoType.I:
                    HeldBlock[0].SetActive(true);
                    break;
                case TetrominoType.J:
                    HeldBlock[1].SetActive(true);
                    break;
                case TetrominoType.L:
                    HeldBlock[2].SetActive(true);
                    break;
                case TetrominoType.O:
                    HeldBlock[3].SetActive(true);
                    break;
                case TetrominoType.S:
                    HeldBlock[4].SetActive(true);
                    break;
                case TetrominoType.T:
                    HeldBlock[5].SetActive(true);
                    break;
                case TetrominoType.Z:
                    HeldBlock[6].SetActive(true);
                    break;
            }
        }
        else
        {
            HeldBlock[7].SetActive(true);
        }

    }

    public IEnumerator BackPulse(float duration)
    {
        // Set to target color (#CE9A4D), wait, then fade back to original color
        float fadeDuration = duration;
        float elapsed = 0f;

        // Cache original color
        Color originalColor = backBase != null ? backBase.color : Color.white;

        // Parse target color from hex and preserve original alpha
        Color targetColor;
        if (!ColorUtility.TryParseHtmlString("#763700", out targetColor))
        {
            targetColor = new Color(118f / 255f, 55f / 255f, 0f / 255f, 1f);
        }
        targetColor.a = originalColor.a;

        if (backBase != null)
        {
            backBase.color = targetColor;
        }

        // Hold the target color for the specified duration
        yield return new WaitForSeconds(0f);

        // Fade back to the original color
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            if (backBase != null)
            {
                backBase.color = Color.Lerp(targetColor, originalColor, t);
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        if (backBase != null)
        {
            backBase.color = originalColor;
        }
    }

    public void UpdateEMPStateIcon()
    {
        if (gameManager.player.hasEmpGrenade && !gameManager.player.empOnCooldown)
        {
            EMP_Icon.color = new Color(EMP_Icon.color.r, EMP_Icon.color.g, EMP_Icon.color.b, 1f);
        }
        else if (gameManager.player.hasEmpGrenade && gameManager.player.empOnCooldown)
        {
            EMP_Icon.color = new Color(EMP_Icon.color.r, EMP_Icon.color.g, EMP_Icon.color.b, 0.5f);
        }
        else
        {
            EMP_Icon.color = new Color(EMP_Icon.color.r, EMP_Icon.color.g, EMP_Icon.color.b, 0f);
        }
    }
}
