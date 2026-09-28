using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerBoard playerBoard;
    public void ComboCount()
    {
        int linesCleared = playerBoard.ClearLines();
        
        int lineScore = 0;
        switch (linesCleared)
        {
            case 1: lineScore = 100; break;
            case 2: lineScore = 300; break;
            case 3: lineScore = 500; break;
            case 4: lineScore = 800; break;
            default: if (linesCleared > 4) lineScore = linesCleared * 200; break;
        }

        playerManager.playerStatus.score += lineScore;

        if (linesCleared > 0)
        {
            playerManager.playerStatus.comboCount += 1;

            int milestone = playerManager.playerStatus.comboCount / 2;
            if (milestone > playerManager.playerStatus.lastComboMilestone)
            {
                int ammoToAdd = milestone - playerManager.playerStatus.lastComboMilestone;
                for (int i = 0; i < ammoToAdd; i++)
                {
                    if (playerManager.playerStatus.attackAmmo < playerManager.playerStatus.maxAmmo)
                    {
                        playerManager.playerStatus.attackAmmo++;
                    }
                }
                playerManager.playerStatus.lastComboMilestone = milestone;
            }

            if (playerManager.playerStatus.comboCount >= 4 && !playerManager.playerStatus.hasEmpGrenade && !playerManager.playerStatus.empOnCooldown)
            {
                playerManager.playerStatus.hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                if (playerManager.gameDisplay != null) playerManager.gameDisplay.UpdateEMPStateIcon();
            }

            if (playerManager.playerStatus.comboCount > 1)
            {
                playerManager.playerStatus.score += 50 * playerManager.playerStatus.comboCount;
                if (playerManager.gameDisplay != null) playerManager.gameDisplay.UpdateComboText();
            }
        }
        else
        {
            playerManager.playerStatus.comboCount = 0;
            playerManager.playerStatus.lastComboMilestone = 0;
        }

        if (playerManager.gameDisplay != null) playerManager.gameDisplay.UpdateChips(playerManager.playerStatus.score);
    }
}