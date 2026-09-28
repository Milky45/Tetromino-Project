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

    private bool isPushing = false;

    public void SpawnRocks()
    {
        if (rockSpawnPoints == null) return;
        activeRocks = new ActiveRock[rockSpawnPoints.Length];

        for (int i = 0; i < rockSpawnPoints.Length; i++)
        {
            if (skillManager != null && i < skillManager.rockCtr && rockPrefab != null && i < rockPrefab.Length)
            {
                GameObject rock = Instantiate(rockPrefab[i], rockSpawnPoints[i].position, Quaternion.identity);
                if (rockGroupContainer != null) rock.transform.parent = rockGroupContainer;
                ActiveRock activeRockScript = rock.GetComponent<ActiveRock>();
                if (activeRockScript != null)
                {
                    activeRockScript.Initialize(skillManager, i);
                    activeRocks[i] = activeRockScript;
                }
            }
        }
    }
    
    public void PushRockGroup() 
    {
        if (!isPushing)
        {
            StartCoroutine(PushRoutine());
        }
    }

    private System.Collections.IEnumerator PushRoutine()
    {
        isPushing = true;
        float pushDirection = (skillManager != null && skillManager.isPlayer1) ? 1f : -1f;
        float pushSpeed = (skillManager != null && skillManager.charSkills != null) ? skillManager.charSkills.rockPushForce : 5f;
        float duration = 3.0f;
        float elapsed = 0f;

        while (elapsed < duration && rockGroupContainer != null)
        {
            rockGroupContainer.Translate(Vector3.right * pushDirection * pushSpeed * Time.deltaTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        isPushing = false;
    }

}
