using UnityEngine;

public class AmmoDisplay : MonoBehaviour
{
    [Header("Insert ammo display for Player 1")]
    public GameObject P1Ammo_0;
    public GameObject P1Ammo_1;
    public GameObject P1Ammo_2;
    public GameObject P1Ammo_3;
    public GameObject P1Ammo_4;
    public GameObject P1Ammo_5;

    [Header("Insert ammo display for Player 2")]
    public GameObject P2Ammo_0;
    public GameObject P2Ammo_1;
    public GameObject P2Ammo_2;
    public GameObject P2Ammo_3;
    public GameObject P2Ammo_4;
    public GameObject P2Ammo_5;

    [Header("Insert grenade display for Player 1")]
    public GameObject P1EMP_0;
    public GameObject P1EMP_1;
    [Header("Insert grenade display for Player 2")]
    public GameObject P2EMP_0;
    public GameObject P2EMP_1;

    [Header("Insert Refereces of game managers here")]
    public GameManager gameP1;
    public GameManagerP2 gameP2;

    public void AmmoUpdateP1()
    {
        if (gameP1.attackAmmo == 1)
        {
            P1Ammo_0.SetActive(false);
            P1Ammo_1.SetActive(true);
            P1Ammo_2.SetActive(false);
            P1Ammo_3.SetActive(false);
            P1Ammo_4.SetActive(false);
            P1Ammo_5.SetActive(false);
        }
        else if (gameP1.attackAmmo == 2)
        {
            P1Ammo_0.SetActive(false);
            P1Ammo_1.SetActive(false);
            P1Ammo_2.SetActive(true);
            P1Ammo_3.SetActive(false);
            P1Ammo_4.SetActive(false);
            P1Ammo_5.SetActive(false);
        }
        else if (gameP1.attackAmmo == 3)
        {
            P1Ammo_0.SetActive(false);
            P1Ammo_1.SetActive(false);
            P1Ammo_2.SetActive(false);
            P1Ammo_3.SetActive(true);
            P1Ammo_4.SetActive(false);
            P1Ammo_5.SetActive(false);
        }
        else if (gameP1.attackAmmo == 4)
        {
            P1Ammo_0.SetActive(false);
            P1Ammo_1.SetActive(false);
            P1Ammo_2.SetActive(false);
            P1Ammo_3.SetActive(false);
            P1Ammo_4.SetActive(true);
            P1Ammo_5.SetActive(false);
        }
        else if (gameP1.attackAmmo == 5)
        {
            P1Ammo_0.SetActive(false);
            P1Ammo_1.SetActive(false);
            P1Ammo_2.SetActive(false);
            P1Ammo_3.SetActive(false);
            P1Ammo_4.SetActive(false);
            P1Ammo_5.SetActive(true);
        }
        else 
        {
            P1Ammo_0.SetActive(true);
            P1Ammo_1.SetActive(false);
            P1Ammo_2.SetActive(false);
            P1Ammo_3.SetActive(false);
            P1Ammo_4.SetActive(false);
            P1Ammo_5.SetActive(false);
        }
    }

    public void AmmoUpdateP2()
    {
        if (gameP2.attackAmmo == 1)
        {
            P2Ammo_0.SetActive(false);
            P2Ammo_1.SetActive(true);
            P2Ammo_2.SetActive(false);
            P2Ammo_3.SetActive(false);
            P2Ammo_4.SetActive(false);
            P2Ammo_5.SetActive(false);
        }
        else if (gameP2.attackAmmo == 2)
        {
            P2Ammo_0.SetActive(false);
            P2Ammo_1.SetActive(false);
            P2Ammo_2.SetActive(true);
            P2Ammo_3.SetActive(false);
            P2Ammo_4.SetActive(false);
            P2Ammo_5.SetActive(false);
        }
        else if (gameP2.attackAmmo == 3)
        {
            P2Ammo_0.SetActive(false);
            P2Ammo_1.SetActive(false);
            P2Ammo_2.SetActive(false);
            P2Ammo_3.SetActive(true);
            P2Ammo_4.SetActive(false);
            P2Ammo_5.SetActive(false);
        }
        else if (gameP2.attackAmmo == 4)
        {
            P2Ammo_0.SetActive(false);
            P2Ammo_1.SetActive(false);
            P2Ammo_2.SetActive(false);
            P2Ammo_3.SetActive(false);
            P2Ammo_4.SetActive(true);
            P2Ammo_5.SetActive(false);
        }
        else if (gameP2.attackAmmo == 5)
        {
            P2Ammo_0.SetActive(false);
            P2Ammo_1.SetActive(false);
            P2Ammo_2.SetActive(false);
            P2Ammo_3.SetActive(false);
            P2Ammo_4.SetActive(false);
            P2Ammo_5.SetActive(true);
        }
        else
        {
            P2Ammo_0.SetActive(true);
            P2Ammo_1.SetActive(false);
            P2Ammo_2.SetActive(false);
            P2Ammo_3.SetActive(false);
            P2Ammo_4.SetActive(false);
            P2Ammo_5.SetActive(false);
        }
    }

    public void GrenadeUpdateP1()
    {
        if (gameP1.hasEmpGrenade == true)
        {
            P1EMP_0.SetActive(false);
            P1EMP_1.SetActive(true);
        }
        else
        {
            P1EMP_0.SetActive(true);
            P1EMP_1.SetActive(false);
        }
    }

    public void GrenadeUpdateP2()
    {
        if (gameP2.hasEmpGrenade == true)
        {
            P2EMP_0.SetActive(false);
            P2EMP_1.SetActive(true);
        }
        else
        {
            P2EMP_0.SetActive(true);
            P2EMP_1.SetActive(false);
        }
    }
}
