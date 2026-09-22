using UnityEngine;

public class ActiveRock : MonoBehaviour
{
    [Header("References")] // will figure out how to get the references from skill manager later
    private SkillManager skillManager; 
    private TriggerSkills triggerSkills;

    [Header("Rock Animator")]
    public Animator rockAnimator;

    [Header("Rock Settings")]
    public int rockIndex; // Index of the rock (0, 1, or 2)


    // destroy the rock when it collides with the opponent 2d collider
    // but first check if ur player 1 or player 2 and then check if the opponent is player 1 or player 2
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(skillManager.isPlayer1 && collision.CompareTag("Player2"))
        {
            Debug.Log($"Rock {rockIndex + 1} collided with Player 2");
            // call function upon collision
        }
        else if(!skillManager.isPlayer1 && collision.CompareTag("Player1"))
        {
            Debug.Log($"Rock {rockIndex + 1} collided with Player 1");
            // call function upon collision
        } 
        else
        {
            Debug.Log($"Rock {rockIndex + 1} collided with something else: {collision.gameObject.name}");
        }

        // disable the script and destroy the rock after collision
        skillManager.gameManager.player.attackAmmo--;
        this.enabled = false;
        Destroy(gameObject);
    }

    

}
