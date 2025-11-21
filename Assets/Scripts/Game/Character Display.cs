using UnityEngine;

public class CharacterDisplay : MonoBehaviour
{
    public bool isPlayer1 = true;
    public bool isTeamBattle = false;

    public GameObject[] charDisplay1 = new GameObject[8];
    public GameObject[] charDisplay2 = new GameObject[8];


    private void Awake()
    {
        // all character displays are disabled by default
        for (int i = 0; i < charDisplay1.Length; i++)
        {
            charDisplay1[i].SetActive(false);
        }
    }

}
