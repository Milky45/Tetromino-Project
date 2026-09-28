using UnityEngine;
using System.Collections;

public class SkillCoroutines : MonoBehaviour
{
    [Header("References")]
    public Coroutine pulseRoutine;
    public SkillManager skillManager;
    private Coroutine zeroAtkCooldownRoutine;


    public IEnumerator BlindPulse()
    {
        skillManager.BO_Renderer.enabled = true;
        skillManager.ballRenderer.enabled = false;
        Color baseColor = skillManager.BO_Renderer.color;
        skillManager.gameManager.pvp.opponentPlayerManager.shaker.boardShake();

        // Step 1: Set to full opacity
        skillManager.   BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 1f);
        // Hold full opacity for 3 seconds
        yield return new WaitForSeconds(2.5f);
        skillManager.animBall.SetTrigger("Return");

        // Step 2: Fade out over 3 seconds
        float fadeDuration = 2.5f;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            skillManager.BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }
        Debug.Log("FadeOut");
        skillManager.BO_Renderer.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0f);
        skillManager.BO_Renderer.enabled = false;
    }

    public void EnableZeroAttackCooldownForTenSeconds()
    {
        PlayerStatus status = null;
        if (skillManager != null && skillManager.gameManager != null && skillManager.gameManager.pvp != null)
        {
            status = skillManager.gameManager.pvp.playerStatus;
        }

        if (status == null)
        {
            Debug.LogWarning("Cannot apply zero attack cooldown: missing PlayerStatus reference.");
            return;
        }

        if (zeroAtkCooldownRoutine != null)
        {
            StopCoroutine(zeroAtkCooldownRoutine);
        }
        zeroAtkCooldownRoutine = StartCoroutine(ZeroAttackCooldownCoroutine(status, 10f));
    }

    private IEnumerator ZeroAttackCooldownCoroutine(PlayerStatus status, float durationSeconds)
    {
        float originalCooldown = status.atkCD_Time;

        status.atkOnCooldown = false;
        status.atkTempCD = originalCooldown;
        status.atkCD_Time = 0f;
        Debug.Log($"Attack cooldown set to 0 for {durationSeconds} seconds.");

        yield return new WaitForSeconds(durationSeconds);

        status.atkCD_Time = status.atkTempCD > 0f ? status.atkTempCD : originalCooldown;
        Debug.Log("Attack cooldown restored.");
        if (skillManager != null && skillManager.characterSkillAnim != null)
        {
            skillManager.characterSkillAnim.SetTrigger("Return");
        }

        zeroAtkCooldownRoutine = null;
    }

    public IEnumerator BoardFlip()
    {
        Debug.Log("Board flip started!");

        // Store original position and rotation
        Vector3 originalPosition = skillManager.opponentMainTileMap.position;
        Quaternion originalRotation = skillManager.opponentMainTileMap.rotation;
        Quaternion ghostOriginalRotation = skillManager.opponentGhostTileMap.rotation;

        // Flip Z position and rotation
        skillManager.opponentMainTileMap.position = new Vector3(originalPosition.x, originalPosition.y, -5f);
        skillManager.opponentMainTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);

        if (skillManager.opponentGhostTileMap != null)
        {
            skillManager.opponentGhostTileMap.position = skillManager.opponentMainTileMap.position;
            skillManager.opponentGhostTileMap.rotation = Quaternion.Euler(0f, 180f, 180f);
        }

        yield return new WaitForSeconds(skillManager.skillDuration);

        // Restore position and rotation
        skillManager.opponentMainTileMap.position = originalPosition;
        skillManager.opponentMainTileMap.rotation = originalRotation;

        if (skillManager.opponentGhostTileMap != null)
        {
            skillManager.opponentGhostTileMap.position = originalPosition;
            skillManager.opponentGhostTileMap.rotation = ghostOriginalRotation;
        }

        Debug.Log("Board flip ended!");
    }
}
