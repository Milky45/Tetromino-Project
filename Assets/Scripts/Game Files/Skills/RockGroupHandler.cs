using UnityEngine;

public class RockGroupHandler : MonoBehaviour
{
    // this script will not be rellying on animators to push rocks towards the opponent
    //  it will only rely on x direction movement depending on the player 1 or player 2
    [Header("References")]
    public SkillManager skillManager; // Reference to the SkillManager script
    public GameObject[] rockPrefab; // Prefab for the rock object
    // container for the rocks to be spawned in
    public Transform[] rockSpawnPoints; // Array of spawn points for the rocks
    public Transform rockGroupContainer; // Container to hold the spawned rocks
    public ActiveRock[] activeRocks; // Array to hold references to the ActiveRock scripts for each rock

    public void SpawnRocks()
    {
        for (int i = 0; i < rockSpawnPoints.Length; i++)
        {
            if (i < skillManager.rockCtr) // Only spawn rocks up to the rockCount
            {
                GameObject rock = Instantiate(rockPrefab[i], rockSpawnPoints[i].position, Quaternion.identity);
                ActiveRock activeRockScript = rock.GetComponent<ActiveRock>();
                if (activeRockScript != null)
                {
                    activeRockScript.rockIndex = i; // Assign the index to the ActiveRock script
                    activeRocks[i] = activeRockScript; // Store the reference in the array
                }
            }
        }
    }
    
    public void PushRockGroup() 
    {
        float pushDirection = skillManager.isPlayer1 ? 1f : -1f; // Determine direction based on player
        float pushSpeed = skillManager.charSkills.rockPushForce; // Get the push speed from SkillManager
        // move the rock group container in the x direction based on the push speed and direction
        rockGroupContainer.Translate(Vector3.right * pushDirection * pushSpeed * Time.deltaTime);
    }

}
