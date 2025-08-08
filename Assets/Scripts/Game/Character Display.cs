using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public bool isPlayer1 = true;

    public GameObject[] characterDisplay = new GameObject[8];

    private void Awake()
    {
        // all character displays are disabled by default
        for (int i = 0; i < characterDisplay.Length; i++)
        {
            characterDisplay[i].SetActive(false);
        }
    }

}
