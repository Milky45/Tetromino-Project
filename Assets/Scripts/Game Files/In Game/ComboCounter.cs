using UnityEngine;

public class ComboCounter : MonoBehaviour
{
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private PlayerBoard playerBoard;
    public void ComboCount()
    {
        int linesCleared = playerBoard.ClearLines();
        playerManager.playerStatus.score += 100 * linesCleared;


        if (linesCleared > 0)
        {
            playerManager.playerStatus.comboCount += linesCleared;

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
                // if (!isSolo)
                // {
                //     gameDisplay.Ammo_Update(playerManager.playerStatus.attackAmmo);
                // }
            }
            //if (playerManager.playerStatus.comboCount >= 4 && !playerManager.playerStatus.hasEmpGrenade && !playerManager.playerStatus.empOnCooldown && !isSolo)
            
            if (playerManager.playerStatus.comboCount >= 4 && !playerManager.playerStatus.hasEmpGrenade && !playerManager.playerStatus.empOnCooldown)
            {
                playerManager.playerStatus.hasEmpGrenade = true;
                Debug.Log("EMP Grenade acquired!");
                playerManager.gameDisplay.UpdateEMPStateIcon();
                //shaker.EMPShake();
            }

            if (playerManager.playerStatus.comboCount > 1)
            {
                playerManager.playerStatus.score += 100;
                playerManager.gameDisplay.UpdateComboText();
                //shaker.ComboShake();

                // int soundIndex = Mathf.Clamp(playerManager.playerStatus.comboCount, 2, 13);
                // PlayComboSFX(soundIndex);
            }
            else
            {
                //audioManager.PlaySFX(audioManager.clear1);
                // play sfx here
            }
            //shaker.ChipsShake();
        }
        else
        {
            playerManager.playerStatus.comboCount = 0;
            playerManager.playerStatus.lastComboMilestone = 0;
            //shaker.ComboInvalidShake(); 
        }

        playerManager.gameDisplay.UpdateChips(playerManager.playerStatus.score);
    }
}