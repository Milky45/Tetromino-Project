using UnityEngine;
using UnityEngine.UIElements;

public class EmpEvents : MonoBehaviour
{
    public Game_Manager gameManager;
    public Animator EmpAnim;

    public void Detonate_Event()
    {
        gameManager.pvp.DetonateEmp();
    }
}
