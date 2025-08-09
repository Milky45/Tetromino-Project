using TMPro;
using UnityEngine;
using UnityEngine.UI;


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


}
