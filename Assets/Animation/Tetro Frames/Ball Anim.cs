using UnityEngine;

public class BallAnim : MonoBehaviour
{
    public bool isPlayer1 = true;
    public TetroSkill tetroSkill;

    private void Start()
    {
        if (isPlayer1)
        {
            tetroSkill = GameObject.Find("Character Manager P1").GetComponent<TetroSkill>();
        }
        else
        {
            tetroSkill = GameObject.Find("Character Manager P2").GetComponent<TetroSkill>();
        }
    }

    public void BallEvent()
    {
        tetroSkill.ActivateSkill();
    }

    public void SfxEvent()
    {
        tetroSkill.SFX();
    }
}
